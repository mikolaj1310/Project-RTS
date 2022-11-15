using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[System.Serializable] public class UnityEventVector3 : UnityEvent<Vector3> { }

public class MouseController : MonoBehaviour
{
    public RectTransform unitSelectionBox;
    private Vector2 unitSelectionStartPos;
    public LayerMask clickLayer;
    public ArrayList selectedUnits;
    Vector3 clickPosition;
    [SerializeField] float unitFormationDistance = 2.5f;
    private BuildingController buildController;

    // Start is called before the first frame update
    void Start()
    {
        selectedUnits = new ArrayList();
        buildController = GetComponent<BuildingController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!buildController.buildMode )
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
                clickPosition = -Vector3.one;


                //clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 5f));
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000f, clickLayer))
                {
                    clickPosition = hit.point;

                }

                MoveUnit();

            }
        }
    }

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
                    unit.transform.GetComponent<Unit>().HighlightUnit(true);
                    selectedUnits.Add(unit);
                }
            }
        }
    }

    void SelectUnit()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            foreach(GameObject unit in selectedUnits)
            {
                unit.transform.GetComponent<Unit>().HighlightUnit(false);
            }
            selectedUnits.Clear();
        }
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000f);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider)
                {
                    if (hit.collider.tag != "UI")
                    {
                        if (hit.collider.tag != "Terrain")
                        {
                            if (hit.collider.gameObject.transform.tag == "Unit")
                            {
                                GameObject unit = hit.collider.gameObject.transform.gameObject;
                                if (!selectedUnits.Contains(unit))
                                {
                                    unit.transform.GetComponent<Unit>().HighlightUnit(true);
                                    selectedUnits.Add(unit);
                                }
                                else if (selectedUnits.Contains(unit) == true && Input.GetKey(KeyCode.LeftShift))
                                {
                                    unit.transform.GetComponent<Unit>().HighlightUnit(false);
                                    selectedUnits.Remove(unit);
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }
    }

    void MoveUnit()
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
                ((GameObject)selectedUnits[0]).transform.GetComponent<Unit>().SetTarget(clickPosition + unitPositions[0]);
                ((GameObject)selectedUnits[1]).transform.GetComponent<Unit>().SetTarget(clickPosition + unitPositions[1]);
                ((GameObject)selectedUnits[2]).transform.GetComponent<Unit>().SetTarget(clickPosition + unitPositions[2]);
                if (selectedUnits.Count == 4)
                {
                    ((GameObject)selectedUnits[3]).transform.GetComponent<Unit>().SetTarget(clickPosition + unitPositions[3]);
                }
            }
            else
            {
                foreach (GameObject unit in selectedUnits)
                {
                    unit.transform.GetComponent<Unit>().SetTarget(clickPosition + offset);
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
                unit.transform.GetComponent<Unit>().SetTarget(clickPosition);
                unit.transform.GetComponent<Unit>().SetTarget(clickPosition);
            }
        }
    }
}
