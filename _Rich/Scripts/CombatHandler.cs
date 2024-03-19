using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    [SerializeField] GameObject timingAttackGameObject;

    ISpecialAttack timingAttack;

    void Start()
    {
        timingAttack = timingAttackGameObject.GetComponent<ISpecialAttack>();

        if (timingAttack != null ) 
        {
            timingAttack.AttackCompletedEvent += OnTimingAttackCompleted;
        }
    }

    public void OnTimingAttackStart()
    {
        timingAttack.StartAttack();
    }

    private void OnTimingAttackCompleted(object sender, ISpecialAttack.AttackResult result)
    {
        if (result == ISpecialAttack.AttackResult.SUCCESS)
        {
            DoDoubleDamage();
        }
        else
        {
            DoNormalDamage();
        }
    }

    void DoDoubleDamage()
    {
        print("Double damage");
    }

    void DoNormalDamage()
    {
        print("NormalDamage");
    }
}
