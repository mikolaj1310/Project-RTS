using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum BuildingType
{
    BT_LumberMill,
    BT_House,
    BT_Bank,
    BT_Barracks,
    BT_Alchemist,
    BT_Tower,
}

public class BuildingController : MonoBehaviour
{
    GameObject tempBuilding;
    private ArrayList buildings;
    
    Vector3 clickPosition;
    public bool buildMode { get; private set; }

    public LayerMask clickLayer;

    private GameObject housePrefab;
    private GameObject lumbermillPrefab;
    private GameObject bankPrefab;
    private GameObject barracksPrefab;
    private GameObject alchemistPrefab;
    private GameObject towerPrefab;
    
    void Start()
    {
        buildMode = false;
        housePrefab = Resources.Load("Prefabs/Buildings/P_House") as GameObject;
        lumbermillPrefab = Resources.Load("Prefabs/Buildings/P_LumberMill") as GameObject;
        bankPrefab = Resources.Load("Prefabs/Buildings/P_Bank") as GameObject;
        barracksPrefab = Resources.Load("Prefabs/Buildings/P_Barracks") as GameObject;
        alchemistPrefab = Resources.Load("Prefabs/Buildings/P_Alchemist") as GameObject;
        towerPrefab = Resources.Load("Prefabs/Buildings/P_Tower") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
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
                tempBuilding = null;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    tempBuilding = Instantiate(newBuilding);
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

    public void CreateBuilding(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.BT_House:
                tempBuilding = Instantiate(housePrefab);
                break;
            case BuildingType.BT_Barracks:
                tempBuilding = Instantiate(barracksPrefab);
                break;
            case BuildingType.BT_Alchemist:
                tempBuilding = Instantiate(alchemistPrefab);
                break;
            case BuildingType.BT_Bank:
                tempBuilding = Instantiate(bankPrefab);
                break;
            case BuildingType.BT_Tower:
                tempBuilding = Instantiate(towerPrefab);
                break;
            case BuildingType.BT_LumberMill:
                tempBuilding = Instantiate(lumbermillPrefab);
                break;
        }
    }
}
