using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum UnitState{
    Idle,
    MovingToTarget,
    Attacking,
    Working,
    Building,
    Deploying
}


public class UnitController : MonoBehaviour
{

    private Vector3 destination;
    private Vector3 direction;
    private UnitState currentState;
    [SerializeField] private float movementSpeed;
    bool unitSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        currentState = UnitState.Idle;
        direction = Vector3.zero;
        HighlightUnit(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case UnitState.Idle:
                {
                    break;
                }
            case UnitState.MovingToTarget:
                {
                    direction = destination - transform.position;
                    transform.rotation = Quaternion.Euler(direction);
                    transform.forward = direction;
                    transform.Translate(translation: Vector3.forward * (Time.deltaTime * movementSpeed));
                    if (Vector3.Distance(transform.position, destination) < 0.1f)
                    {
                        currentState = UnitState.Idle;
                    }
                    break;
                }
        }
    }

    public void MoveToDestination(Vector3 dest)
    {
        destination = dest;
        currentState = UnitState.MovingToTarget;
    }

    public void HighlightUnit(bool highlight)
    {
        float lightIntensity;
        if (!highlight)
        {
            lightIntensity = 0;
            unitSelected = false;
        }
        else
        {
            lightIntensity = 100;
            unitSelected = true;
        }

        this.gameObject.transform.Find("UnitSelection").GetComponent<Light>().intensity = lightIntensity;
    }
}
