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
    [Header("Input Data")]
    [SerializeField] PlayerController m_LocalPlayerConroller;
    [SerializeField] NetworkPlayerInfo m_PlayerOneInfo;
    [SerializeField] NetworkPlayerInfo m_PlayerTwoInfo;
    [SerializeField] JukeBox LocalFeedback,RemoteFeedback;
    [SerializeField] VolumeProfile PostProcessing;
    private UnityEngine.Rendering.Universal.ChromaticAberration m_ChromaticAberration;
    private UnityEngine.Rendering.Universal.ColorAdjustments m_ColorAdjustments;
    private UnityEngine.Rendering.Universal.ColorCurves m_ColorCurves;

    [Header("Constants To Tune")]

    [SerializeField] float modifyspeedminimumthreshold = 0.1f;

    private PlayerMovementData PlayerOneData, PlayerTwoData;

    [Header("Tuning Parameters")]
    [SerializeField] float SafeSeparationZone = 30;
    [SerializeField] float MaxSeparationZone = 200;
    [SerializeField] float VirtualTouchDistance = 2;
    [SerializeField] float DirectionSeperationCosAngle = 0.7f;


    [Header("Debugging Information")]
    public float separationDistance;
    public float deltaSpeed;
    public float averageCycleDuration;
    public float DirectionOrientationAngle;
    private bool twoplayersready = false;

    [Header("Not Sure if Needed Parameters")]
    public float averageSpeed;

    // Start is called before the first frame update
    void Start()
    {
        PlayerOneData = new PlayerMovementData( new Vector3(0,0,0), new Vector3(0, 0, 0), 0,0);
        PlayerTwoData = new PlayerMovementData(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0, 0);
        separationDistance = 0;
        deltaSpeed = 0;
        averageSpeed = 0;
        averageCycleDuration = 2;
    }

    private void OnEnable()
    {
        TimeEventManager.OnTimeEvent += UpdateStepFrequency;
        GameManager.OnPlayerListUpdated += PlayersUpdated;
    }
    private void OnDisable()
    {
        TimeEventManager.OnTimeEvent -= UpdateStepFrequency;
        GameManager.OnPlayerListUpdated -= PlayersUpdated;
    }

    // Update is called once per frame
    void Update()
    {
        if (twoplayersready)
        {
            PlayerOneData = m_PlayerOneInfo.GetData();
            PlayerTwoData = m_PlayerTwoInfo.GetData();
            UpdateGroupParameters();
            ProvideFeedback();
        }
    }

    private void PlayersUpdated()
    {
        if (GameManager.PlayerRefList.Count >= 2)
        {
            m_PlayerOneInfo = GameManager.LocalPlayerObject.GetComponent<NetworkPlayerInfo>();
            m_PlayerTwoInfo = GameManager.RemotePlayerObject.GetComponent<NetworkPlayerInfo>();
            LocalFeedback = GameManager.LocalPlayerObject.GetComponent<JukeBox>();
            RemoteFeedback = GameManager.RemotePlayerObject.GetComponent<JukeBox>();
            twoplayersready = true;
        }
        else 
        {
            twoplayersready = false;
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
        if (PlayerOneData.Speed == 0) Frequency1 = 0;
        if (PlayerTwoData.Speed == 0) Frequency2 = 0;
        if (Frequency1 == 0 || Frequency2 == 0) return Frequency2 + Frequency1;

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
        DirectionOrientationThresholding(DirectionOrientationAngle);

        //Speed Assistance
        SpeedAssistance(deltaSpeed);

        //Rhythm Broadcasting
        UpdateStepFrequency();
    }

    private void SeparationDistanceThresholding(float distance)
    {
        PostProcessing.TryGet(out m_ColorAdjustments);
        PostProcessing.TryGet(out m_ChromaticAberration);
        float value = math.max( 1, (distance - SafeSeparationZone) / MaxSeparationZone ) ;
        float grayscalevalue = math.max(0.5f, (distance - SafeSeparationZone) / MaxSeparationZone);
        if (distance > 0)
        {
            // AnimationCurve curve = new AnimationCurve();
            // curve = AnimationCurve.Linear(0, 0, 1, 1);
            // TextureCurve mycurve = new TextureCurve(curve, 0f , true , new Vector2(1,1));
            // m_ColorCurves.lumVsSat.Override(mycurve);
            // m_ColorCurves.satVsSat.Override(mycurve);

            // ACESS POST PROCESSING AND APPLY CHROMATIC ABERRATION
            m_ChromaticAberration.intensity.Override(value);
            m_ColorAdjustments.hueShift.Override(distance * -1.5f);
            
        }
        else
        {
            m_ChromaticAberration.intensity.Override(0);
        }

        if (distance <= VirtualTouchDistance)
        {
            //Haptic Feedback
            //bell shaped function, peaks at 0 for a value of 1 and then is reduced quickly to become ~0.2 at +- 2
            // can replace 1 / ( 1  + x^2 ) with sech(x) for faster decay, exp(-x^2) for extremely faster decay
            //float hapticfeedback = 1 / (1 + diff * diff); 
        }
    }


    private void DirectionOrientationThresholding(float Angle)
    {
        if (Angle > DirectionSeperationCosAngle)
        {
            //Haptic Feedbacks
        }
    }

    private void SpeedAssistance(float deltaspeed)
    {
        if ( deltaspeed < 1 || deltaspeed > -1)
        {
            return;
        }
        else 
        {
            // if P1 is moving faster than P2
            if (deltaspeed > modifyspeedminimumthreshold)
            {
                //LOCAL PLAYER IS FASTER, SO WE DO NOTHING, ON THE REMOTE PLAYER SIDE, HIS SPEED WILL INCREASE
            }

            // if P2 is moving faster than P1
            else if (deltaspeed < -modifyspeedminimumthreshold)
            {
                //LOCAL PLAYER IS SLOWER, SO WE SHOULD AMPLIFY THEIR SPEED
                float value = math.max ( 1 , math.exp( deltaspeed) ) ;
                m_LocalPlayerConroller.SetSpeedModifier(value);
            }
        }
    }

    private void UpdateStepFrequency()
    {
        LocalFeedback.setcycle(PlayerOneData.CycleDuration);
        RemoteFeedback.setcycle(averageCycleDuration);
    }
}
