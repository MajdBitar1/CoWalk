using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StateMachine
{
    public IPlayerState CurrentState { get; private set; }

    public IdleState idlestate;
    public WalkingState walkingstate;

    public StateMachine(PlayerController player)
    {
        this.idlestate = new IdleState(player);
        this.walkingstate = new WalkingState(player);
    }

    public void Initialize(IPlayerState startingstate)
    {
        CurrentState = startingstate;
        startingstate.Enter();
    }

    public void TransitionTo(IPlayerState newstate)
    {
        CurrentState.Exit();
        CurrentState = newstate;
        newstate.Enter();
    }

    public void Update()
    {
        CurrentState.Update();
    } 
}
