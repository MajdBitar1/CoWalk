using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IPlayerState
{
    private PlayerController _PlayerController;
    private PlayerMovementData m_inputData = new PlayerMovementData();
    public WalkingState (PlayerController player)
    {
        this._PlayerController = player;
    }

    void IPlayerState.Enter()
    {
    }
    void IPlayerState.Update()
    {
        if (_PlayerController.isMoving == false)
        {
            _PlayerController.stateMachine.TransitionTo(_PlayerController.stateMachine.idlestate);
        }
        if (_PlayerController.TouchWalkingEnabled)
        {
            m_inputData = _PlayerController.GetArmSwinger().ComputeMovement(_PlayerController.LeftLocked,_PlayerController.RightLocked);
            _PlayerController.moveplayer(m_inputData);
        }
        else
        {
            _PlayerController.PlayerSpeed = 0;
        }
    }

    void IPlayerState.Exit()
    {
       _PlayerController.SetArmswing(false);
       m_inputData = new PlayerMovementData();
    }
}
