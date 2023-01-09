using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Resource
{
    private List<GameObject> treeVariants;
    public GameObject occupyingUnit { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        resourceType = ResourceType.RT_Tree;
        treeVariants = new List<GameObject>();
        treeVariants.Add(GameObject.Find("deco_tree_A"));
        treeVariants.Add(GameObject.Find("deco_tree_B"));
        treeVariants.Add(GameObject.Find("deco_tree_C"));

        foreach (var tree in treeVariants)
        {
            tree.SetActive(false);
        }
        
        var randomTree = Random.Range(0, 2);

        switch (randomTree)
        {
            case 0:
                treeVariants[0].SetActive(true);
                break;
            case 1:
                treeVariants[1].SetActive(true);
                break;
            case 2:
                treeVariants[2].SetActive(true);
                break;
        }
    }

    public void SetOccupyingUnit(GameObject unit)
    {
        occupyingUnit = unit;
    }
}
