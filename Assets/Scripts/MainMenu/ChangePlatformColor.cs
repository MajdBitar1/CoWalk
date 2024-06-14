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
            PlayerOneTP.OnPlayerEnterPlatform += ChangeColor;
            PlayerOneTP.OnPlayerExitPlatform += ResetColor;
        }
        else
        {
            PlayerTwoTP.OnPlayerTwoEnterPlatform += ChangeColor;
            PlayerTwoTP.OnPlayerTwoExitPlatform += ResetColor;
        }
    }

    void OnDisable()
    {
        if(m_playerNumber == 1)
        {
        PlayerOneTP.OnPlayerEnterPlatform -= ChangeColor;
        PlayerOneTP.OnPlayerExitPlatform -= ResetColor;
        }
        else
        {
        PlayerTwoTP.OnPlayerTwoEnterPlatform -= ChangeColor;
        PlayerTwoTP.OnPlayerTwoExitPlatform -= ResetColor;
        }
    }

    // Start is called before the first frame update
    void ChangeColor()
    {
        
        m_tpManager.PlayerReadyCheck(true, m_playerNumber);
    }

    void ResetColor()
    {
        m_tpManager.PlayerReadyCheck(false, m_playerNumber);
    }
}
