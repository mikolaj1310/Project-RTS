using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingProduction : BuildingBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (productionQueue.Count > 0)
        {
            if (unitProductionProgress[0] > unitProductionTime[0])
            {
                mouseController.CmdSpawnUnit(mouseController.clientID, productionQueue[0], transform.position + RandomPointOnCircleEdge(2), GetDestinationAroundFlagMarker());
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
    
    private Vector3 RandomPointOnCircleEdge(float radius)
    {
        var vector2 = Random.insideUnitCircle.normalized * radius;
        return new Vector3(vector2.x, 0, vector2.y);
    }
    
    [Client]
    protected void AddUnitToProductionQueue(string unitPath, float productionTime, string iconPath)
    {
        if (productionQueue.Count > 7) return;
        
        productionQueue.Add(unitPath);
        unitProductionTime.Add(productionTime);
        unitProductionProgress.Add(0);
        productionQueueImages.Add(Resources.Load<Sprite>(iconPath));
    }
}
