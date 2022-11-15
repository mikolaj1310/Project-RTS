using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private BuildingController buildingController;
    [SerializeField] GameObject factoryPrefab;
    
    private void Start()
    {
        buildingController = GameObject.Find("GameManager").GetComponent<BuildingController>();
    }

    public void BuildFactoryButton()
    {
        buildingController.SetCurrentlyHeldBuilding(factoryPrefab);
    }
}
