using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;

public class TeleportationManager : NetworkBehaviour
{
    [Networked] 
    public bool m_playerOneReady {get; set;} = false;
    
    [Networked]
    public bool m_playerTwoReady {get; set;} = false;
    [SerializeField] private GameObject _StartingArea;

    public void PlayerReadyCheck(bool state, int playerNumber)
    {
        if (playerNumber == 1)
        {
            m_playerOneReady = state;
        }
        else if (playerNumber == 2)
        {
            m_playerTwoReady = state;
        }

        BothPlayersReadyCheck();
    }

    private void BothPlayersReadyCheck()
    {
        if (m_playerOneReady && m_playerTwoReady)
        {
            // USE A TIMER TO TELEPORT PLAYERS
            Debug.Log("Both players are ready");
            StartCoroutine(TeleportPlayers());
        }
    }

    IEnumerator TeleportPlayers()
    {
        yield return new WaitForSeconds(2.0f);
        _StartingArea.SetActive(false);
    }
}
