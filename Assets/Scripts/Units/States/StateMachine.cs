using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, BaseState> availableStates;

    public BaseState currentState { get; private set; }
    private event Action<BaseState> OnStateChanged;

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        availableStates = states;
    }

    private void Update()
    {
        if (currentState == null)
        {
            currentState = availableStates.Values.First();
        }

        var nextState = currentState?.Tick();

        if (nextState != null &&
            nextState != currentState?.GetType())
        {
            SwitchToNewState(nextState);
        }
    }

    private void SwitchToNewState(Type nextState)
    {
        currentState = availableStates[nextState];
        OnStateChanged?.Invoke(currentState);
    }
}
