using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected GameObject gameObject;
    protected Transform transform;
    public abstract Type Tick();

    public BaseState(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
    }
}
