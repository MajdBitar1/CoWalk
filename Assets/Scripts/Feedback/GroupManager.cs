using UnityEngine;
// GroupManager is a class that manages the group of players, 
// it receives the data from the PlayerManagers of each player 
// computes parameters for the group and then sends back modifications 
// for each player so that they can adjust their movement accordingly and receive proper feedback based on other player's actions.
public class GroupManager : MonoBehaviour
{
    [Header("Input Data")]
    [SerializeField] PlayerFeedbackManager m_PlayerFeedbackManager;
    [SerializeField] FootstepsManager _FootStepsMan;
    private NetworkPlayerInfo m_NetworkPlayerOneInfo;
    private NetworkPlayerInfo m_NetworkPlayerTwoInfo;
    private PlayerMovementData PlayerOneData, PlayerTwoData;
    private float _SeparationDistance;
    private float _DeltaSpeed;
    private float _AverageCycleForBoth;
    private bool twoplayersready = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerOneData = new PlayerMovementData( new Vector3(0,0,0), new Vector3(0, 0, 0), 0, 1);
        PlayerTwoData = new PlayerMovementData(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0, 1);
        _SeparationDistance = 0;
        _DeltaSpeed = 0;
        _AverageCycleForBoth = 1;
    }

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
        if (GameManager.PlayersReady)
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
            twoplayersready = true;
        }
        else 
        {
            twoplayersready = false;
        }
    }

    void Update()
    {
        if (twoplayersready)
        {
            PlayerOneData = m_NetworkPlayerOneInfo.GetData();
            PlayerTwoData = m_NetworkPlayerTwoInfo.GetData();
            _FootStepsMan.UpdatePlayersInfo(m_NetworkPlayerOneInfo,m_NetworkPlayerTwoInfo);
            UpdateGroupParameters();
        }
    }
    // Update is called once per frame
    private void UpdateGroupParameters()
    {
        _SeparationDistance = SeparationDistance2D(PlayerOneData.Position,PlayerTwoData.Position);
        _DeltaSpeed = PlayerOneData.Speed - PlayerTwoData.Speed;
        _AverageCycleForBoth = ComputeAverageFrequency(PlayerOneData.CycleDuration, PlayerTwoData.CycleDuration);
    }

    private float ComputeAverageFrequency(float Value1, float Value2)
    {
        return (Value1 + Value2) / 2;
    }

    private float SeparationDistance2D(Vector3 FirstPosition, Vector3 SecondPosition)
    {
        FirstPosition = new Vector3(FirstPosition.x, 0, FirstPosition.z);
        SecondPosition = new Vector3(SecondPosition.x, 0, SecondPosition.z);
        return Vector3.Distance(FirstPosition, SecondPosition);
    }

    public UIData PassUIData()
    {
        UIData data = new UIData();
        data.PlayerOneCycle = PlayerOneData.CycleDuration;
        data.PlayerOneSpeed = PlayerOneData.Speed;
        data.PlayerTwoCycle = PlayerTwoData.CycleDuration;
        data.PlayerTwoSpeed = PlayerTwoData.Speed;
        data.SeparationDistance = _SeparationDistance;
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
    public PlayerMovementData GetPlayerOneData()
    {
        return PlayerOneData;
    }   
    public PlayerMovementData GetPlayerTwoData()
    {
        return PlayerTwoData;
    }
    public float GetSeparationDistance()
    {
        return _SeparationDistance;
    }
    public float GetAverageCycleForBoth()
    {
        return _AverageCycleForBoth;
    }
    public float GetDeltaSpeed()
    {
        return _DeltaSpeed;
    }
    public bool GetRhythmState()
    {
        return m_NetworkPlayerOneInfo.RhythmState;
    }
}

//Basic Footsteps based on Distance covered
// CurrentPosition = PlayerTwoData.Position;
// DistanceCovered += Vector3.Distance(CurrentPosition, PrevPosition);
// //Debug.Log("Distance Covered: " + DistanceCovered);
// if (DistanceCovered > DistanceToCover)
// {
//     m_PlayerFeedbackManager.PlayRemoteFootstepSound(_SeparationDistance);
//     DistanceCovered = 0;
// }
// PrevPosition = CurrentPosition;