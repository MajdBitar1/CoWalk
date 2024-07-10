using UnityEngine;

public class FootstepsManager : MonoBehaviour
{
    [SerializeField] PlayerFeedbackManager m_PlayerFeedbackManager;
    [SerializeField] GroupManager m_GroupManager;
    [SerializeField] GameObject AuraObjectPrefab;
    [SerializeField] GameObject RemoteAura, LocalAura; //CAN BE PRIVATE

    [Header("Constants To Tune")]
    [SerializeField] float MinimumSpeedToPlayFootsteps = 50f;
    [SerializeField] float PlayFootstepDivideByThisRatio = 2f;

    // Footsteps Feedback Time Tracking
    private float _RemoteFootstepCurrentPeriod = 1f;
    private float LocalFootstepCurrentPeriod = 1f;
    private float _RemoteFootstepTime = 0f;
    private float _LocalPlayerFootstepTime = 0;

    //Data Holders
    private NetworkPlayerInfo m_NetworkPlayerOneInfo, m_NetworkPlayerTwoInfo;

    // Audio Player
    private JukeBox RemoteFeedback;

    // Aura Values
    private float SafeSeparationZone = 5;
    private float MaxSeparationZone = 10;
    private float StartingValue = 9;
    private float AdditionalValue = 1;

    private void OnEnable()
    {
        GameManager.OnPlayerListUpdated += SetupFeedback;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= SetupFeedback;
    }

    void LateUpdate()
    {
        if ( !GameManager.PlayersReady ) return;
        if (GameManager.IsCameraMan)
        {
            CameraFeedback();
        }
        else
        {
            FootstepsFeedback(m_NetworkPlayerTwoInfo);
        }

    }
    private void FootstepsFeedback(NetworkPlayerInfo PlayerInfo)
    {
        if (PlayerInfo.Speed > MinimumSpeedToPlayFootsteps)
        {
            if(PlayerInfo.RhythmState)
            {
                // Average Rhythm Based Footstps
                _RemoteFootstepTime += Time.deltaTime;
                if (_RemoteFootstepTime >= _RemoteFootstepCurrentPeriod)
                {
                    PlayRemoteFootstepSound(RemoteAura, PlayerInfo.AuraState, m_GroupManager.GetSeparationDistance()) ;
                    _RemoteFootstepTime = 0;
                    _RemoteFootstepCurrentPeriod = m_GroupManager.GetAverageCycleForBoth() / PlayFootstepDivideByThisRatio;
                }
            }
            else
            {
                // Base Condition Rhythm Based Footstps
                _RemoteFootstepTime += Time.deltaTime;
                if (_RemoteFootstepTime >= _RemoteFootstepCurrentPeriod)
                {
                    PlayRemoteFootstepSound(RemoteAura, PlayerInfo.AuraState,  m_GroupManager.GetSeparationDistance() );
                    _RemoteFootstepTime = 0;
                    _RemoteFootstepCurrentPeriod = PlayerInfo.CycleDuration / PlayFootstepDivideByThisRatio;
                }
            }
        }
    }
    private void CameraFeedback()
    {
        if(!m_NetworkPlayerOneInfo.AuraState) return;

        if (m_NetworkPlayerOneInfo.Speed > MinimumSpeedToPlayFootsteps)
        {
            _LocalPlayerFootstepTime += Time.deltaTime;
            if (_LocalPlayerFootstepTime >= LocalFootstepCurrentPeriod)
            {
                PlayRemoteFootstepSound(LocalAura ,m_NetworkPlayerOneInfo.AuraState,  m_GroupManager.GetSeparationDistance() );
                _LocalPlayerFootstepTime = 0;
                LocalFootstepCurrentPeriod = m_NetworkPlayerOneInfo.CycleDuration / PlayFootstepDivideByThisRatio;
            }
        }

        if (m_NetworkPlayerTwoInfo.Speed > MinimumSpeedToPlayFootsteps)
        {
            _RemoteFootstepTime += Time.deltaTime;
                if (_RemoteFootstepTime >= _RemoteFootstepCurrentPeriod)
                {
                    PlayRemoteFootstepSound(RemoteAura, m_NetworkPlayerTwoInfo.AuraState, m_GroupManager.GetSeparationDistance()) ;
                    _RemoteFootstepTime = 0;
                    _RemoteFootstepCurrentPeriod = m_NetworkPlayerTwoInfo.CycleDuration / PlayFootstepDivideByThisRatio;
                }
        }

    }
    private void SetupFeedback()
    {
        if (GameManager.IsCameraMan)
        {
            RemoteFeedback = GameManager.PlayerTwo.GetComponentInChildren<JukeBox>();
            if (LocalAura == null)
                LocalAura = CreateAura(GameManager.PlayerOne.gameObject.transform.FindChildRecursive("AllLOD"));
            if (RemoteAura == null)
                RemoteAura = CreateAura(GameManager.PlayerTwo.gameObject.transform.FindChildRecursive("AllLOD"));
        }
        else 
        {
            RemoteFeedback = GameManager.RemotePlayerObject.GetComponentInChildren<JukeBox>(); 
            if (RemoteAura == null)
                RemoteAura = CreateAura(GameManager.RemotePlayerObject.gameObject.transform.FindChildRecursive("AllLOD"));
        }
    }

    private GameObject CreateAura(Transform transform)
    {
        GameObject AuraObject = Instantiate(AuraObjectPrefab,transform);
        AuraObject.gameObject.transform.parent = transform; 
        AuraObject.gameObject.transform.localPosition = new Vector3(0, 0.01f, 0);
        AuraObject.GetComponent<AuraManager>().UpdateZoneValues(SafeSeparationZone,MaxSeparationZone,StartingValue,AdditionalValue);
        return AuraObject;
    }

    public void PlayRemoteFootstepSound(GameObject AuraObject,bool AuraState,float value)
    {
        if(RemoteFeedback != null)
        {
            RemoteFeedback.PlayFootstepSound();
            if(AuraState)
            {
                m_PlayerFeedbackManager.AuraCodedState = AuraObject.GetComponent<AuraManager>().Aura(AuraState,value);
            }
        }
        else
        {
            Debug.Log("[FEEDBACKMAN] RemoteFeedback is null");
            SetupFeedback();
        }
    }
    public void UpdatePlayersInfo(NetworkPlayerInfo playerOneData, NetworkPlayerInfo playerTwoData)
    {
        m_NetworkPlayerOneInfo = playerOneData;
        m_NetworkPlayerTwoInfo = playerTwoData;
    }

    public void UpdateDistanceSliders(float Safe, float Max, float Cst, float Add)
    {
        SafeSeparationZone = Safe;
        MaxSeparationZone = Max;
        StartingValue = Cst;
        AdditionalValue = Add;
        if (RemoteAura != null)
        {
            RemoteAura.GetComponent<AuraManager>().UpdateZoneValues(Safe,Max,Cst,Add);
        }
        if (LocalAura != null)
        {
            LocalAura.GetComponent<AuraManager>().UpdateZoneValues(Safe,Max,Cst,Add);
        }
    }
}
