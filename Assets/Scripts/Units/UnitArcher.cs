using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitArcher : Unit
{
    
    protected override void Start()
    {
        base.Start();
        unitType = UnitType.UT_Archer;
    }
}
