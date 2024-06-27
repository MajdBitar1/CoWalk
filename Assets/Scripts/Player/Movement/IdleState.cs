using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerController player;
    public IdleState(PlayerController player)
    {
        this.player = player;
    }
    // Start is called before the first frame update
    void IPlayerState.Enter()
    {
        //player.isMoving = false;
    }

    // Update is called once per frame
    void IPlayerState.Update()
    {
        if (player.isMoving == true)
        {
            player.stateMachine.TransitionTo(player.stateMachine.walkingstate);
        }
    }

    void IPlayerState.Exit()
    {
        player.SetArmswing(true);
    }
}
