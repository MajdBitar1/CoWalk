using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NetworkChangesTransmitter : MonoBehaviour
{
    [SerializeField] GroupManager m_GroupManager;
    [SerializeField] NetworkPlayerInfo m_NetworkPlayerOneInfo, m_NetworkPlayerTwoInfo;


    private void OnEnable()
    {
        GameManager.OnPlayerListUpdated += PlayersUpdated;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= PlayersUpdated;
    }

    private void PlayersUpdated()
    {
        if (GameManager.IsCameraMan)
        {
            m_NetworkPlayerOneInfo = GameManager.PlayerOne.GetComponent<NetworkPlayerInfo>();
            m_NetworkPlayerTwoInfo = GameManager.PlayerTwo.GetComponent<NetworkPlayerInfo>();
        }
        else
        {
            m_NetworkPlayerOneInfo = GameManager.LocalPlayerObject.GetComponent<NetworkPlayerInfo>();
            m_NetworkPlayerTwoInfo = GameManager.RemotePlayerObject.GetComponent<NetworkPlayerInfo>();
        }
    }


    public UIData PassUIData()
    {
        UIData data = new UIData();
        data.PlayerOneCycle = m_NetworkPlayerOneInfo.CycleDuration;
        data.PlayerOneSpeed = m_NetworkPlayerOneInfo.Speed;
        data.PlayerTwoCycle = m_NetworkPlayerTwoInfo.CycleDuration;
        data.PlayerTwoSpeed = m_NetworkPlayerTwoInfo.Speed;
        data.SeparationDistance = m_GroupManager.GetSeparationDistance();
        return data;
    }

    public void ButtonUpdatedValues(bool AuraState,bool RhythmState, float SpeedAmpValue)
    {
        if(m_NetworkPlayerOneInfo == null || m_NetworkPlayerTwoInfo == null)
        {
            return;
        }
        m_NetworkPlayerOneInfo.RPC_Update_AuraState(AuraState);
        m_NetworkPlayerTwoInfo.RPC_Update_AuraState(AuraState);

        m_NetworkPlayerOneInfo.RPC_Update_RhythmState(RhythmState);
        m_NetworkPlayerTwoInfo.RPC_Update_RhythmState(RhythmState);

        m_NetworkPlayerOneInfo.RPC_Update_Amplifier(SpeedAmpValue);
        m_NetworkPlayerTwoInfo.RPC_Update_Amplifier(SpeedAmpValue);
    }
    public void UpdateDistanceSliders(float safedis,float maxdis,float cstdis, float adddis)
    {
        if(m_NetworkPlayerOneInfo == null || m_NetworkPlayerTwoInfo == null)
        {
            return;
        }
        m_NetworkPlayerOneInfo.RPC_Update_ADDDIST(adddis);
        m_NetworkPlayerTwoInfo.RPC_Update_ADDDIST(adddis); 

        m_NetworkPlayerOneInfo.RPC_Update_SAFEDIST(safedis);
        m_NetworkPlayerTwoInfo.RPC_Update_SAFEDIST(safedis);

        m_NetworkPlayerOneInfo.RPC_Update_MAXDIST(maxdis);
        m_NetworkPlayerTwoInfo.RPC_Update_MAXDIST(maxdis);

        m_NetworkPlayerOneInfo.RPC_Update_CSTDIST(cstdis);
        m_NetworkPlayerTwoInfo.RPC_Update_CSTDIST(cstdis);
    }

    public void UpdateTracing(bool state)
    {
        if(m_NetworkPlayerOneInfo == null || m_NetworkPlayerTwoInfo == null)
        {
            return;
        }
        m_NetworkPlayerOneInfo.RPC_Update_Tracing(state);
        m_NetworkPlayerTwoInfo.RPC_Update_Tracing(state); 
    }
}
