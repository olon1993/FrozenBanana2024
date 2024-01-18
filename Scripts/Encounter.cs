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
    [SerializeField] GameObject VictoryPanel;
    [SerializeField] GameObject DefeatPanel;

    private List<GameObject> _turnOrder;
    private int _turnIndex = 0;
    private bool _isGameOver = false;
    private bool _isPlayerVictorious = false;

    // Start is called before the first frame update
    void Start()
    {
        _turnOrder = new List<GameObject>();

        CreateTurnOrder();
        CreateActionList(_turnOrder[0].GetComponent<Combatant>());
    }

    void EvaluateEncounterState()
    {
        _isGameOver = true;
        foreach (GameObject gameObject in PlayerCharacters)
        {
            if (gameObject.GetComponent<Combatant>().IsDead == false)
            {
                _isGameOver = false;
            }
            else
            {
                if (_turnOrder.Contains(gameObject))
                {
                    _turnOrder.Remove(gameObject);
                }
            }
        }

        _isPlayerVictorious = true;
        foreach (GameObject gameObject in Enemies)
        {
            if(gameObject.GetComponent<Combatant>().IsDead == false)
            {
                _isPlayerVictorious = false;
            }
            else
            {
                if (_turnOrder.Contains(gameObject))
                {
                    _turnOrder.Remove(gameObject);
                }
            }
        }
    }

    void NextTurn()
    {
        EvaluateEncounterState();
        if (_isGameOver)
        {
            GameOver();
        }
        else if (_isPlayerVictorious)
        {
            EncounterResults();
        }
        else
        {
            _turnIndex++;
            if (_turnIndex >= _turnOrder.Count)
            {
                _turnIndex = 0;
            }

            if (PlayerCharacters.Contains(_turnOrder[_turnIndex]))
            {
                CreateActionList(_turnOrder[_turnIndex].GetComponent<Combatant>());
            }
            else
            {
                // Enemy turn
                _turnOrder[_turnIndex].GetComponent<EnemyAi>().TakeTurn();
            }
        }
    }

    void EncounterResults()
    {
        foreach (Transform child in ActionPanelGO.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Victory!");
        VictoryPanel.SetActive(true);
    }

    void GameOver()
    {
        foreach (Transform child in ActionPanelGO.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Game Over");
        DefeatPanel.SetActive(true);
    }

    void CreateActionList(Combatant combatant)
    {
        foreach (BodyPart bodypart in combatant.GetComponent<Combatant>().BodyParts)
        {
            if (bodypart.IsCrippled)
            {
                continue;
            }

            foreach (AbilitySO abilitySO in bodypart.Abilities)
            {
                GameObject buttonIns = Instantiate(ButtonPrefab, ActionPanelGO.transform);
                buttonIns.GetComponentInChildren<TMP_Text>().SetText(abilitySO.Name + " (" + abilitySO.Region + ")");
                buttonIns.GetComponent<Button>().onClick.AddListener(delegate {
                    AttackButtonClicked(combatant.GetComponent<Combatant>(), abilitySO);
                });
            }
        }
    }

    void CreateTurnOrder()
    {
        foreach (GameObject playerCharacter in PlayerCharacters)
        {
            Combatant combatant = playerCharacter.GetComponent<Combatant>();
            combatant.OnTurnComplete += NextTurn;

            if (combatant.IsDead)
            {
                continue;
            }

            _turnOrder.Add(playerCharacter);
        }

        int playerCount = _turnOrder.Count;

        foreach (GameObject enemy in Enemies)
        {
            Combatant combatant = enemy.GetComponent<Combatant>();
            combatant.OnTurnComplete += NextTurn;

            if (combatant.IsDead)
            {
                continue;
            }

            _turnOrder.Add(enemy);
        }
    }

    void AttackButtonClicked(Combatant combatant, AbilitySO attackSO)
    {
        ClearActionList();
        combatant.Attack(attackSO.Region);
    }

    private void ClearActionList()
    {
        // Clear current actionh list
        foreach (Transform child in ActionPanelGO.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
