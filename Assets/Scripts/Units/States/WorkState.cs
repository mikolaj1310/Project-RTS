using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkState : BaseState
{
    private Unit unit;
    
    public WorkState(Unit nUnit) : base(nUnit.gameObject)
    {
        unit = nUnit;
    }

    public override Type Tick()
    {
        Debug.Log("work state");
        //if (unit.target != null)
        //{
        //    return typeof(MoveState);
        //}
        return null;
    }
}