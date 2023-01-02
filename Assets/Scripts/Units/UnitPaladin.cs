using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPaladin : Unit
{
    protected override void Start()
    {
        base.Start();
        unitType = UnitType.UT_Paladin;
    }
}
