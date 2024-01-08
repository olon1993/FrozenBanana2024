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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
