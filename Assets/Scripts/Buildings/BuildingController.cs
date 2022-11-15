using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    GameObject tempBuilding;
    private ArrayList buildings;
    
    Vector3 clickPosition;
    public bool buildMode { get; private set; }

    public LayerMask clickLayer;
    // Start is called before the first frame update
    void Start()
    {
        buildMode = false;
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
                Debug.Log("exists");
                clickPosition = hit.point;
                tempBuilding.transform.position = clickPosition;
            }

            if (Input.GetMouseButtonDown(0))
            {
                buildMode = false;
                tempBuilding = null;
            }
        }
        else
        {
            tempBuilding = null;
            buildMode = false;
        }
        
    }
    
    public void SetCurrentlyHeldBuilding(GameObject go)
    {
        tempBuilding = Instantiate(go);
        
    }
}
