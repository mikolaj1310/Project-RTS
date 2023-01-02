using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingBarracks : BuildingBase
{
    protected override void Start()
    {
        base.Start();
        maxHealth = 100f;
        currentHealth = 0f;
        buildingType = BuildingType.BT_Barracks;
    }

    protected override void Update()
    {
        base.Update();
        if (productionQueue.Count > 0)
        {
            
            if (unitProductionProgress[0] > unitProductionTime[0])
            {
                //cid, prodqueue[0], spawn point, target
                mouseController.CmdSpawnUnit(mouseController.clientID, productionQueue[0], transform.position, GetDestinationAroundFlagMarker());
                productionQueue.RemoveAt(0);
                unitProductionTime.RemoveAt(0);
                unitProductionProgress.RemoveAt(0);
                productionQueueImages.RemoveAt(0);
            }
            else
            {
                if (buildingSelected)
                {
                    buildingUI.transform.Find("ProductionProgressBar").GetComponent<Slider>().value =
                        unitProductionProgress[0] / unitProductionTime[0];
                }
                unitProductionProgress[0] += Time.deltaTime;
            }
            
        }

        if (updateUI)
        {
            var prodQueue = buildingUI.transform.Find("ProductionQueue");

            for (int i = 0; i < 8; i++)
            {
                if (productionQueueImages.Count > i)
                {
                    Image image = prodQueue.Find("Queue" + i).Find("Image").GetComponent<Image>();
                    image.sprite = productionQueueImages[i];
                    image.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    Image image = prodQueue.Find("Queue" + i).Find("Image").GetComponent<Image>();
                    image.color = new Color(1, 1, 1, 0);
                }

            }
        }
    }

    [Client]
    private void AddUnitToProductionQueue(string unitPath, float productionTime, string iconPath)
    {
        if (productionQueue.Count > 7) return;
        
        productionQueue.Add(unitPath);
        unitProductionTime.Add(productionTime);
        unitProductionProgress.Add(0);
        productionQueueImages.Add(Resources.Load<Sprite>(iconPath));
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
        base.BuildingUISetup();
        buildingUI = GameObject.Find("GUI").transform.Find("BarracksMenu").gameObject;
        buildingUI.transform.Find("TrainPaladinButton").GetComponent<Button>().onClick.AddListener(SpawnPaladin);
        buildingUI.transform.Find("TrainArcherButton").GetComponent<Button>().onClick.AddListener(SpawnArcher);
    }

}
