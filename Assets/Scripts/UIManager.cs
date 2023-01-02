using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public BuildingController buildingController;
    
    private void Start()
    {
    }

    public void BuildHouseButton()
    {
        buildingController.CreateBuilding(BuildingType.BT_House);
    }
    public void BuildBarracksButton()
    {
        buildingController.CreateBuilding(BuildingType.BT_Barracks);
    }
    public void BuildBankButton()
    {
        buildingController.CreateBuilding(BuildingType.BT_Bank);
    }
    public void BuildTowerButton()
    {
        buildingController.CreateBuilding(BuildingType.BT_Tower);
    }
    public void BuildAlchemistButton()
    {
        buildingController.CreateBuilding(BuildingType.BT_Alchemist);
    }
    public void BuildLumberMillButton()
    {
        buildingController.CreateBuilding(BuildingType.BT_LumberMill);
    }
}
