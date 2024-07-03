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
    public static bool PlayersReady = false;
    [SerializeField] GameObject NetworkManager;
    [SerializeField] GameObject CameraManPrefab;
    [Header("Enable Spectator Mode")]
    public bool ExposedCameraManBool = false;
    public static bool IsCameraMan = false;
    private static GameObject _CameraMan;

    void Awake()
    {
        IsCameraMan = ExposedCameraManBool;
        origin = GameObject.Find("origin");
        if (IsCameraMan)
        {
            Destroy(origin);
            _CameraMan = Instantiate(CameraManPrefab);
            _CameraMan.transform.position = new Vector3(8.5f, 20, 0);
            _CameraMan.transform.rotation = Quaternion.Euler(45, -90, 0);
        }
        Instantiate(NetworkManager);
        OVRPlugin.systemDisplayFrequency = 72.0f;
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
            if (IsCameraMan)
            {
                _CameraMan.GetComponent<ObserverCameraFollow>().Target = PlayerTwo;
            }
        }
        DefineLocalAndRemotePlayers();
    }

    public static void DefineLocalAndRemotePlayers()
    {
        if (PlayerOne == null || PlayerTwo == null)
        {
            Debug.LogError("[GM] Player One or Player Two not found");
            PlayersReady = false;
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
            PlayersReady = true;
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

    public static GameObject GetOrigin()
    {
        return origin;
    }
}
