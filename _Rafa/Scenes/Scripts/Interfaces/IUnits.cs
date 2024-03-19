using System;
using UnityEngine;

public interface IUnits
{
    public string GetName();
    public Team GetTeam();
    public Sprite GetSprite();
    public void InitializeUnit();
    public CombatStatistic GetStatistics();
    public void SetRanges(int moveRange, int attackRange);
    public (int move, int attack) GetRanges();
    public void SetMoved();
    public bool GetMoved();
    public bool GetReady();
    public void ChangeHP(int HPChange);
    public void ChangeMP(int MPChange);
    public (int HP, int MP) GetCurrentHealth();
    public void ActivateUnit();
    public void DeactivateUnit();
}