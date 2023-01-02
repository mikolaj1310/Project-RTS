using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHighlight : MonoBehaviour
{
    
    public void ShowHighlight()
    {
        gameObject.transform.Find("eclipse").GetComponent<SpriteRenderer>().enabled = true;
    }

    public void HideHighlight()
    {
        gameObject.transform.Find("eclipse").GetComponent<SpriteRenderer>().enabled = false;
    }
}
