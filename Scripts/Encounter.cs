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
    // Initialize at -1 so that the first call to NextTurn() begins turn order at index 0;
    private int _turnIndex = -1;
    private bool _isGameOver = false;
    private bool _isPlayerVictorious = false;

    // Start is called before the first frame update
    void Start()
    {
        _turnOrder = new List<GameObject>();

        CreateTurnOrder();
        NextTurn();
        //CreateActionList(_turnOrder[0].GetComponent<Combatant>());
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

    /// <summary>
    /// Checks to see if the encounter is concluded. The encounter concludes when the game is over
    /// or when the player is victorious. 
    /// </summary>
    /// <returns>bool, returns true if the encounter has concluded, returns false if the
    /// encounter is ongoing.</returns>
    bool EvaluateEncounterState()
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

        if (_isGameOver)
        {
            GameOver();
            return true;
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

        if (_isPlayerVictorious)
        {
            EncounterResults();
            return true;
        }

        return false;
    }

    void NextTurn()
    {
        ClearActionList();
        if(EvaluateEncounterState() == false)
        {
            _turnIndex++;
            if (_turnIndex >= _turnOrder.Count)
            {
                _turnIndex = 0;
            }

            _turnOrder[_turnIndex].GetComponent<Combatant>().StartTurn();
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
        Debug.Log("Victory!");
        ClearActionList();
        VictoryPanel.SetActive(true);
    }

    void GameOver()
    {
        Debug.Log("Game Over");
        ClearActionList();
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
                if (abilitySO.IsActiveAbility == false)
                {
                    continue;
                }

                GameObject buttonIns = Instantiate(ButtonPrefab, ActionPanelGO.transform);
                buttonIns.GetComponentInChildren<TMP_Text>().SetText(abilitySO.Name + " (" + abilitySO.Region + ")");
                buttonIns.GetComponent<Button>().onClick.AddListener(delegate {
                    AttackButtonClicked(combatant.GetComponent<Combatant>(), abilitySO);
                });
            }
        }

        GameObject endTurnButton = Instantiate(ButtonPrefab, ActionPanelGO.transform);
        endTurnButton.GetComponentInChildren<TMP_Text>().SetText("End Turn");
        endTurnButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            combatant.EndTurn();
        });
    }

    private void ClearActionList()
    {
        // Clear current action list
        foreach (Transform child in ActionPanelGO.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void AttackButtonClicked(Combatant combatant, AbilitySO attackSO)
    {
        if(combatant.CurrentEnergyPoints >= attackSO.EnergyCost)
        {
            combatant.Attack(attackSO);
            EvaluateEncounterState();
        }
        else
        {
            Debug.Log("Not enough energy!");
        }
    }
}
