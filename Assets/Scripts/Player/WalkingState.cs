using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IPlayerState
{
    private PlayerController player;
    private PlayerMovementData m_inputData = new PlayerMovementData();
    public WalkingState (PlayerController player)
    {
        this.player = player;
    }

    void IPlayerState.Enter()
    {
    }
    void IPlayerState.Update()
    {
        if (player.isMoving == false)
        {
            player.stateMachine.TransitionTo(player.stateMachine.idlestate);
        }
        if (OVRInput.Get(OVRInput.Touch.Any))//(OVRInput.Get(OVRInput.Touch.PrimaryThumbRest) || OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
        {
            m_inputData = player.GetArmSwinger().ComputeMovement(player.LeftLocked,player.RightLocked);
            player.moveplayer(m_inputData);
        }
        else
        {
            if (m_inputData.Speed > 0f)
                m_inputData.Speed -= 1f;
            if (m_inputData.Speed < 0f)
                m_inputData.Speed = 0f;
        }

    }

    void IPlayerState.Exit()
    {
       player.SetArmswing(false);
       m_inputData = new PlayerMovementData();
    }
}
