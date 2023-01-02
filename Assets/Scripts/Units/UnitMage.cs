using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMage : Unit
{
    
    protected override void Start()
    {
        base.Start();
        unitType = UnitType.UT_Mage;
    }
}
