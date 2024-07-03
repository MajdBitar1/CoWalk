using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPXR.Recorder.Models;

public class PlayerController : MonoBehaviour
{
    //State Machine Variables
    public StateMachine stateMachine;
    private IdleState _idlestate;
    private WalkingState _walkingstate;

    //Player Movement Variables
    [Header("Player Object References")]
    [SerializeField] Armswing m_armswing;
    [SerializeField] ComputeArmRhythm m_armrhythm;
    [SerializeField] PlayerFeedbackManager playerFeedbackManager;
    [SerializeField] GameObject LeftHand, RightHand;

    [SerializeField] private float _MaxSwingMagnitudeAllowed = 10f;
    public float PlayerCycleDuration=1f;
    public float PlayerAverageSpeed = 0f;

    private NetworkPlayerInfo m_NetworkPlayerInfo;
    private CharacterController m_CharacterController;
    private Vector3 m_currentdirection;
    public float PlayerSpeed;
    private PlayerMovementData m_PlayerMoveData = new PlayerMovementData();
    private List<float> SpeedBuffer = new List<float>();


    [Header("STATE VARIABLES")]
    //State Dependent Variables
    public bool isMoving;
    public bool TouchWalkingEnabled = false;
    public bool RightLocked = false;
    public bool LeftLocked = false;
    public bool RhythmEnabled = false;

    [Header("CHEATS")]
    private bool _Experimenter = false;


    //
    void OnEnable()
    {
        AvatarNetworkManager.OnMetaAvatarSetup += SetupCC;
        GroupManager.OnUIUpdated += ChangesFromUI;
    }

    void OnDisable()
    {
        AvatarNetworkManager.OnMetaAvatarSetup -= SetupCC;
        GroupManager.OnUIUpdated -= ChangesFromUI;
    }

    private void SetupCC()
    {
        m_NetworkPlayerInfo = GetComponentInParent<NetworkPlayerInfo>();
        m_CharacterController = GetComponentInParent<CharacterController>();
        m_armswing.enabled = true;
        m_armrhythm.enabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine(this);
        _idlestate = new IdleState(this);
        _walkingstate = new WalkingState(this);
        stateMachine.Initialize(_walkingstate);
        PlayerSpeed = 0;
        m_currentdirection = Vector3.zero;
        isMoving = true;
    }

    // Update is called once per frame
    private void Update()
    {
        stateMachine.Update();
        if (m_NetworkPlayerInfo == null) return;
        m_PlayerMoveData.CycleDuration = PlayerCycleDuration;
        PlayerAverageSpeed = UpdateSpeedBuffer(PlayerSpeed);
    }
    private void FixedUpdate()
    {
        if (!GameManager.PlayersReady) return;
        CheckUpdatesfromUI();
    }
    private void LateUpdate()
    {
        if(m_NetworkPlayerInfo != null)
        {
            UpdateNetworkInfo();
        }
    }
    public void moveplayer(PlayerMovementData inputdata)
    {
        m_PlayerMoveData = inputdata;
        m_currentdirection = m_PlayerMoveData.Direction;
        PlayerSpeed = m_PlayerMoveData.Speed;
        Vector3 value = PlayerSpeed * Time.deltaTime * Vector3.ProjectOnPlane(m_currentdirection, Vector3.up);
        if (value.magnitude > _MaxSwingMagnitudeAllowed * ( 1 + m_NetworkPlayerInfo.SpeedAmplifier) )
        {
            value = value.normalized * _MaxSwingMagnitudeAllowed * ( 1 + m_NetworkPlayerInfo.SpeedAmplifier);
        }

        if (PlayerAverageSpeed > 50f)
            m_CharacterController.SimpleMove( value ); // * Runner.DeltaTime
    }
    public void SetArmswing(bool state)
    {
        m_armswing.enabled = state;
        m_armrhythm.enabled = state;
    }

    private float UpdateSpeedBuffer(float speed)
    {
        SpeedBuffer.Add(speed);
        if (SpeedBuffer.Count > 72)
        {
            SpeedBuffer.RemoveAt(0);
            float avrg = 0;
            foreach (float s in SpeedBuffer)
            {
                avrg += s;
            }
            avrg = avrg / SpeedBuffer.Count;
            return avrg;
        }
        return 0;
    }

    private void UpdateNetworkInfo()
    {
        m_NetworkPlayerInfo.RPC_Update_Speed(PlayerAverageSpeed);
        m_NetworkPlayerInfo.RPC_Update_CycleDuration(PlayerCycleDuration);
        m_NetworkPlayerInfo.RPC_Update_Direction(m_currentdirection);
    }

    public void UpdateLock()
    {
        if (LeftLocked && RightLocked)
        {
            isMoving = false;
            SetArmswing(false);
        }
        else 
        {
            isMoving = true;
            SetArmswing(true);
        }
        playerFeedbackManager.LockVisualState(LeftLocked,RightLocked);
    }

    public void ShowMenu()
    {
        if (_Experimenter)
        {
            playerFeedbackManager.ShowMenu();
        }
    }

    private void ChangesFromUI()
    {
        m_armswing.UpdateAmplifier(m_NetworkPlayerInfo.SpeedAmplifier);
        playerFeedbackManager.AuraEnabled = m_NetworkPlayerInfo.AuraState;
        playerFeedbackManager.RhythmEnabled = m_NetworkPlayerInfo.RhythmState;
        RhythmEnabled = m_NetworkPlayerInfo.RhythmState;

        playerFeedbackManager.UpdateDistanceSliders(m_NetworkPlayerInfo.SAFEDIS, m_NetworkPlayerInfo.MAXDIST, m_NetworkPlayerInfo.CSTDIST, m_NetworkPlayerInfo.ADDDIST);
    }

    private void CheckUpdatesfromUI()
    {
        m_armswing.UpdateAmplifier(m_NetworkPlayerInfo.SpeedAmplifier);
        playerFeedbackManager.AuraEnabled = m_NetworkPlayerInfo.AuraState;
        playerFeedbackManager.RhythmEnabled = m_NetworkPlayerInfo.RhythmState;
        RhythmEnabled = m_NetworkPlayerInfo.RhythmState;

        playerFeedbackManager.UpdateDistanceSliders(m_NetworkPlayerInfo.SAFEDIS, m_NetworkPlayerInfo.MAXDIST, m_NetworkPlayerInfo.CSTDIST, m_NetworkPlayerInfo.ADDDIST);
    }
    public PlayerMovementData GetPlayerMovementData()
    {
        return m_PlayerMoveData;
    }
    public GameObject GetLeftHand()
    {
        return LeftHand;
    }
    public GameObject GetRightHand()
    {
        return RightHand;
    }

    public Vector3 GetDirection()
    {
        return m_currentdirection;
    }

    public Armswing GetArmSwinger()
    {
        return m_armswing;
    }
    public PlayerFeedbackManager GetPlayerFeedbackManager()
    {
        return playerFeedbackManager;
    }
    public void SetExperimenter(bool state)
    {
        _Experimenter = state;
    }
}
