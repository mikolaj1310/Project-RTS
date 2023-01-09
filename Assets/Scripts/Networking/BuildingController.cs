using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using kcp2k;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;


public class BuildingController : NetworkBehaviour
{
    GameObject tempBuilding;
    private ArrayList buildings;
    private BuildingType currentlyHeldBuildingType;
    
    Vector3 clickPosition;
    public bool buildMode { get; private set; }

    public LayerMask clickLayer;

    private GameObject housePrefab;
    private GameObject lumbermillPrefab;
    private GameObject bankPrefab;
    private GameObject barracksPrefab;
    private GameObject alchemistPrefab;
    private GameObject towerPrefab;


    private MouseController mouseController;
    private UIManager uiManager;
    private bool townHallFound = false;
    
    //SERVER
    private ArrayList allBuildings;
    private ArrayList[] playerBuildings;
    public int clientID { get; private set; }
    
    [Client]
    void Start()
    {
        buildMode = false;
        housePrefab = Resources.Load("Prefabs/Buildings/P_House") as GameObject;
        lumbermillPrefab = Resources.Load("Prefabs/Buildings/P_LumberMill") as GameObject;
        bankPrefab = Resources.Load("Prefabs/Buildings/P_Bank") as GameObject;
        barracksPrefab = Resources.Load("Prefabs/Buildings/P_Barracks") as GameObject;
        alchemistPrefab = Resources.Load("Prefabs/Buildings/P_Alchemist") as GameObject;
        towerPrefab = Resources.Load("Prefabs/Buildings/P_Tower") as GameObject;
        currentlyHeldBuildingType = BuildingType.BT_NONE;

        if (isClient && isOwned)
        {
            mouseController = gameObject.transform.GetComponent<MouseController>();
            //Debug.Log(mouseController.gameObject.name);
            //clientID = (int)netId;
            //Debug.Log("Connection ID: " + clientID);
            uiManager = FindObjectOfType<UIManager>();
            uiManager.buildingController = this;
        }

        //if(isClient) 
        //clientID = (int)netId;
        playerBuildings = new ArrayList[2];
        playerBuildings[0] = new ArrayList();
        playerBuildings[1] = new ArrayList();
    }

    [ClientCallback]
    public void SetClientID(int cid)
    {
        clientID = cid;
    }

    [Client]
    private void FindOwnedTownHall()
    {
        var halls = FindObjectsOfType<BuildingTownHall>();
        foreach (var hall in halls)
        {
            if (hall.ownerID == clientID)
            {
                CmdSetTownHall(clientID, hall.gameObject);
                townHallFound = true;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetTownHall(int cID, GameObject hall)
    {
        playerBuildings[cID - 1].Add(hall.gameObject);
    }

    // Update is called once per frame
    //[Client]
    void Update()
    {
        if (isClient && isLocalPlayer)
        {
            if (!townHallFound)
            {
                FindOwnedTownHall();
            }
            if (tempBuilding)
            {
                buildMode = true;
                clickPosition = -Vector3.one;

                //clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 5f));
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000f, clickLayer))
                {
                    clickPosition = hit.point;
                    tempBuilding.transform.position = clickPosition;
                }

                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                {
                    Destroy(tempBuilding);
                    tempBuilding = null;
                    buildMode = false;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    buildMode = false;
                    var newBuilding = tempBuilding;
                    CmdSpawnBuilding(clientID, currentlyHeldBuildingType, clickPosition);
                    
                    Destroy(tempBuilding);
                    tempBuilding = null;
                    
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        CreateBuilding(currentlyHeldBuildingType);
                        buildMode = true;
                    }
                }
            }
            else
            {
                tempBuilding = null;
                buildMode = false;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSpawnBuilding(int netClientID, BuildingType buildingType, Vector3 spawnPosition)
    {
        
        var building = Instantiate(GetBuildingForServer(buildingType), spawnPosition, quaternion.identity);
        NetworkServer.Spawn(building, connectionToClient);
        playerBuildings[netClientID - 1].Add(building);
        building.GetComponent<BuildingBase>().SetClientID(netClientID);
    }

    [Command(requiresAuthority = false)]
    public void CmdSelectBuilding(int clientNetId, GameObject building)
    {
        if (playerBuildings[clientNetId - 1].Contains(building))
        {
            RpcAddBuildingToSelection(clientNetId, building);
        }
    }
    

    [ClientRpc]
    private void RpcAddBuildingToSelection(int clientNetId, GameObject building)
    {
        if (clientID == clientNetId)
        {
            mouseController.selectedUnits.Add(building);
            building.transform.GetComponent<BuildingBase>().SelectBuilding();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSelectResource(GameObject resource)
    {
        RpcAddResourceToSelection(resource);
    }
    
    [ClientRpc]
    private void RpcAddResourceToSelection(GameObject resource)
    {
        mouseController.selectedUnits.Add(resource);
        resource.transform.GetComponent<BuildingBase>().SelectBuilding();
    }
    
    private GameObject GetBuildingForServer(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.BT_House:
                return housePrefab;
            case BuildingType.BT_Barracks:
                return barracksPrefab;
            case BuildingType.BT_Alchemist:
                return alchemistPrefab;
            case BuildingType.BT_Bank:
                return bankPrefab;
            case BuildingType.BT_Tower:
                return towerPrefab;
            case BuildingType.BT_LumberMill:
                return lumbermillPrefab;
        }

        return null;
    }

    public void CreateBuilding(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.BT_House:
                tempBuilding = Instantiate(housePrefab);
                currentlyHeldBuildingType = BuildingType.BT_House;
                break;
            case BuildingType.BT_Barracks:
                tempBuilding = Instantiate(barracksPrefab);
                currentlyHeldBuildingType = BuildingType.BT_Barracks;
                break;
            case BuildingType.BT_Alchemist:
                tempBuilding = Instantiate(alchemistPrefab);
                currentlyHeldBuildingType = BuildingType.BT_Alchemist;
                break;
            case BuildingType.BT_Bank:
                tempBuilding = Instantiate(bankPrefab);
                currentlyHeldBuildingType = BuildingType.BT_Bank;
                break;
            case BuildingType.BT_Tower:
                tempBuilding = Instantiate(towerPrefab);
                currentlyHeldBuildingType = BuildingType.BT_Tower;
                break;
            case BuildingType.BT_LumberMill:
                tempBuilding = Instantiate(lumbermillPrefab);
                currentlyHeldBuildingType = BuildingType.BT_LumberMill;
                break;
        }
    }
}
