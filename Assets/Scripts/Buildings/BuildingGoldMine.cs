using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingGoldMine : BuildingBase
{
    protected override void Start()
    {
        base.Start();
        buildFinished = true;
        buildingType = BuildingType.BT_GoldMine;
    }

    public override void SetFlagPosition(Vector3 newFlagPosition)
    {
    }

    [ClientCallback]
    public override void SelectBuilding()
    {
        if (!buildingUI)
        {
            BuildingUISetup();
        }
        buildingSelected = true;
        buildingUI.SetActive(true);
        updateUI = true;
    }
    
    [ClientCallback]
    public override void DeselectBuilding()
    {
        buildingSelected = false;
        buildingUI.SetActive(false);
        updateUI = false;
    }
}
