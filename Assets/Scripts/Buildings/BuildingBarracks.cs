using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingBarracks : BuildingProduction
{
    protected override void Start()
    {
        base.Start();
        maxHealth = 100f;
        currentHealth = 0f;
        buildingType = BuildingType.BT_Barracks;
    }

    [Client]
    private void SpawnPaladin()
    {
        if (!buildFinished) { return; }
        var unitPath = "Prefabs/Units/P_UnitPaladin";
        var iconPath = "Prefabs/Art/Icons/UI_PaladinIcon";
        AddUnitToProductionQueue(unitPath, 2, iconPath);
    }
    
    [Client]
    private void SpawnArcher()
    {
        if (!buildFinished) { return; }
        var unitPath = "Prefabs/Units/P_UnitArcher";
        var iconPath = "Prefabs/Art/Icons/UI_ArcherIcon";
        AddUnitToProductionQueue(unitPath, 2, iconPath);
        //mouseController.CmdSpawnUnit(mouseController.clientID, unitPath, flagMarker.transform.position);
    }
    

    [Client]
    protected override void BuildingUISetup()
    {
        buildingUI = GameObject.Find("GUI").transform.Find("BarracksMenu").gameObject;
        buildingUI.transform.Find("TrainPaladinButton").GetComponent<Button>().onClick.AddListener(SpawnPaladin);
        buildingUI.transform.Find("TrainArcherButton").GetComponent<Button>().onClick.AddListener(SpawnArcher);
    }

}
