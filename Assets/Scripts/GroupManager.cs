using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

using UnityEngine.Rendering.Universal;
using Unity.Mathematics;


// GroupManager is a class that manages the group of players, 
// it receives the data from the PlayerManagers of each player 
// computes parameters for the group and then sends back modifications 
// for each player so that they can adjust their movement accordingly and receive proper feedback based on other player's actions.
public class GroupManager : MonoBehaviour
{

    public delegate void UIUpdated();
    public static event UIUpdated OnUIUpdated;

    [Header("Input Data")]
    [SerializeField] PlayerController m_LocalPlayerConroller;
    [SerializeField] NetworkPlayerInfo m_PlayerOneInfo;
    [SerializeField] NetworkPlayerInfo m_PlayerTwoInfo;
    [SerializeField] PlayerFeedbackManager m_PlayerFeedbackManager;

    [Header("Constants To Tune")]
    [SerializeField] float modifyspeedminimumthreshold = 0.1f;

    private PlayerMovementData PlayerOneData, PlayerTwoData;

    [Header("Tuning Parameters")]
    [SerializeField] float SafeSeparationZone = 30;
    [SerializeField] float MaxSeparationZone = 150;
    [SerializeField] float DirectionSeperationCosAngle = 0.7f;


    [Header("Debugging Information")]
    public float separationDistance;
    public float deltaSpeed;
    public float averageCycleDuration;
    public float DirectionOrientationAngle;
    public float footsteptime = 0f;
    public float _FootstepCurrentFreq = 0.5f;
    private bool twoplayersready = false;

    [Header("Not Sure if Needed Parameters")]
    public float averageSpeed;

    private Vector3 CurrentPosition, PrevPosition;

    private float DistanceCovered = 0;
    public float DistanceToCover = 10f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerOneData = new PlayerMovementData( new Vector3(0,0,0), new Vector3(0, 0, 0), 0, 2);
        PlayerTwoData = new PlayerMovementData(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0, 2);
        separationDistance = 0;
        deltaSpeed = 0;
        averageSpeed = 0;
        averageCycleDuration = 2;
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
            m_PlayerOneInfo = GameManager.LocalPlayerObject.GetComponent<NetworkPlayerInfo>();
            m_PlayerTwoInfo = GameManager.RemotePlayerObject.GetComponent<NetworkPlayerInfo>();
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
            PlayerOneData = m_PlayerOneInfo.GetData();
            PlayerTwoData = m_PlayerTwoInfo.GetData();
            UpdateGroupParameters();
            ProvideFeedback();
            FootstepsAudio();
        }
    }
    // Update is called once per frame

    private void FootstepsAudio()
    {
        if (PlayerTwoData.Speed > 30)
        {
            if(m_LocalPlayerConroller.RhythmEnabled)
            {
                footsteptime += Time.deltaTime;
                if (footsteptime >= _FootstepCurrentFreq)
                {
                m_PlayerFeedbackManager.PlayRemoteFootstepSound();
                footsteptime = 0;
                _FootstepCurrentFreq = averageCycleDuration;
                }
            }
            else
            {
                CurrentPosition = PlayerTwoData.Position;
                DistanceCovered += Vector3.Distance(CurrentPosition, PrevPosition);
                Debug.Log("Distance Covered: " + DistanceCovered);
                if (DistanceCovered > DistanceToCover)
                {
                    m_PlayerFeedbackManager.PlayRemoteFootstepSound();
                    DistanceCovered = 0;
                }
                PrevPosition = CurrentPosition;
            }

        }
    }
    private void UpdateGroupParameters()
    {
        separationDistance = SeparationDistance2D(PlayerOneData.Position,PlayerTwoData.Position);
        deltaSpeed = PlayerOneData.Speed - PlayerTwoData.Speed;

        averageCycleDuration = ComputeAverageFrequency(PlayerOneData.CycleDuration, PlayerTwoData.CycleDuration);
        DirectionOrientationAngle = Vector3.Angle(PlayerOneData.Direction , PlayerTwoData.Direction);

        //not sure if needed
        averageSpeed = (PlayerOneData.Speed + PlayerTwoData.Speed)/2;
    }

    private float ComputeAverageFrequency(float Frequency1, float Frequency2)
    {
        return (Frequency1 + Frequency2) / 2;
    }

    private float SeparationDistance2D(Vector3 FirstPosition, Vector3 SecondPosition)
    {
        FirstPosition = new Vector3(FirstPosition.x, 0, FirstPosition.z);
        SecondPosition = new Vector3(SecondPosition.x, 0, SecondPosition.z);
        return Vector3.Distance(FirstPosition, SecondPosition);
    }


    private void ProvideFeedback()
    {
        //AURA
        SeparationDistanceThresholding(separationDistance);
        //Rhythm Broadcasting
        //UpdateStepFrequency();
    }

    private void SeparationDistanceThresholding(float distance)
    {
        m_PlayerFeedbackManager.Aura(distance);
    }

    public UIData PassUIData()
    {
        UIData data = new UIData();
        data.PlayerOneCycle = PlayerOneData.CycleDuration;
        data.PlayerOneSpeed = PlayerOneData.Speed;
        data.PlayerTwoCycle = PlayerTwoData.CycleDuration;
        data.PlayerTwoSpeed = PlayerTwoData.Speed;
        data.SeparationDistance = separationDistance;
        return data;
    }

    public void ButtonUpdatedValues(bool AuraState,bool RhythmState, float SpeedAmpValue, float BrightnessValue)
    {
        if(m_PlayerOneInfo == null || m_PlayerTwoInfo == null)
        {
            return;
        }
        m_PlayerOneInfo.RPC_Update_AuraState(AuraState);
        m_PlayerTwoInfo.RPC_Update_AuraState(AuraState);

        m_PlayerOneInfo.RPC_Update_RhythmState(RhythmState);
        m_PlayerTwoInfo.RPC_Update_RhythmState(RhythmState);

        m_PlayerOneInfo.RPC_Update_Amplifier(SpeedAmpValue);
        m_PlayerTwoInfo.RPC_Update_Amplifier(SpeedAmpValue);

        m_PlayerOneInfo.RPC_Update_Brightness(BrightnessValue);
        m_PlayerTwoInfo.RPC_Update_Brightness(BrightnessValue);
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
}
