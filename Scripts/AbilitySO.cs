using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilitySO", menuName = "Scriptable Objects/AbilitySO", order = 0)]
public class AbilitySO : ScriptableObject
{
    public string Name;
    public string Region;
    public bool IsActiveAbility;
    public int Damage;
    public int EnergyCost;
}
