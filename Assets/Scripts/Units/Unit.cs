using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    //public because I want to be able to get it anywhere, only set privately
    public Vector3? target { get; private set; }
    public StateMachine stateMachine => GetComponent<StateMachine>();
    bool unitSelected = false;
    [SerializeField] public float movementSpeed { get; private set; }
    public Animator animation { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }

    private void Awake()
    {
        InitializeStateMachine();
        target = null;
        HighlightUnit(false);
        movementSpeed = 3;
        animation = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(this) },
            {typeof(MoveState), new MoveState(this) }
        };

        GetComponent<StateMachine>().SetStates(states);
    }

    public void SetTarget(Vector3? tar)
    {
        target = tar;
        /*if (tar != null)
            navMeshAgent.SetDestination((Vector3)tar);
        else
            navMeshAgent.SetDestination(Vector3.zero);*/
    }
    
    public void HighlightUnit(bool highlight)
    {
        float lightIntensity;
        if (!highlight)
        {
            lightIntensity = 0;
            unitSelected = false;
        }
        else
        {
            lightIntensity = 300000;
            unitSelected = true;
        }

        this.gameObject.transform.Find("UnitSelection").GetComponent<Light>().intensity = lightIntensity;
    }

    public virtual void PlayIdleAnimation()
    {
        animation.ResetTrigger("Walk");
        animation.SetTrigger("Idle");
    }
    
    public virtual void PlayWalkAnimation()
    {
        animation.ResetTrigger("Idle");
        animation.SetTrigger("Walk");
    }
}