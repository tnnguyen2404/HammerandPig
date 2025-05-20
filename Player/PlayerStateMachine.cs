using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    private PlayerState currentState;

    public void Initialize(PlayerState initialState, PlayerController controller)
    {
        currentState = initialState;
        currentState.EnterState(controller);
    }

    public void Update(PlayerController controller)
    {
        currentState.UpdateState(controller);
    }

    public void SwitchState(PlayerState newState, PlayerController controller)
    {
        currentState.ExitState(controller);
        currentState = newState;
        currentState.EnterState(controller);
    }
}
