using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigStateMachine
{
    private PigBaseState currentState;

    public void Initialize(PigBaseState startState, PigController controller)
    {
        currentState = startState;
        currentState.EnterState(controller);
    }

    public void SwitchState(PigBaseState newState, PigController controller)
    {
        currentState.ExitState(controller);
        currentState = newState;
        currentState.EnterState(controller);
    }

    public void Update(PigController controller)
    {
        currentState.UpdateState(controller);
    }

    public void FixedUpdate(PigController controller)
    {
        currentState.FixedUpdateState(controller);
    }
}
