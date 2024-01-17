using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    Combatant _combatant;
    string[] _regions = new string[] { "High", "Mid", "Low" };

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
        _combatant.Attack(_regions[Random.Range(0, _regions.Length - 1)]);
    }
}
