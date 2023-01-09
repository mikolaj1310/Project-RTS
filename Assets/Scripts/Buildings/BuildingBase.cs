using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum BuildingType
{
    BT_LumberMill,
    BT_House,
    BT_Bank,
    BT_Barracks,
    BT_Alchemist,
    BT_Tower,
    BT_TownHall,
    BT_GoldMine,
    BT_NONE
}
public class BuildingBase : NetworkBehaviour
{
    protected bool buildingSelected = false;
    
    
    protected GameObject buildingUI;
    public MouseController mouseController;
    private GameObject flagPrefab;
    protected GameObject flagMarker;
    protected NavMeshSurface navMeshSurface;
    
    protected GameObject buildingModel;
    protected GameObject buildInProgressModel;
    protected Vector3 buildInProgressStartPosition;

    protected GameObject healthBar;

    //SERVER
    [SyncVar] protected float maxHealth;
    [SyncVar] protected float currentHealth;
    [SyncVar] public bool buildFinished = false;
    [SyncVar] protected BuildingType buildingType;

    [SyncVar] protected int clientID;
    protected List<string> productionQueue;
    protected List<float> unitProductionTime;
    protected List<float> unitProductionProgress;
    protected List<Sprite> productionQueueImages;

    protected bool updateUI = false;
    
    
    [Client]
    protected virtual void Start()
    {
        var mcs = FindObjectsOfType<MouseController>();
        foreach (var mc in mcs)
        {
            if (mc.gameObject.GetComponent<NetworkIdentity>().isOwned)
            {
                mouseController = mc;
            }
        }

        productionQueue = new List<string>();
        unitProductionTime = new List<float>();
        unitProductionProgress = new List<float>();
        productionQueueImages = new List<Sprite>();
        buildingModel = transform.Find("Model").gameObject;
        buildInProgressModel = transform.Find("BuildInProgress").gameObject;
        buildInProgressStartPosition = buildingModel.transform.position;
        flagPrefab = Resources.Load("Prefabs/Buildings/P_Flag") as GameObject;
        flagMarker = Instantiate(flagPrefab, Vector3.zero, Quaternion.identity);
        flagMarker.SetActive(false);
        buildingType = BuildingType.BT_NONE;
        healthBar = Resources.Load("Prefabs/P_HealthBar") as GameObject;
        healthBar = Instantiate(healthBar);
        healthBar.transform.position = transform.position;
        healthBar.transform.parent = transform;
        navMeshSurface = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
    }
    
    protected virtual void Update()
    {
        if (!isClient && !isOwned)
            return;

        if(buildFinished)
            healthBar.SetActive(false);
            
        
        if (!buildFinished)
        {
            healthBar.GetComponentInChildren<Slider>().value = currentHealth / maxHealth;
            if (currentHealth > maxHealth)
            {
                buildFinished = true;
                buildInProgressModel.SetActive(false);
                CmdRecalculateNavMesh();
            }
                
            buildingModel.transform.position = Vector3.Lerp(buildInProgressStartPosition,
                transform.position, currentHealth / maxHealth);
        }
    }

    [Command]
    protected void CmdRecalculateNavMesh()
    {
        RpcRecalculateNavMesh();
    }
    
    [ClientRpc]
    protected void RpcRecalculateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    
    protected Vector3 GetDestinationAroundFlagMarker()
    {
        var positionToCheck = flagMarker.transform.position;
        float checkingDistance = 0.0f;
        
        int layer = 9;
        int layerMask = 1 << layer;

        var colliders = Physics.OverlapSphere(positionToCheck, 0.5f, layerMask);
        while (colliders.Length > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                colliders = Physics.OverlapSphere(
                    positionToCheck = flagMarker.transform.position + new Vector3(
                        Random.Range(-checkingDistance, checkingDistance), 
                        0, 
                        Random.Range(-checkingDistance, checkingDistance)),
                    0.5f,
                    layerMask);
                
            }
            checkingDistance += 0.05f;
        }
        return positionToCheck;
            
    }

    [ClientCallback]
    public virtual void SetFlagPosition(Vector3 newFlagPosition)
    {
        flagMarker.transform.position = newFlagPosition;
    }
    
    [ClientCallback]
    protected virtual void BuildingUISetup()
    {
        buildingUI = GameObject.Find("GUI").transform.Find("DefaultMenu").gameObject;
    }
    
    [ClientCallback]
    public virtual void SelectBuilding()
    {
        if (!buildingUI)
        {
            BuildingUISetup();
        }
        buildingSelected = true;
        transform.Find("BuildingSelection").gameObject.SetActive(true);
        buildingUI.SetActive(true);
        flagMarker.SetActive(true);
        updateUI = true;
    }
    
    [ClientCallback]
    public virtual void DeselectBuilding()
    {
        buildingSelected = false;
        transform.Find("BuildingSelection").gameObject.SetActive(false);
        buildingUI.SetActive(false);
        flagMarker.SetActive(false);
        updateUI = false;
    }

    public BuildingType GetBuildingType() { return buildingType; }
    public void SetClientID(int cid) { clientID = cid; }

    public int GetClientID() { return clientID; }

    public float GetCurrentHealthPercentage() { return currentHealth / maxHealth; }

    public void IncrementBuildingHealth(float health) { currentHealth += health; }
}
