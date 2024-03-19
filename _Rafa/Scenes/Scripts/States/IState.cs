public interface IState
{
    void OnEnterState();
    void UpdateState();
    void OnExitState();
}