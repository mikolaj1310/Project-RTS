using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildState : BaseState
{
    
    private Unit unit;
    
    public BuildState(Unit nUnit) : base(nUnit.gameObject)
    {
        unit = nUnit;
    }

    public override Type Tick()
    {
        
        
        if (!unit.unitTargetObject)
        {
            unit.PlayIdleAnimation();
            return typeof(IdleState);
        }

        var targetBuilding = unit.unitTargetObject.GetComponent<BuildingBase>();
        
        if (targetBuilding.GetCurrentHealthPercentage() < 1f)
        {
            targetBuilding.IncrementBuildingHealth(5 * Time.deltaTime);
        }
        else
        {
            unit.unitTargetObject = null;
        }

        return null;
    }
}
