using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeedbackManager : MonoBehaviour
{
    [Header("Player Related Objects")]
    [SerializeField] GameObject _LeftEye, _RightEye;

    [Header("Feedback Objects")]
    [SerializeField] GameObject m_AuraObj,m_StatsMenuObj,m_TuningMenuObj,m_InteractionObj;
    [SerializeField] Material m_AuraMat;
    [SerializeField] GameObject LeftGestureLock, RightGestureLock;


    [Header("Constants To Tune")]
    [SerializeField] float SafeSeparationZone = 5;
    [SerializeField] float MaxSeparationZone = 10;

    private JukeBox RemoteFeedback;
    private PlayerController m_PlayerController;
    public bool _ShowMenu = false;
    private bool ToTheRight = false;
    private bool AuraBroken = false;
    private int counter = 0;

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
        m_AuraObj.gameObject.transform.localPosition = new Vector3(0, 2, 0);
    }

    private void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
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

    public void Aura(float distance)
    {
        if (!AuraEnabled)
        {
            m_AuraObj.SetActive(false);
            return;
        }
        //Normalize Distance relative to Min and Max Separation Zones, to Get a value which is Negative While in Safe zone, between safe and max the value will be between 0 and 1
        float value = Mathf.Min( 1, (distance - SafeSeparationZone) / MaxSeparationZone ) ;
        if (value > 0 && !AuraBroken)
        {
            //if separation distance exceeds the max Zone, Aura will break
            if (value >= 1f)
            {
                AuraBroken = true;
                m_AuraObj.SetActive(false);
                return;
            }   
            float angle = 0;
            //Decide the Direction of the Aura, Right or Left, based on the closest eye to the partner object.
            float distancetoRight = Vector3.Distance(GameManager.RemotePlayerObject.transform.position, _RightEye.gameObject.transform.position);
            float distancetoLeft = Vector3.Distance(GameManager.RemotePlayerObject.transform.position, _LeftEye.gameObject.transform.position);
            if (distancetoRight < distancetoLeft)
            {
                ToTheRight = true;
            }
            else
            {
                ToTheRight = false;
            }
            
            //Initially the aura will fade in, until it reaches a certain value.
            if (value <= 0.5f)
            {
                if (ToTheRight)
                {
                    angle = 40 * value + 270;
                }
                else
                {
                    angle = -40 * value + 90;
                }
            }
            else
            {
                if (ToTheRight)
                {
                    angle = 270;
                }
                else
                {
                    angle = 90;
                }
            }
            m_AuraObj.gameObject.transform.rotation = Quaternion.Euler(0, angle , 0);

            //At the end of separation, Aura will flicker
            if (value > 0.60f)
            {
                if (counter > 0 && counter <= 40)
                {
                    //Debug.Log("[FEEDBACKMAN] Value: " + value + "Current Fade is LOW");
                    m_AuraMat.SetFloat("_Fade", 0.5f);
                }
                else if (counter > 40 && counter <= 80)
                {
                    //Debug.Log("[FEEDBACKMAN] Value: " + value + "Current Fade is HIGH");
                    m_AuraMat.SetFloat("_Fade", 1f);
                }
                else
                {
                    counter = 0;
                }
                counter++;
            }
            else
            {
                m_AuraMat.SetFloat("_Fade", Aura_Brightness * 4f);
            }

            m_AuraObj.SetActive(true);
        }
        //IF AURA IS BROKEN, REUNION WITH PARTNER ACTIVATES IT AGAIN
        if (value <= 0)
        {
            m_AuraObj.SetActive(false);
            AuraBroken = false;
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
            float angle = Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, new Vector3 (0,0,1)));
            angle = Mathf.Rad2Deg * angle;

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
