using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Combatant : MonoBehaviour
{
    public event Action OnTurnComplete;

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

    public void Attack(string region)
    {
        Debug.Log(name + " attacks " + Enemy.name + "!");
        BodyPart[] enemyBodyParts = Enemy.GetComponentsInChildren<BodyPart>();
        for(int i = 0; i < enemyBodyParts.Length; i++)
        {
            if(enemyBodyParts[i].Region == region)
            {
                enemyBodyParts[i].TakeDamage(5);
            }
        }

        OnTurnComplete();
    }

    public void Die()
    {
        IsDead = true;
    }
}
