using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTower : BuildingBase
{
    protected override void Start()
    {
        base.Start();
        maxHealth = 100f;
        currentHealth = 0f;
        buildingType = BuildingType.BT_Tower;
    }
}
