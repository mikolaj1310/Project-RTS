using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPeasant : Unit
{
    
    protected override void Start()
    {
        base.Start();
        unitType = UnitType.UT_Peasant;
    }
}
