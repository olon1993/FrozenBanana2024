using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour 
{

    [SerializeField] Camera _camera;
    [SerializeField] float cameraMoveSpeed;
    Bounds cameraBounds;
    [SerializeField] GridSystem gridController;

    // Combat Tile Display
    [SerializeField] GameObject tilePanel;
    [SerializeField] Image tileSprite;
    [SerializeField] TMP_Text tileName;
    [SerializeField] TMP_Text avoidBonus;
    [SerializeField] TMP_Text defenseBonus;

    // Combat Unit Display
    [SerializeField] GameObject unitPanel;
    [SerializeField] Image unitSprite;
    [SerializeField] TMP_Text unitName;
    [SerializeField] TMP_Text moveValue;
    [SerializeField] TMP_Text HPValue;

    // Combat Preview
    [SerializeField] GameObject combatPanel;
    [SerializeField] TMP_Text attackerName;
    [SerializeField] TMP_Text defenderName;
    [SerializeField] TMP_Text attackerDmg;
    [SerializeField] TMP_Text defenderDmg;
    [SerializeField] TMP_Text attackerHP;
    [SerializeField] TMP_Text defenderHP;
    [SerializeField] TMP_Text attackerHit;
    [SerializeField] TMP_Text defenderHit;
    [SerializeField] TMP_Text attackerCrit;
    [SerializeField] TMP_Text defenderCrit;

    void Start()
    {
        // Tilemap boundary
        cameraBounds = gridController.MapGrid.localBounds;
    }
    public void DisplayBattlePhase(Team phase)
    {

    }
    public void CloseBattlePhaseDisplay()
    {
    }
    public void DisplayCombatPreview(IUnits attacker, IUnits defender, CombatTile attackerTile, CombatTile defenderTile)
    {
        attackerName.text = attacker.GetName();
        defenderName.text = defender.GetName();
        attackerHP.text = attacker.GetCurrentHealth().HP.ToString();
        defenderHP.text = defender.GetCurrentHealth().HP.ToString();
        int attkDmg = attacker.GetStatistics().Attack - defender.GetStatistics().Defense - defenderTile.DefenseBonus;
        int defDmg =  defender.GetStatistics().Attack - attacker.GetStatistics().Defense - attackerTile.DefenseBonus;
        attackerDmg.text = Clamp(0, attkDmg, 99).ToString();
        defenderDmg.text = Clamp(0, defDmg, 99).ToString();
        attackerHit.text = "100%";
        defenderHit.text = "100%";
        attackerCrit.text = "0%";
        defenderCrit.text = "0%";
        combatPanel.SetActive(true);
    }
    public void CloseCombatPreview()
    {
        combatPanel.SetActive(false);
    }
    public void DisplayActionMenu()
    {

    }

    public void DisplayTileData(CombatTile tile)
    {
        tileSprite.sprite = tile.TileSprite;
        tileName.text = tile.GetName();
        avoidBonus.text = tile.AvoidBonus.ToString();
        defenseBonus.text = tile.DefenseBonus.ToString();
        tilePanel.SetActive(true);
    }   

    public void HideTileData()
    {
        tilePanel.SetActive(false);
    }

    public void DisplayUnitData(IUnits unit)
    {
        
        unitSprite.sprite = unit.GetSprite();
        unitName.text = unit.GetName();
        moveValue.text = unit.GetStatistics().Move.ToString();
        HPValue.text = unit.GetCurrentHealth().HP.ToString();
        unitPanel.SetActive(true);
    }   

    public void HideUnitData()
    {
        unitPanel.SetActive(false);
    }

    void Update()
    {
        int left = Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
        int right = Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;
        int up = Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0;
        int down = Input.GetKeyDown(KeyCode.DownArrow) ? -1 : 0;

        Vector3 current = _camera.transform.position;
        _camera.transform.position = new Vector3
        (
            Clamp(cameraBounds.min.x, current.x + cameraMoveSpeed * (left + right), cameraBounds.max.x),  
            Clamp(cameraBounds.min.y, current.y + cameraMoveSpeed * (up + down), cameraBounds.max.y), 
            current.z
        );
    }

    float Clamp(float min, float value, float max)
    {
        if(value >= min && value <= max)
        {
            return value;
        }

        if(value < min) return min;
        return max;
    }

}