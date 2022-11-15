using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{
    private Unit unit;
    private Vector3 target;
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
        //direction = (Vector3) unit.target - unit.transform.position;
        //unit.transform.rotation = Quaternion.Euler(direction);
        //unit.transform.forward = direction;
        //unit.transform.Translate(translation: Vector3.forward * (Time.deltaTime * unit.movementSpeed));
        

        if (Vector3.Distance(unit.transform.position, (Vector3)unit.target) < stopDistance)
        {

            unit.PlayIdleAnimation();
            unit.SetTarget(null);
            
            return typeof(IdleState);
        }
        return null;
    }
}
