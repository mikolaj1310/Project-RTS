using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking.Types;
using UnityEngine.Rendering;


[System.Serializable] public class UnityEventVector3 : UnityEvent<Vector3> { }

public class MouseController : NetworkBehaviour
{
    //-------CLIENT
    public RectTransform unitSelectionBox;
    private Vector2 unitSelectionStartPos;
    public LayerMask clickLayer;
    public ArrayList selectedUnits { get; private set; }
    Vector3 clickPosition;
    [SerializeField] float unitFormationDistance = 2.5f;
    private BuildingController buildController;
    [SyncVar]public int clientID;

    
    //--------SERVER
    private ArrayList allUnits;
    public ArrayList[] playerUnits { get; private set; }
    
    


    // Start is called before the first frame update
    void Start()
    {
        playerUnits = new ArrayList[2];
        playerUnits[0] = new ArrayList();
        playerUnits[1] = new ArrayList();
        
        selectedUnits = new ArrayList();
        buildController = GetComponent<BuildingController>();
        unitSelectionBox = GameObject.Find("GUI").transform.Find("SelectionBox").GetComponent<RectTransform>();
    }
    
    

    // Update is called once per frame
    [Client]
    void Update()
    {
        if (isClient && isOwned)
        {
            SetClientID();
            if (!buildController.buildMode || !IsPointerOverUIObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SelectUnit();
                    unitSelectionStartPos = Input.mousePosition;
                    unitSelectionBox.sizeDelta = Vector2.zero;
                }

                if (Input.GetMouseButton(0))
                {
                    UpdateSelectionBox(Input.mousePosition);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    ReleaseSelectionBox();
                }

                if (Input.GetMouseButtonDown(1))
                {
                    ProcessRMBCommand();
                }
            }
        }
    }

    [Client]
    private void SetClientID()
    {
        GetComponent<BuildingController>().SetClientID(clientID);
    }
    
    //------------------SERVER
    [Command(requiresAuthority = false)]
    public void CmdSpawnUnit(int clientNetId, string unitPrefab, Vector3 spawnPosition, Vector3 destination)
    {
        var unit = Instantiate(Resources.Load(unitPrefab) as GameObject, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(unit, connectionToClient);
        unit.GetComponent<Unit>().CmdSetDestination(destination, UnitTargetType.UTT_Move);
        playerUnits[clientNetId - 1].Add(unit);
    }

    [Command(requiresAuthority = false)]
    private void CmdSelectUnit(int clientNetId, GameObject unit)
    {
        if (playerUnits[clientNetId - 1].Contains(unit))
        {
            RpcAddUnitToSelection(clientNetId, unit);
        }
    }

    [ClientRpc]
    private void RpcAddUnitToSelection(int clientNetId, GameObject unit)
    {
        if (!isLocalPlayer) return;
        if (clientID == clientNetId)
        {
            selectedUnits.Add(unit);
            unit.transform.GetComponent<Unit>().HighlightUnit(true);
        }
        else
        {
            unit.transform.GetComponent<Unit>().HighlightUnit(false);
            
        }
    }

    //------------------CLIENT
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    
    [Client]
    void UpdateSelectionBox(Vector2 curMousePos)
    {
        if(!unitSelectionBox.gameObject.activeInHierarchy)
        {
            unitSelectionBox.gameObject.SetActive(true);
        }

        float boxWidth = curMousePos.x - unitSelectionStartPos.x;
        float boxHeight = curMousePos.y - unitSelectionStartPos.y;

        unitSelectionBox.sizeDelta = new Vector2(Mathf.Abs(boxWidth), Mathf.Abs(boxHeight));
        unitSelectionBox.anchoredPosition = unitSelectionStartPos + new Vector2(boxWidth / 2, boxHeight / 2);
    }

    [Client]
    void ReleaseSelectionBox()
    {
        unitSelectionBox.gameObject.SetActive(false);

        Vector2 min = unitSelectionBox.anchoredPosition - (unitSelectionBox.sizeDelta / 2);
        Vector2 max = unitSelectionBox.anchoredPosition + (unitSelectionBox.sizeDelta / 2);

        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject unit in units)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x &&
                screenPos.y > min.y && screenPos.y < max.y)
            {
                if (selectedUnits.Contains(unit) == false)
                {
                    CmdSelectUnit(clientID, unit);
                    //unit.transform.GetComponent<Unit>().HighlightUnit(true);
                    //selectedUnits.Add(unit);
                }
            }
        }
    }

    [Client]
    private void ClearUnitSelection()
    {
        foreach (GameObject unit in selectedUnits)
        {
            if (!unit)
                break;
            
            if(unit.CompareTag("Unit"))
                unit.transform.GetComponent<Unit>().HighlightUnit(false);
            if(unit.CompareTag("Building") || unit.CompareTag("Resource"))
                unit.transform.GetComponent<BuildingBase>().DeselectBuilding();
        }
        selectedUnits.Clear();
    }

    [ClientCallback]
    void SelectUnit()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if(!IsPointerOverUIObject())
                ClearUnitSelection();
        }
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000f);
        if (hits.Length > 0)
        {
            
            foreach (RaycastHit hit in hits)
            {
                if (!hit.collider) continue;
                if (hit.collider.tag == "UI") continue;
                if (hit.collider.tag == "Terrain") continue;
                if (hit.collider.gameObject.transform.tag != "Unit") continue;
                if (SelectedUnitsContainsBuilding()) continue;
                
                GameObject unit = hit.collider.gameObject;
                if (!selectedUnits.Contains(unit))
                {
                    CmdSelectUnit(clientID, unit);
                }
                else if (selectedUnits.Contains(unit) && Input.GetKey(KeyCode.LeftShift))
                {
                    unit.transform.GetComponent<Unit>().HighlightUnit(false);
                    selectedUnits.Remove(unit);
                }
                
            }
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider)
                {
                    if (hit.collider.CompareTag("Building"))
                    {
                        GameObject building = hit.collider.gameObject;
                        if (!selectedUnits.Contains(building))
                        {
                            buildController.CmdSelectBuilding(clientID, building);
                        }
                        return;
                    }
                }
            }
            
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider)
                {
                    if (hit.collider.CompareTag("Resource"))
                    {
                        GameObject building = hit.collider.gameObject;
                        if (!selectedUnits.Contains(building))
                        {
                            buildController.CmdSelectResource(building);
                        }
                        return;
                    }
                }
            }
        }
    }

    private bool SelectedUnitsContainsBuilding()
    {
        foreach (GameObject unit in selectedUnits)
        {
            if (unit.CompareTag("Building"))
                return true;
            if (unit.CompareTag("Resource"))
                return true;
        }

        return false;
    }

    [ClientCallback]
    private void ProcessRMBCommand()
    {
        //GET CLICK POSITION
        clickPosition = -Vector3.one;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, clickLayer))
        {
            clickPosition = hit.point;
        }

        RaycastHit[] buildingCheck = Physics.RaycastAll(ray, 1000f);
        GameObject targetedBuildingObject = null;
        
        foreach (var possibleBuilding in buildingCheck)
        {
            if (possibleBuilding.transform.gameObject.CompareTag("Building") || possibleBuilding.transform.gameObject.CompareTag("Resource"))
            {
                targetedBuildingObject = possibleBuilding.transform.gameObject;
                break;
            }

        }
        
        
        
        ArrayList buildingsInList = new ArrayList();
        foreach (GameObject unit in selectedUnits)
        {
            if (unit.CompareTag("Building") || unit.CompareTag("Resource"))
            {
                buildingsInList.Add(unit);
            }
        }

        switch (buildingsInList.Count)
        {
            case 0:
                if (!targetedBuildingObject)
                {
                    IssueMoveCommand();
                }
                else
                {
                    if (targetedBuildingObject.GetComponent<BuildingBase>().GetCurrentHealthPercentage() < 1f)
                    {
                        IssueRepairBuildingCommand(targetedBuildingObject);
                    }
                    else
                    {
                        if (targetedBuildingObject.GetComponent<BuildingBase>().GetBuildingType() ==
                            BuildingType.BT_LumberMill)
                            IssueWorkBuildingCommand();
                    }
                }
                break;
            default:
                foreach (GameObject building in buildingsInList)
                {
                    building.GetComponent<BuildingBase>().SetFlagPosition(clickPosition);
                }
                break;
        }
      
    }

    [ClientCallback]
    private void IssueMoveCommand()
    {
        

        if (selectedUnits.Count > 1)
        {
            Vector3[] unitPositions = new Vector3[4];
            Vector3 offset = Vector3.zero;

            float defXPos = clickPosition.x;
            float defZPos = clickPosition.z;

            float xPos = defXPos;
            float zPos = defZPos;

            if (selectedUnits.Count < 3)
            {
                xPos = (-selectedUnits.Count + (unitFormationDistance / 2)) / selectedUnits.Count;
                zPos = 0;
            }
            else if (selectedUnits.Count == 3)
            {
                xPos = (-selectedUnits.Count - (unitFormationDistance * 2)) / 2;
                zPos = -unitFormationDistance / 2;
                unitPositions[0] = new Vector3(-unitFormationDistance / 2, 0, -unitFormationDistance / 2);
                unitPositions[1] = new Vector3(unitFormationDistance / 2, 0, -unitFormationDistance / 2);
                unitPositions[2] = new Vector3(0, 0, unitFormationDistance / 2);
            }
            else if (selectedUnits.Count == 4)
            {
                xPos = (-selectedUnits.Count - (unitFormationDistance * 2)) / 2;
                zPos = -unitFormationDistance / 2;
                unitPositions[0] = new Vector3(-unitFormationDistance / 2, 0, -unitFormationDistance / 2);
                unitPositions[1] = new Vector3(unitFormationDistance / 2, 0, -unitFormationDistance / 2);
                unitPositions[2] = new Vector3(-unitFormationDistance / 2, 0, unitFormationDistance / 2);
                unitPositions[3] = new Vector3(unitFormationDistance / 2, 0, unitFormationDistance / 2);
            }
            else if (selectedUnits.Count > 4)
            {
                //int c1 = a - (a % b);
                
                xPos = ((-unitFormationDistance * 3) + unitFormationDistance) / 2;
                zPos = -selectedUnits.Count - (-selectedUnits.Count % -xPos); //- selectedUnits.Count / 
                zPos = (zPos / 2) + unitFormationDistance;
            }
            

            offset = new Vector3(xPos, 0, zPos);
            int index = 1;
            if (selectedUnits.Count == 3 || selectedUnits.Count == 4)
            {
                ((GameObject)selectedUnits[0]).transform.GetComponent<Unit>().CmdSetDestination(clickPosition + unitPositions[0], UnitTargetType.UTT_Move);
                ((GameObject)selectedUnits[1]).transform.GetComponent<Unit>().CmdSetDestination(clickPosition + unitPositions[1], UnitTargetType.UTT_Move);
                ((GameObject)selectedUnits[2]).transform.GetComponent<Unit>().CmdSetDestination(clickPosition + unitPositions[2], UnitTargetType.UTT_Move);
                if (selectedUnits.Count == 4)
                {
                    ((GameObject)selectedUnits[3]).transform.GetComponent<Unit>().CmdSetDestination(clickPosition + unitPositions[3], UnitTargetType.UTT_Move);
                }
            }
            else
            {
                foreach (GameObject unit in selectedUnits)
                {
                    unit.transform.GetComponent<Unit>().CmdSetDestination(clickPosition + offset, UnitTargetType.UTT_Move);
                    offset.x += unitFormationDistance;
                    if (index == 3)
                    {
                        offset.z += unitFormationDistance;
                        offset.x = xPos;
                        index = 0;
                    }
                    index++;
                }
            }
        }
        else
        {
            foreach (GameObject unit in selectedUnits)
            {
                unit.transform.GetComponent<Unit>().CmdSetDestination(clickPosition, UnitTargetType.UTT_Move);
            }
        }
    }
    
    private void IssueRepairBuildingCommand(GameObject selectedBuilding)
    {
        if(selectedBuilding.GetComponent<BuildingBase>().GetClientID() == clientID)
            foreach (GameObject unit in selectedUnits)
            {
                if (unit.GetComponent<Unit>().unitType == UnitType.UT_Peasant)
                {
                    unit.transform.GetComponent<Unit>()
                        .CmdSetDestination(clickPosition, UnitTargetType.UTT_Build);
                    unit.GetComponent<Unit>().unitTargetObject = selectedBuilding;
                }
            }
    }

    [ClientCallback]
    private void IssueWorkBuildingCommand()
    {
        foreach (GameObject unit in selectedUnits)
        {
            if(unit.GetComponent<Unit>().unitType == UnitType.UT_Peasant)
                unit.transform.GetComponent<Unit>().CmdSetDestination(clickPosition, UnitTargetType.UTT_Work);
        }
    }
}
