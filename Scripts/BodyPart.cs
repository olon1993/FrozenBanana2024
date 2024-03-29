using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public event Action OnDie;

    [SerializeField] public string Name;
    [SerializeField] public int MaxHp;
    [SerializeField] public string Region;
    [SerializeField] public List<AbilitySO> Abilities;
    bool containsLiveAbilitySO;

    [SerializeField] private int currentHp;
    [SerializeField] public bool IsCrippled = false;

    private void Start()
    {
        currentHp = MaxHp;

        for (int i = 0; i < Abilities.Count; i++)
        {
            if(Abilities[i].Name == "Live")
            {
                containsLiveAbilitySO = true;
                break;
            }
        }
    }

    public bool TakeDamage(int damage)
    {
        currentHp = Mathf.Max(0, currentHp - damage);

        Debug.Log(Name + " takes " + damage + " damage.");

        if(currentHp == 0)
        {
            if (containsLiveAbilitySO)
            {
                Debug.Log("BodyPart.Die");
                OnDie();
                return true;
            }
            else
            {
                Debug.Log("BodyPart.Cripple");
                IsCrippled = true;
            }
        }

        return false;
    }
}
