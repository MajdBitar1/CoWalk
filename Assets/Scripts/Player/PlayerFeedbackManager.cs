using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeedbackManager : MonoBehaviour
{
    [Header("Player Related Objects")]
    [SerializeField] GameObject _LeftEye, _RightEye;

    [Header("Feedback Objects")]
    [SerializeField] GameObject m_AuraObj,m_StatsMenuObj,m_TuningMenuObj,m_InteractionObj;
    [SerializeField] GameObject StaticRingEffect;
    [SerializeField] Material m_AuraMat;
    [SerializeField] GameObject LeftGestureLock, RightGestureLock;


    [Header("Constants To Tune")]
    [SerializeField] float SafeSeparationZone = 5;
    [SerializeField] float MaxSeparationZone = 10;
    [SerializeField] float PulsePeriodFactor = 10;

    private JukeBox RemoteFeedback;
    private PlayerController m_PlayerController;
    public bool _ShowMenu = false;
    private bool ToTheRight = false;
    private bool AuraBroken = false;
    private float counter = 0;
    private VisualEffect m_AuraEffect;

    [Header("Feedback States")]
    public bool AuraEnabled = false;
    public bool RhythmEnabled = false;
    public float Aura_Brightness = 0.25f;

    private void OnEnable()
    {
        //TimeEventManager.OnTimeEvent += UpdateStepFrequency;
        GameManager.OnPlayerListUpdated += GetRemoteFeedback;
    }
    private void OnDisable()
    {
        //TimeEventManager.OnTimeEvent -= UpdateStepFrequency;
        GameManager.OnPlayerListUpdated -= GetRemoteFeedback;
    }
    private void GetRemoteFeedback()
    {
        RemoteFeedback = GameManager.RemotePlayerObject.GetComponentInChildren<JukeBox>();
        m_AuraObj.gameObject.transform.parent = GameManager.RemotePlayerObject.gameObject.transform;
        StaticRingEffect.gameObject.transform.parent = GameManager.RemotePlayerObject.gameObject.transform;
        m_AuraObj.gameObject.transform.localPosition = new Vector3(0, 2, 0);
        StaticRingEffect.gameObject.transform.localPosition = new Vector3(0, 2, 0);
    }

    private void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
        m_AuraEffect = m_AuraObj.GetComponent<VisualEffect>();
    }


    public void PlayRemoteFootstepSound()
    {
        // if (!RhythmEnabled)
        // {
        //     return;
        // }
        if(RemoteFeedback != null)
        {
        RemoteFeedback.PlayFootstepSound();
        }
        else
        {
            Debug.Log("[FEEDBACKMAN] RemoteFeedback is null");
            GetRemoteFeedback();
        }
    }
    
    private bool ObjectInCameraView(GameObject obj)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(obj.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }
    public void Aura(float distance)
    {
        if (!AuraEnabled)
        {
            StaticRingEffect.SetActive(false);
            counter = 0;
            return;
        }
        //Normalize Distance relative to Min and Max Separation Zones, to Get a value which is Negative While in Safe zone, between safe and max the value will be between 0 and 1
        float value = Mathf.Min( 1, (distance - SafeSeparationZone) / MaxSeparationZone ) ;
        if (value > 0 && !AuraBroken)
        {
            //Check if you can see the other player
            bool inView = ObjectInCameraView(GameManager.RemotePlayerObject);
            if (inView) 
            {
                //if you can see other player, ripple effect will stop and a static ring will appear around the other player
                counter = 0;
                m_AuraEffect.Stop();
                StaticRingEffect.SetActive(true);
                return;
            }
            else
            {
                StaticRingEffect.SetActive(false);
                m_AuraObj.gameObject.transform.localScale = new Vector3( 40 + value * 10f , 0f, 40 + value * 10f);
                float pulseperiod = 1 / (value * PulsePeriodFactor);
                //if separation distance exceeds the max Zone, Aura will break
                if (value >= 1f)
                {
                    AuraBroken = true;
                    return;
                }   
                if (counter == 0)
                {
                    m_AuraEffect.Play();
                }
                else if (counter >= pulseperiod)
                {
                    m_AuraEffect.Play();
                    counter = 0;
                }
                counter += Time.deltaTime;
            }
        }
        //IF AURA IS BROKEN, REUNION WITH PARTNER ACTIVATES IT AGAIN
        if (value <= 0)
        {
            AuraBroken = false;
            StaticRingEffect.SetActive(false);
            counter = 0;
        }
    }
    public void LockVisualState(bool Left, bool Right)
    {
        LeftGestureLock.SetActive(Left);
        RightGestureLock.SetActive(Right);
    }
    public void ShowMenu()
    {
        _ShowMenu = !_ShowMenu;
        if (_ShowMenu)
        {
            m_InteractionObj.SetActive(true);
            // float angle = Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, new Vector3 (0,0,1)));
            // angle = Mathf.Rad2Deg * angle;
            m_StatsMenuObj.gameObject.transform.forward = Camera.main.transform.forward.normalized;
            m_StatsMenuObj.gameObject.transform.position = transform.position + Camera.main.transform.forward.normalized * 3  + new Vector3(0, 3f, 0);

            Vector3 myvec = Vector3.Cross(Camera.main.transform.forward.normalized, Camera.main.transform.up.normalized).normalized;
            m_TuningMenuObj.gameObject.transform.forward = myvec;
            m_TuningMenuObj.gameObject.transform.position = transform.position + myvec * 3 + new Vector3(0, 3f, 0);
            m_StatsMenuObj.SetActive(true);
            m_TuningMenuObj.SetActive(true);
        }
        else
        {
            m_StatsMenuObj.SetActive(false);
            m_TuningMenuObj.SetActive(false);
            m_InteractionObj.SetActive(false);
        
        }
    }

}
