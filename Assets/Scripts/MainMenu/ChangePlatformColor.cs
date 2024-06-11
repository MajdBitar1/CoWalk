using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlatformColor : MonoBehaviour
{
    [Range(1, 2)]
    public int m_playerNumber;

    private MeshRenderer m_meshRenderer;
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
    void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    void ChangeColor()
    {
        m_meshRenderer.material.color = Color.blue;
        m_tpManager.PlayerReadyCheck(true, m_playerNumber);
    }

    void ResetColor()
    {
        m_meshRenderer.material.color = Color.red;
        m_tpManager.PlayerReadyCheck(false, m_playerNumber);
    }
}
