using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlatformColor : MonoBehaviour
{
    [Range(1, 2)]
    public int m_playerNumber;
    [SerializeField] TeleportationManager m_tpManager;
    void OnEnable()
    {
        if (m_playerNumber == 1)
        {
            PlayerOneTP.OnPlayerEnterPlatform += SetPlayer;
            PlayerOneTP.OnPlayerExitPlatform += ResetPlayer;
        }
        else
        {
            PlayerTwoTP.OnPlayerTwoEnterPlatform += SetPlayer;
            PlayerTwoTP.OnPlayerTwoExitPlatform += ResetPlayer;
        }
    }

    void OnDisable()
    {
        if(m_playerNumber == 1)
        {
        PlayerOneTP.OnPlayerEnterPlatform -= SetPlayer;
        PlayerOneTP.OnPlayerExitPlatform -= ResetPlayer;
        }
        else
        {
        PlayerTwoTP.OnPlayerTwoEnterPlatform -= SetPlayer;
        PlayerTwoTP.OnPlayerTwoExitPlatform -= ResetPlayer;
        }
    }

    // Start is called before the first frame update
    void SetPlayer()
    {
        m_tpManager.PlayerReadyCheck(true, m_playerNumber);
    }

    void ResetPlayer()
    {
        m_tpManager.PlayerReadyCheck(false, m_playerNumber);
    }
}
