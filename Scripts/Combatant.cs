using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Combatant : MonoBehaviour
{
    public event Action OnTurnComplete;

    [SerializeField] int baseEnergyPoints = 10;
    [SerializeField] public int CurrentEnergyPoints;
    [SerializeField] public List<BodyPart> BodyParts;
    [SerializeField] Combatant Enemy;
    [SerializeField] public bool IsDead = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (BodyPart bodyPart in BodyParts)
        {
            bodyPart.OnDie += Die;
        }
    }

    public void StartTurn()
    {
        CurrentEnergyPoints = baseEnergyPoints;
    }

    public void EndTurn()
    {
        OnTurnComplete();
    }

    public void Attack(AbilitySO attackSO)
    {
        Debug.Log(name + " attacks " + Enemy.name + "!");
        CurrentEnergyPoints -= attackSO.EnergyCost;

        BodyPart[] enemyBodyParts = Enemy.GetComponentsInChildren<BodyPart>();
        for(int i = 0; i < enemyBodyParts.Length; i++)
        {
            if(enemyBodyParts[i].Region == attackSO.Region)
            {
                enemyBodyParts[i].TakeDamage(attackSO.Damage);
            }
        }
    }

    public void Die()
    {
        IsDead = true;
    }
}
