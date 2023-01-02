using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveState : BaseState
{
    private Unit unit;
    private float stopDistance;
    private Vector3 direction;
    
    public MoveState(Unit nUnit) : base(nUnit.gameObject)
    {
        unit = nUnit;
        stopDistance = 0.2f;
    }

    public override Type Tick()
    {
        unit.PlayWalkAnimation();
        unit.navMeshAgent.SetDestination((Vector3)unit.target);

        if (unit.unitTargetType == UnitTargetType.UTT_Move)
        {
            if (Vector3.Distance(unit.transform.position, (Vector3) unit.target) < stopDistance)
            {

                unit.PlayIdleAnimation();
                unit.CmdSetDestination(null, unit.unitTargetType);

                return typeof(IdleState);
                if (unit.unitTargetType == UnitTargetType.UTT_Work)
                    return typeof(WorkState);
            }
        }

        if (unit.unitTargetType == UnitTargetType.UTT_Build)
        {
            if (Vector3.Distance(unit.transform.position, (Vector3) unit.target) < 1.5f)
            {
                unit.PlayIdleAnimation();
                unit.CmdSetDestination(transform.position, unit.unitTargetType);
                unit.navMeshAgent.SetDestination((Vector3)unit.target);
                return typeof(BuildState);
            }
        }

        return null;
    }
}
