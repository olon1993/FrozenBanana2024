using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimingAttack : MonoBehaviour, ISpecialAttack
{
    [SerializeField] bool debugModeOn;

    [SerializeField] PlayableDirector playableDirector;
    [SerializeField] TimelineAsset attackSequenceTimelineAsset;
    [SerializeField] SignalAsset startSuccessWindowSignalAsset;
    [SerializeField] SignalEmitter endSuccessWindowSignalEmitter;

    [SerializeField] Animator guideAnimator;
    [SerializeField] string countdownAnimatorStateName;

    [SerializeField] string RESET_TRIGGER;
    [SerializeField] string COUNTDOWN_TRIGGER;
    [SerializeField] string SUCCESS_WINDOW_TRIGGER;
    [SerializeField] string SUCCESS_TRIGGER;
    [SerializeField] string FAIL_TRIGGER;

    enum AttackState
    {
        BASE,
        COUNTDOWN,
        SUCCESS_WINDOW,
        SUCCESS,
        FAIL
    }

    AttackState attackState = AttackState.BASE;

    bool attackSucceeded;
    bool attackAttempted;

    public event EventHandler<ISpecialAttack.AttackResult> AttackCompletedEvent;

    void Awake()
    {
        if (playableDirector == null) 
        {
            Debug.LogError($"Missing PlayableDirector component on {name}");
        }
    }

    void Start()
    {
        SyncGuideAnimationWithCharacterAnimation();
        attackState = AttackState.BASE;
    }

    void SyncGuideAnimationWithCharacterAnimation()
    {
        // Get time until success window from attack sequence timeline
        double time = GetTimeOfSignalEmitterInTimeline(attackSequenceTimelineAsset, startSuccessWindowSignalAsset.name);

        // Set speed of guide countdown animation so time matches above
        SetCountdownAnimationDuration(time);
    }

    double GetTimeOfSignalEmitterInTimeline(TimelineAsset timelineAsset, string signalAssetName)
    {
        TrackAsset track = timelineAsset.GetRootTrack(0);
        int markerCount = track.GetMarkerCount();

        if (debugModeOn) print($"Marker count: {markerCount}");

        for (int i = 0; i < markerCount; i++)
        {
            IMarker marker = track.GetMarker(i);
            var signalEmitter = marker as SignalEmitter;
            var signalAsset = signalEmitter.asset;
            debugLog($"marker({i}) name: {signalAsset.name}");
            if (signalAsset.name == signalAssetName)
            {
                var timeUntilSignalEmitted = marker.time;
                debugLog($"Success window start time = {timeUntilSignalEmitted}");

                return timeUntilSignalEmitted;
            }
        }
        Debug.LogError($"Cannot return time of {signalAssetName} signal as it is not emitted from {timelineAsset} timeline");
        return Mathf.Infinity;
    }

    void SetCountdownAnimationDuration(double duration)
    {
        if (guideAnimator == null)
        {
            Debug.LogError($"{nameof(guideAnimator)} not assigned on {gameObject.name}");
            return;
        }
        
        AnimatorState countdownAnimatorState = GetAnimatorStateByName(guideAnimator, countdownAnimatorStateName, 0);

        if (countdownAnimatorState == null) 
        {
            Debug.LogError($"Unable to locate {countdownAnimatorStateName} animation state on {guideAnimator.name}");
            return;
        }

        debugLog("Found countdown animator state");

        // Get animationClip from countdown state
        Motion motion = countdownAnimatorState.motion;
        AnimationClip animationClip = motion as AnimationClip;

        // Get total duration of countdown animationclip
        var clipTime = animationClip.length;

        // Adjust speed of animator state so it lasts for the "duration" time
        var speed = clipTime / duration;
        debugLog($"Required countdown animation playback speed: {speed}");
        countdownAnimatorState.speed = (float)speed;
    }

    AnimatorState GetAnimatorStateByName(Animator animator, string stateName, int layerIndex)
    {
        // Get all animator states from controller
        AnimatorController controller = guideAnimator.runtimeAnimatorController as AnimatorController;
        AnimatorControllerLayer layer = controller.layers[layerIndex];
        ChildAnimatorState[] childAnimatorStates = layer.stateMachine.states;

        if (childAnimatorStates == null) return null;

        foreach (ChildAnimatorState c_state in childAnimatorStates)
        {
            if (c_state.state.name == stateName)
            {
                return c_state.state;
            }
        }
        return null;
    }

    public void StartAttack()
    {
        if (attackState != AttackState.BASE)
        {
            Debug.Log("Timing attack already in progress");
            return;
        }


        attackSucceeded = false;
        attackAttempted = false;
        debugLog("TimingAttackStarted");
        playableDirector.Play();
        guideAnimator.SetTrigger(COUNTDOWN_TRIGGER);
        attackState = AttackState.COUNTDOWN;
    }

    public void SuccessWindowStart()
    {
        if (attackState == AttackState.COUNTDOWN) 
        { 
            debugLog("TimingAttackSuccessWindowStart");
            attackState = AttackState.SUCCESS_WINDOW;
            guideAnimator.SetTrigger(SUCCESS_WINDOW_TRIGGER);
        }
    }

    public void SuccessWindowEnd() 
    {
        if (attackState != AttackState.SUCCESS_WINDOW) return;

        debugLog("Timing Attack Failed: Too Slow");
        attackState = AttackState.FAIL;
        guideAnimator.SetTrigger(FAIL_TRIGGER);
    }

    public void InitialAttackEnd()
    {
        if (attackState == AttackState.SUCCESS)
        {
            DoSuccessAction();
        }
        else 
        {
            DoFailureAction();
        }
    }
    void DoSuccessAction()
    {
        // continue to second attack
    }

    void DoFailureAction()
    {
        playableDirector.Stop();
        EndOfTimingAttack();
    }

    public void SecondAttackEnd()
    {
        EndOfTimingAttack();
    }

    void EndOfTimingAttack()
    {
        AttackCompletedEvent.Invoke(this, attackState == AttackState.SUCCESS ? ISpecialAttack.AttackResult.SUCCESS : ISpecialAttack.AttackResult.FAIL);

        ResetGuideAnimatorTriggers();
        guideAnimator.SetTrigger(RESET_TRIGGER);
        attackState = AttackState.BASE;
    }

    void ResetGuideAnimatorTriggers()
    {
        guideAnimator.ResetTrigger(RESET_TRIGGER);
        guideAnimator.ResetTrigger(COUNTDOWN_TRIGGER);
        guideAnimator.ResetTrigger(SUCCESS_WINDOW_TRIGGER);
        guideAnimator.ResetTrigger(SUCCESS_TRIGGER);
        guideAnimator.ResetTrigger(FAIL_TRIGGER);
    }

    void OnAttack()
    {
        if (attackAttempted) { return; }

        attackAttempted = true;

        switch (attackState)
        {
            case AttackState.COUNTDOWN:
                debugLog("Timing Attack Failed: too quick");
                guideAnimator.SetTrigger(FAIL_TRIGGER);
                attackState = AttackState.FAIL;
                attackSucceeded = false;
                break;

            case AttackState.SUCCESS_WINDOW:
                debugLog("Timing Attack Successfull");
                guideAnimator.SetTrigger(SUCCESS_TRIGGER);
                attackState = AttackState.SUCCESS;
                attackSucceeded = true;
                break;

            default: 
                break;
        }
    }

    void debugLog(string msg)
    {
        if (debugModeOn) print(msg);    
    }
}

public interface ISpecialAttack
{
    public enum AttackResult
    {
        SUCCESS,
        FAIL
    }
    public void StartAttack();

    event EventHandler<AttackResult> AttackCompletedEvent;
}
