using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType
{
    UT_Paladin,
    UT_Archer,
    UT_Peasant,
    UT_Mage
}

public enum UnitTargetType
{
    UTT_Move,
    UTT_Work,
    UTT_Build
}

public class Unit : NetworkBehaviour
{
    //public because I want to be able to get it anywhere, only set privately
    //[SyncVar] public Vector3 networkTarget;
    public UnitType unitType { get; protected set; }
    public Vector3? target { get; private set; }
    public UnitTargetType unitTargetType;
    public GameObject unitTargetObject;
    
    public StateMachine stateMachine => GetComponent<StateMachine>();
    bool unitSelected = false;
    [SerializeField] public float movementSpeed { get; private set; }
    public Animator animation { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }
    private GameObject unitselectionHightlightPrefab;
    private GameObject unitselectionHightlight;
    private UnitHighlight unitHighlight;

    private void Awake()
    {
        unitselectionHightlightPrefab = Resources.Load("Prefabs/Units/P_UnitSelectionHighlight") as GameObject;
        unitselectionHightlight = Instantiate(unitselectionHightlightPrefab, transform.position, Quaternion.identity);
        unitselectionHightlight.transform.SetParent(this.gameObject.transform);
        unitHighlight = unitselectionHightlight.GetComponent<UnitHighlight>();
        
        InitializeStateMachine();
        target = null;
        movementSpeed = 3;
        animation = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        HighlightUnit(false);
    }

    private void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(this) },
            {typeof(MoveState), new MoveState(this) },
            {typeof(WorkState), new WorkState(this) },
            {typeof(BuildState), new BuildState(this) }
        };

        GetComponent<StateMachine>().SetStates(states);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetDestination(Vector3? tar, UnitTargetType targetType)
    {
        if (tar == null) { return; }
        if(tar == Vector3.zero) { return; }
        
        unitTargetType = targetType;
        target = tar;
        if (unitTargetType == UnitTargetType.UTT_Move)
            unitTargetObject = null;
        RpcClientSetDestination(tar, targetType);
    }

    [ClientRpc]
    public void RpcClientSetDestination(Vector3? tar, UnitTargetType targetType)
    {
        unitTargetType = targetType;
        target = tar;
        if (unitTargetType == UnitTargetType.UTT_Move)
            unitTargetObject = null;
    }

    [Client]
    public void HighlightUnit(bool highlight)
    {
        
        if (highlight)
        {
            unitHighlight.ShowHighlight();
        }
        else
        {
            unitHighlight.HideHighlight();
        }
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