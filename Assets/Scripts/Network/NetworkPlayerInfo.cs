using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;   

public class NetworkPlayerInfo : NetworkBehaviour
{
    [Networked] 
    public float Speed { get; set; }

    [Networked] 
    public float CycleDuration { get; set; }

    [Networked] 
    public Vector3 Direction { get; set; }

    private PlayerMovementData m_PlayerMoveData ;

    void Start()
    {
        m_PlayerMoveData = new PlayerMovementData();
        Speed = 0;
        CycleDuration = 0f;
    }


    [Rpc(sources: RpcSources.InputAuthority , targets: RpcTargets.All)]
    public void RPC_Update_Speed(float speed)
    {
        Speed = speed;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_Update_CycleDuration(float cycleduration)
    {
        CycleDuration = cycleduration;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_Update_Direction(Vector3 direction)
    {
        Direction = direction;
    }


    public PlayerMovementData GetData()
    {
        m_PlayerMoveData.Position = transform.position;
        m_PlayerMoveData.Speed = Speed;
        m_PlayerMoveData.CycleDuration = CycleDuration;
        m_PlayerMoveData.Direction = Direction;

        return m_PlayerMoveData;
    }


}
