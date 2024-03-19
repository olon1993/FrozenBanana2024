using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CombatStatistic
{
    public int Attack;
    public int Defense;
    public int Move;
    public int Resistance;
    public int HP;
    public int MP;
}
public class CombatUnit : MonoBehaviour, IUnits
{
    // Set up data
    public Team UnitTeam;
    public string UnitName;
    public CombatStatistic statistics;
    public int AttackRange;
    public Sprite _readySprite;
    public Sprite _sleepSprite;

    // Internal State
    public class UnitState
    {
        public int currentHP;
        public int currentMP;
        public bool ready;
        public bool moved;
    }
    UnitState _currentState;

    // UI
    [SerializeField]  Image _healthBar;

    public void InitializeUnit()
    {
        _currentState = new UnitState()
        {
            currentHP = statistics.HP,
            currentMP = statistics.MP,
        };
    }
    public string GetName()
    {
        return UnitName;
    }
    public Sprite GetSprite()
    {
        return _readySprite;
    }
    public Team GetTeam()
    {
        return UnitTeam;
    }
    public CombatStatistic GetStatistics()
    {
        return statistics;
    }
    public void SetRanges(int moveRange, int attackRange)
    {
        statistics.Move = moveRange;
        AttackRange = attackRange;
    }
    public (int move, int attack) GetRanges()
    {
        return (statistics.Move, AttackRange);
    }
    public (int HP, int MP) GetCurrentHealth()
    {
        return (_currentState.currentHP, _currentState.currentMP);
    }
    public void ChangeHP(int HPChange)
    {
        _currentState.currentHP = Math.Clamp(_currentState.currentHP + HPChange, 0, statistics.HP);
        float fillValue = (float)_currentState.currentHP / (float)statistics.HP;
        _healthBar.fillAmount = (float)_currentState.currentHP / (float)statistics.HP;
    }
    public void ChangeMP(int MPChange)
    {
        _currentState.currentMP = Math.Clamp(_currentState.currentMP + MPChange, 0, statistics.MP);
    }
    public void SetMoved()
    {
        if(_currentState is null) 
        { 
            Debug.LogError("Unit not initialized.");
            return;
        }
        _currentState.moved = true;
    }
    public bool GetReady()
    {
        if(_currentState is null) 
        { 
            Debug.LogError("Unit not initialized.");
            return false;
        }
        return _currentState.ready;
    }
    public bool GetMoved()
    {
        if(_currentState is null) 
        { 
            Debug.LogError("Unit not initialized.");
            return false;
        }
        return _currentState.moved;
    }
    public void ActivateUnit()
    {
        if(_currentState is null) 
        { 
            Debug.LogError("Unit not initialized.");
            return;
        }

        //gameObject.GetComponent<SpriteRenderer>().sprite = _readySprite;
        _currentState.ready = true;
        _currentState.moved = false;
    }
    public void DeactivateUnit()
    {
        if(_currentState is null) 
        { 
            Debug.LogError("Unit not initialized.");
            return;
        }

        //gameObject.GetComponent<SpriteRenderer>().sprite = _sleepSprite;
        _currentState.ready = false;
        _currentState.moved = true;
    }
}
