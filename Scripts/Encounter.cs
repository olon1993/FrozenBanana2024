using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Encounter : MonoBehaviour
{
    [SerializeField] List<GameObject> PlayerCharacters;
    [SerializeField] List<GameObject> Enemies;
    [SerializeField] GameObject ActionPanelGO;
    [SerializeField] GameObject ButtonPrefab;

    private List<GameObject> _turnOrder;
    private int _turnIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _turnOrder = new List<GameObject>();

        foreach (GameObject combatant in PlayerCharacters)
        {
            _turnOrder.Add(combatant);
            combatant.GetComponent<Combatant>().OnTurnComplete += NextTurn;

            foreach (BodyPart bodypart in combatant.GetComponent<Combatant>().BodyParts)
            {
                foreach(AbilitySO abilitySO in bodypart.Abilities)
                {
                    GameObject buttonIns = Instantiate(ButtonPrefab, ActionPanelGO.transform);
                    buttonIns.GetComponentInChildren<TMP_Text>().SetText(abilitySO.Name);
                    buttonIns.GetComponent<Button>().onClick.AddListener(delegate {
                        AttackButtonClicked(combatant.GetComponent<Combatant>(), abilitySO);
                    });
                }
            }
        }

        foreach(GameObject combatant in Enemies)
        {
            _turnOrder.Add(combatant);
            combatant.GetComponent<Combatant>().OnTurnComplete += NextTurn;
        }
    }

    void NextTurn()
    {
        _turnIndex++;
        if(_turnIndex >= _turnOrder.Count)
        {
            _turnIndex = 0;
        }

        if (PlayerCharacters.Contains(_turnOrder[_turnIndex]) == false)
        {
            // Enemy turn
            _turnOrder[_turnIndex].GetComponent<EnemyAi>().TakeTurn();
        }
    }

    void AttackButtonClicked(Combatant combatant, AbilitySO attackSO)
    {
        combatant.Attack(attackSO.Region);
    }
}
