using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAlchemist : BuildingBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        maxHealth = 100f;
        currentHealth = 0f;
        buildingType = BuildingType.BT_Alchemist;
    }
}
