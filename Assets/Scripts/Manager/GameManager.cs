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
    public static GameObject PlayerOne, PlayerTwo, origin;
    public int playercount = 0;
    [SerializeField] GameObject NetworkManager;

    void Awake()
    {
        Instantiate(NetworkManager);
        OVRPlugin.systemDisplayFrequency = 72.0f;
        origin = GameObject.Find("origin");
    }
    void Start()
    {
        //StartCoroutine( SettingUpRunner() );
    }
    private void LateUpdate()
    {
        playercount = PlayerRefList.Count;
    }

    public static void DefinePlayerOne(GameObject player)
    {
        PlayerOne = player;
        if (PlayerOne.GetComponentInChildren<PlayerController>() != null)
        {
            PlayerOne.GetComponentInChildren<PlayerController>().SetExperimenter(true);
        }
        DefineLocalAndRemotePlayers();
    }
    public static void DefinePlayerTwo(GameObject player)
    {
        PlayerTwo = player;
        if (PlayerTwo.GetComponentInChildren<PlayerController>() != null)
        {
            PlayerTwo.GetComponentInChildren<PlayerController>().SetExperimenter(false);
        }
        DefineLocalAndRemotePlayers();
    }

    public static void DefineLocalAndRemotePlayers()
    {
        if (PlayerOne == null || PlayerTwo == null)
        {
            Debug.LogError("[GM] Player One or Player Two not found");
            return;
        }
        else
        {
            if (PlayerOne.GetComponentInChildren<PlayerController>() != null)
            {
                LocalPlayerObject = PlayerOne;
                RemotePlayerObject = PlayerTwo;
            }
            else
            {
                LocalPlayerObject = PlayerTwo;
                RemotePlayerObject = PlayerOne;
            }
            OnPlayerListUpdated();
        }
    }

    public static void UpdatePlayerList()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerRefList = new List<GameObject>(players);
    }

    public static void CheckOberservers()
    {
        foreach (GameObject player in PlayerRefList)
        {
            if (player != LocalPlayerObject && player != RemotePlayerObject)
            {
                Debug.Log("[GM] Setting Observer");
            }
        }
    }

    IEnumerator SettingUpRunner()
    {
        Instantiate(NetworkManager);
        yield return new WaitForSeconds(1);
        //XRXP.SetActive(true);
    }

    public static GameObject GetOrigin()
    {
        return origin;
    }

    // public static void ResetPlayerList()
    // {
    //     Debug.Log("[GM] Resetting Player List");
    //     PlayerRefList.Clear();
    //     UpdatePlayerList();
    // }

    // private static void CheckPlayerList()
    // {
    //     foreach (GameObject player in PlayerRefList)
    //     {
    //         if (player.GetComponentInChildren<PlayerController>() != null)
    //             {
    //                 LocalPlayerObject = player;
    //                 //LocalPlayerObject.GetComponentInChildren<AudioSource>().volume = 0.5f;
    //             }
    //         else
    //         {
    //             RemotePlayerObject = player;
    //         }
    //     }
    // }
}
