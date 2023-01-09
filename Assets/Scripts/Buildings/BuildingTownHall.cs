using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingTownHall : BuildingProduction
{
    [SerializeField] public int ownerID;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        maxHealth = 100f;
        currentHealth = 100f;
        buildingType = BuildingType.BT_TownHall;
        buildFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        SetMouseController();
    }

    [ClientCallback]
    private void SetMouseController()
    {
        if (!mouseController)
        {
            var mcs = FindObjectsOfType<MouseController>();
            foreach (var mc in mcs)
            {
                if (mc.gameObject.GetComponent<NetworkIdentity>().isOwned)
                {
                    mouseController = mc;
                }
            }
        }
    }

    [Client]
    private void SpawnPeasant()
    {
        if (!buildFinished) { return; }
        var unitPath = "Prefabs/Units/P_UnitPeasant";
        var iconPath = "Prefabs/Art/Icons/UI_PeasantIcon";
        AddUnitToProductionQueue(unitPath, 2, iconPath);
        //mouseController.CmdSpawnUnit(mouseController.clientID, unitPath, flagMarker.transform.position);
    }
    
    [Client]
    protected override void BuildingUISetup()
    {
        buildingUI = GameObject.Find("GUI").transform.Find("TownHallMenu").gameObject;
        buildingUI.transform.Find("TrainPeasantButton").GetComponent<Button>().onClick.AddListener(SpawnPeasant);
    }
}
