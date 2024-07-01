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
    [Networked]
    public float SpeedAmplifier { get; set;}
    [Networked]
    public bool AuraState { get; set; }
    [Networked]
    public bool RhythmState { get; set; }   

    [Networked]
    public float SAFEDIS { get; set;}
    [Networked]
    public float MAXDIST { get; set;}
    [Networked]
    public float CSTDIST { get; set;}
    [Networked]
    public float ADDDIST { get; set;}


    private PlayerMovementData m_PlayerMoveData ;

    void Start()
    {
        m_PlayerMoveData = new PlayerMovementData();
        Speed = 0;
        CycleDuration = 1f;
        SpeedAmplifier = 0.25f;
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

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_Update_Amplifier(float value)
    {
        SpeedAmplifier = value;
    }
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_Update_AuraState(bool value)
    {
        AuraState = value;
    }
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_Update_RhythmState(bool value)
    {
        RhythmState = value;
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_Update_SAFEDIST(float value)
    {
        SAFEDIS = value;
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_Update_MAXDIST(float value)
    {
        MAXDIST = value;
    }
        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_Update_CSTDIST(float value)
    {
        CSTDIST = value;
    }
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_Update_ADDDIST(float value)
    {
        ADDDIST = value;
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
