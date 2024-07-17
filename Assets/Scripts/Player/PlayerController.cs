using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public StateMachine stateMachine;
    private WalkingState _walkingstate;

    //Player Movement Variables
    [Header("Player Object References")]
    [SerializeField] Armswing m_armswing;
    [SerializeField] ComputeArmRhythm m_armrhythm;
    [SerializeField] GameObject LeftHand, RightHand;
    private float _MaxSwingMagnitudeAllowed = 10f;
    public float PlayerCycleDuration=1f;
    public float PlayerAverageSpeed = 0f;
    private NetworkPlayerInfo m_NetworkPlayerInfo;
    private CharacterController m_CharacterController;
    private Vector3 m_currentdirection;
    public float PlayerSpeed;
    private List<float> SpeedBuffer = new List<float>();


    [Header("STATE VARIABLES")]
    //State Dependent Variables
    public bool isMoving;
    public bool TouchWalkingEnabled = false;
    public bool RightLocked = false;
    public bool LeftLocked = false;

    void OnEnable()
    {
        AvatarNetworkManager.OnMetaAvatarSetup += SetupCC;
    }

    void OnDisable()
    {
        AvatarNetworkManager.OnMetaAvatarSetup -= SetupCC;
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
        _walkingstate = new WalkingState(this);
        stateMachine.Initialize(_walkingstate);
        PlayerSpeed = 0;
        m_currentdirection = Vector3.zero;
        isMoving = true;
    }
    // Update is called once per frame
    private void Update()
    {
        if (m_NetworkPlayerInfo == null) return;
        stateMachine.Update();
        PlayerAverageSpeed = UpdateSpeedBuffer(PlayerSpeed);
    }
    private void LateUpdate()
    {
        if(m_NetworkPlayerInfo == null)
        {
            return;
        }
        UpdateNetworkInfo();
    }
    public void moveplayer(PlayerMovementData inputdata)
    {
        m_currentdirection = inputdata.Direction;
        PlayerSpeed = inputdata.Speed;
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
    public GameObject GetLeftHand()
    {
        return LeftHand;
    }
    public GameObject GetRightHand()
    {
        return RightHand;
    }
    public Armswing GetArmSwinger()
    {
        return m_armswing;
    }
}
