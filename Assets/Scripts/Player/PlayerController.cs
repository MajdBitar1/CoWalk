using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using XPXR.Recorder.Models;

public class PlayerController : MonoBehaviour
{
    //State Machine Variables
    public StateMachine stateMachine;
    private IdleState _idlestate;
    private WalkingState _walkingstate;

    //Player Movement Variables

    [SerializeField] Armswing m_armswing;
    [SerializeField] ComputeArmRhythm m_armrhythm;
    [SerializeField] GameObject LeftHand, RightHand;
    [SerializeField] PlayerFeedbackManager playerFeedbackManager;
    private NetworkPlayerInfo m_NetworkPlayerInfo;
    public CharacterController m_CharacterController;
    private JukeBox m_Juekbox;
    private Vector3 m_currentdirection;
    public float PlayerSpeed;
    public float PlayerCycleDuration=2f;

    private PlayerMovementData m_PlayerMoveData = new PlayerMovementData();

    //State Dependent Variables
    public bool isMoving;
    public bool RightLocked = false;
    public bool LeftLocked = false;
    public bool IsPointingLeft = false;
    public bool IsPointingRight = false;
    public bool ReleasedLeftPointing = false;
    public bool ReleasedRightPointing = false;


    //
    void OnEnable()
    {
        AvatarNetworkManager.OnMetaAvatarSetup += SetupCC;
    }

    void OnDisable()
    {
        AvatarNetworkManager.OnMetaAvatarSetup -= SetupCC;

        XPXRManager.Recorder.StopSession();
        StartCoroutine( EndTrackingRoutine() );
    }

    // void OnApplicationQuit()
    // {
    //     XPXRManager.Recorder.StopSession();
    //     StartCoroutine( EndTrackingRoutine() );
    // }

    IEnumerator EndTrackingRoutine()
    {
        yield return new WaitUntil(() => XPXRManager.Recorder.TransfersState() == 0);
        XPXRManager.Recorder.EndTracing();
    }

    private void SetupCC()
    {
        m_NetworkPlayerInfo = GetComponentInParent<NetworkPlayerInfo>();
        m_CharacterController = GetComponentInParent<CharacterController>();
        m_Juekbox = gameObject.transform.parent.GetComponentInChildren<JukeBox>();
        playerFeedbackManager.SetJukeBox(m_Juekbox);
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

        ///
        XPXRManager.Recorder.StartSession();
        ///
    }

    // Update is called once per frame
    public void Update()
    {
        stateMachine.Update();
        if (m_NetworkPlayerInfo == null) return;
        m_PlayerMoveData.CycleDuration = PlayerCycleDuration;
        UpdateNetworkInfo();

        //
        // SHOULD BE EVERY NBER OF FRAMES AND NOT EVERY FRAME
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"PlayerData","PlayerSpeed", new QuantitativeValue(PlayerSpeed) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"PlayerData","CycleDuration", new QuantitativeValue(PlayerCycleDuration) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.WorldPosition,"PlayerPos","Posotion", new WorldPosition(transform.position,transform.rotation) );
        //
    }

    public void moveplayer(PlayerMovementData inputdata)
    {
        m_PlayerMoveData = inputdata;
        m_currentdirection = m_PlayerMoveData.Direction;
        PlayerSpeed = m_PlayerMoveData.Speed;
        m_CharacterController.SimpleMove(PlayerSpeed * Time.deltaTime * Vector3.ProjectOnPlane(m_currentdirection, Vector3.up) ); // * Runner.DeltaTime
    }
    public void SetArmswing(bool state)
    {
        m_armswing.enabled = state;
        m_armrhythm.enabled = state;
        Debug.Log("Armswing State: " + state);
        XPXRManager.Recorder.AddLogEvent("Armswing State", "is", state.ToString() ); 
    }

    private void UpdateNetworkInfo()
    {
        m_NetworkPlayerInfo.RPC_Update_Speed(PlayerSpeed);
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
        playerFeedbackManager.ShowMenu();
    }

    public void SetSpeedModifier(float speed)
    {
        m_armswing.SpeedTuning(speed);
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
    public JukeBox GetLocalJukeBox()
    {
        return m_Juekbox;
    }
    public PlayerFeedbackManager GetPlayerFeedbackManager()
    {
        return playerFeedbackManager;
    }
}
