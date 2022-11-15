using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    private Unit unit;
    
    public IdleState(Unit nUnit) : base(nUnit.gameObject)
    {
        unit = nUnit;
    }

    public override Type Tick()
    {
        if (unit.target != null)
        {
            return typeof(MoveState);
        }
        return null;
    }
}
