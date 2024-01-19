using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    Combatant _combatant;

    // Start is called before the first frame update
    void Start()
    {
        _combatant = GetComponent<Combatant>();
        if(_combatant == null)
        {
            Debug.Log("No Combatant on " + name);
        }
    }

    public void TakeTurn()
    {
        // Check what body parts are active
        // Get lowest cost ability
        // While energy >= lowest cost
        //      continue to attack randomly
        // End Turn
        BodyPart buffer = _combatant.BodyParts[Random.Range(0, _combatant.BodyParts.Count)];
        _combatant.Attack(buffer.Abilities[Random.Range(0, buffer.Abilities.Count)]);
        _combatant.EndTurn();
    }
}
