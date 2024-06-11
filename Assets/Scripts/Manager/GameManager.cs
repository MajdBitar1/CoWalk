using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public delegate void PlayerListUpdated();
    public static event PlayerListUpdated OnPlayerListUpdated;
    public static List<GameObject> PlayerRefList = new List<GameObject>();
    public static GameObject LocalPlayerObject, RemotePlayerObject;

    public int playercount = 0;

    void Start()
    {
        OVRPlugin.systemDisplayFrequency = 72.0f;
    }

    void FixedUpdate()
    {
        playercount = PlayerRefList.Count;
    }

    public static void UpdatePlayerList()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerRefList.Clear();
        foreach (GameObject player in players)
        {
            if (!PlayerRefList.Contains(player))
            {
                PlayerRefList.Add(player);
                CheckPlayerList();
            }
        }
        if (LocalPlayerObject == null)
        {
            Debug.LogError("[GM] Local Player Object not found");
            CheckPlayerList();
        }
        else if (RemotePlayerObject == null)
        {
            Debug.LogError("[GM] Remote Player Object not found");
            CheckPlayerList();
        }
        else
        {
            Debug.Log("[GM] LOCAL AND REMOTE PLAYER ARE AVAILABLE " + PlayerRefList.Count);
            OnPlayerListUpdated();
        }
    }

    public static void ResetPlayerList()
    {
        Debug.Log("[GM] Resetting Player List");
        PlayerRefList.Clear();
        UpdatePlayerList();
    }

    private static void CheckPlayerList()
    {
        foreach (GameObject player in PlayerRefList)
        {
            if (player.GetComponentInChildren<PlayerController>() != null)
                {
                    LocalPlayerObject = player;
                }
            else
            {
                RemotePlayerObject = player;
            }
        }
    }
}
