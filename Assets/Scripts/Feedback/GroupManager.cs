using UnityEngine;


// GroupManager is a class that manages the group of players, 
// it receives the data from the PlayerManagers of each player 
// computes parameters for the group and then sends back modifications 
// for each player so that they can adjust their movement accordingly and receive proper feedback based on other player's actions.
public class GroupManager : MonoBehaviour
{
    public delegate void PlayersReady();
    public static event PlayersReady OnPlayersReady;
    public delegate void UIUpdated();
    public static event UIUpdated OnUIUpdated;

    [Header("Input Data")]
    [SerializeField] PlayerController m_LocalPlayerConroller;
    [SerializeField] PlayerFeedbackManager m_PlayerFeedbackManager;
    private NetworkPlayerInfo m_NetworkPlayerOneInfo;
    private NetworkPlayerInfo m_NetworkPlayerTwoInfo;
    private PlayerMovementData PlayerOneData, PlayerTwoData;

    [Header("Debugging Information")]
    public float _FootstepCurrentPeriod = 1f;
    private float _SeparationDistance;
    private float _DeltaSpeed;
    private float _AverageCycleForBoth;
    //private float DirectionOrientationAngle;
    private float footsteptime = 0f;
    private bool twoplayersready = false;
    private bool m_AlignState = false;

    [Header("Tuning Parameters")]
    [SerializeField] float DistanceToCover = 1.5f;
    //public float averageSpeed;
    private Vector3 CurrentPosition, PrevPosition;
    private float DistanceCovered = 0;


    // Start is called before the first frame update
    void Start()
    {
        PlayerOneData = new PlayerMovementData( new Vector3(0,0,0), new Vector3(0, 0, 0), 0, 1);
        PlayerTwoData = new PlayerMovementData(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0, 1);
        _SeparationDistance = 0;
        _DeltaSpeed = 0;
        _AverageCycleForBoth = 1;
        CurrentPosition = new Vector3(0, 0, 0);
        PrevPosition = new Vector3(0, 0, 0);
    }

    private void OnEnable()
    {
        //TimeEventManager.OnTimeEvent += UpdateStepFrequency;
        GameManager.OnPlayerListUpdated += PlayersUpdated;
    }
    private void OnDisable()
    {
        //TimeEventManager.OnTimeEvent -= UpdateStepFrequency;
        GameManager.OnPlayerListUpdated -= PlayersUpdated;
    }

    private void PlayersUpdated()
    {
        if (GameManager.PlayerRefList.Count >= 2)
        {
            m_NetworkPlayerOneInfo = GameManager.LocalPlayerObject.GetComponent<NetworkPlayerInfo>();
            m_NetworkPlayerTwoInfo = GameManager.RemotePlayerObject.GetComponent<NetworkPlayerInfo>();
            twoplayersready = true;
            OnPlayersReady();
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
            UpdateGroupParameters();
            FootstepsFeedback();
        }
    }
    // Update is called once per frame

    private void FootstepsFeedback()
    {
        if (PlayerTwoData.Speed > 100)
        {
            if(m_LocalPlayerConroller.RhythmEnabled)
            {
                footsteptime += Time.deltaTime;
                if (footsteptime >= _FootstepCurrentPeriod)
                {
                    m_PlayerFeedbackManager.PlayRemoteFootstepSound(_SeparationDistance);
                    footsteptime = 0;
                    _FootstepCurrentPeriod = _AverageCycleForBoth;
                }
            }
            else
            {
                CurrentPosition = PlayerTwoData.Position;
                DistanceCovered += Vector3.Distance(CurrentPosition, PrevPosition);
                //Debug.Log("Distance Covered: " + DistanceCovered);
                if (DistanceCovered > DistanceToCover)
                {
                    m_PlayerFeedbackManager.PlayRemoteFootstepSound(_SeparationDistance);
                    DistanceCovered = 0;
                }
                PrevPosition = CurrentPosition;
            }
        }
    }
    private void UpdateGroupParameters()
    {
        _SeparationDistance = SeparationDistance2D(PlayerOneData.Position,PlayerTwoData.Position);
        _DeltaSpeed = PlayerOneData.Speed - PlayerTwoData.Speed;
        _AverageCycleForBoth = ComputeAverageFrequency(PlayerOneData.CycleDuration, PlayerTwoData.CycleDuration);
        //DirectionOrientationAngle = Vector3.Angle(PlayerOneData.Direction , PlayerTwoData.Direction);

        //not sure if needed
        //averageSpeed = (PlayerOneData.Speed + PlayerTwoData.Speed)/2;
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

    public void ButtonUpdatedValues(bool AuraState,bool RhythmState, float SpeedAmpValue, float BrightnessValue, bool AlignState)
    {
        if(m_NetworkPlayerOneInfo == null || m_NetworkPlayerTwoInfo == null)
        {
            return;
        }
        m_AlignState = AlignState;  

        m_NetworkPlayerOneInfo.RPC_Update_AuraState(AuraState);
        m_NetworkPlayerTwoInfo.RPC_Update_AuraState(AuraState);

        m_NetworkPlayerOneInfo.RPC_Update_RhythmState(RhythmState);
        m_NetworkPlayerTwoInfo.RPC_Update_RhythmState(RhythmState);

        m_NetworkPlayerOneInfo.RPC_Update_Amplifier(SpeedAmpValue);
        m_NetworkPlayerTwoInfo.RPC_Update_Amplifier(SpeedAmpValue);

        m_NetworkPlayerOneInfo.RPC_Update_Brightness(BrightnessValue);
        m_NetworkPlayerTwoInfo.RPC_Update_Brightness(BrightnessValue);

        m_NetworkPlayerOneInfo.RPC_Update_AlignState(AlignState);
        m_NetworkPlayerTwoInfo.RPC_Update_AlignState(AlignState);
        OnUIUpdated();
    }
    // public void SliderUpdatedValues()
    // {
    //     if(m_PlayerOneInfo == null || m_PlayerTwoInfo == null)
    //     {
    //         return;
    //     }


    //     OnUIUpdated();
    // }
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
}
