using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeedbackManager : MonoBehaviour
{
    [Header("Feedback Objects")]
    [SerializeField] GameObject m_AuraObj,m_StatsMenuObj,m_TuningMenuObj,m_InteractionObj,LeftGestureLock, RightGestureLock;

    [Header("Constants To Tune")]
    [SerializeField] float SafeSeparationZone = 5;
    [SerializeField] float MaxSeparationZone = 10;
    [SerializeField] float StartingValue = 9;
    [SerializeField] float AdditionalValue = 1;
    private JukeBox RemoteFeedback;
    private bool AuraBroken = false;
    private VisualEffect m_AuraEffect;

    [Header("Feedback States")]
    public bool AuraEnabled = false;
    public bool RhythmEnabled = false;
    public bool AlignEnabled = false;
    public bool _ShowMenu = false;
    private bool inView = false;
    public int OneOfTheLocksIsOn = 0;

    private void OnEnable()
    {
        GameManager.OnPlayerListUpdated += GetRemoteFeedback;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= GetRemoteFeedback;
    }
    private void GetRemoteFeedback()
    {
        RemoteFeedback = GameManager.RemotePlayerObject.GetComponentInChildren<JukeBox>();
        m_AuraObj.gameObject.transform.parent = GameManager.RemotePlayerObject.gameObject.transform.FindChildRecursive("AllLOD");
        m_AuraObj.gameObject.transform.localPosition = new Vector3(0, 0.01f, 0);
    }

    private void Start()
    {
        m_AuraEffect = m_AuraObj.GetComponent<VisualEffect>();
    }

    private void Update()
    {
        inView = ObjectInCameraView(GameManager.RemotePlayerObject);
    }
    
    public void PlayRemoteFootstepSound(float value)
    {
        if(RemoteFeedback != null)
        {
            RemoteFeedback.PlayFootstepSound();
            if(AlignEnabled)
            {
                Aura(value);
            }
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
            return;
        }
        //Normalize Distance relative to Min and Max Separation Zones, to Get a value which is Negative While in Safe zone, between safe and max the value will be between 0 and 1
        float value = Mathf.Min( 1, (distance - SafeSeparationZone) / MaxSeparationZone ) ;
        if (!AuraBroken)
        {
            // Value > 0 means the separation distance is > SAFE ZONE
            if (value > 0)
            {
                //Check if you can see the other player
                if (inView) 
                {
                    //if you can see other player, ripple effect will stop
                    m_AuraEffect.Stop();
                    return;
                }
                //You can't see other player, then we have to play the ripple effect
                else
                {
                    // if Value exceeds 1 then the separatino distance > MAX distance and thus the aura breaks!
                    if (value >= 1f)
                    {
                        AuraBroken = true;
                        return;
                    }   

                    // First Scale up based on separation distance, this will ensure that the other player will see ripple effect even at high separation
                    m_AuraObj.gameObject.transform.localScale = new Vector3( StartingValue + AdditionalValue + value * MaxSeparationZone , 0f, StartingValue + AdditionalValue + value * MaxSeparationZone); 
                    //Vector3( 20 + value * 15f , 0f, 20 + value * 15f);
                    
                    //Third is to change the color of the ripples, this will be a gradient from yellow to red
                    Color ColorOnGrad = Color.Lerp( new Color(1,0.74f,0,1) , Color.red , value); // Color(1,0.647f,0,1) is Orange (255,165,0)
                    m_AuraEffect.SetVector4("Color", ColorOnGrad);

                    //Pass the period inorder to play pulses based on it
                    //This will also decide it Alignment is on, to play pulses on every step
                    m_AuraEffect.Play();
                    //PulseAlignment(pulseperiod);
                }
            }
        }
        //Value < 0 Means the separation is in SAFE ZONE, this means the ripples will be smaller and will have a white/transperant color
        //Moreover if aura was broken and players enter safe zone then the AURA will be re-activated again
        if (value <= 0)
        {
            AuraBroken = false;

            if (inView) 
            {
                //if you can see other player, ripple effect will stop
                m_AuraEffect.Stop();
                return;
            }

            m_AuraObj.gameObject.transform.localScale = new Vector3( StartingValue + value * MaxSeparationZone , 0f, StartingValue  + value * MaxSeparationZone);
            m_AuraEffect.SetVector4("Color", Color.white );
            //Independent of Steps, Aura will pulse
            m_AuraEffect.Play();
        }
    }

    public void LockVisualState(bool Left, bool Right)
    {
        LeftGestureLock.SetActive(Left);
        RightGestureLock.SetActive(Right);
        if (Left || Right)
        {
            OneOfTheLocksIsOn = 1;
        }
        else 
        {
            OneOfTheLocksIsOn = 0;
        }
    }
    public void ShowMenu()
    {
        _ShowMenu = !_ShowMenu;
        if (_ShowMenu)
        {
            m_InteractionObj.SetActive(true);
            m_StatsMenuObj.gameObject.transform.forward = Camera.main.transform.forward.normalized;
            m_StatsMenuObj.gameObject.transform.position = transform.position + Camera.main.transform.forward.normalized * 3  + new Vector3(0, 3f, 0);

            Vector3 InfrontOfPlayer = Vector3.Cross(Camera.main.transform.forward.normalized, Camera.main.transform.up.normalized).normalized;
            m_TuningMenuObj.gameObject.transform.forward = InfrontOfPlayer;
            m_TuningMenuObj.gameObject.transform.position = transform.position + InfrontOfPlayer * 3 + new Vector3(0, 3f, 0);
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

    public int AuraStatus()
    {
        if (AuraBroken)
        {
            return -1;
        }
        else if (AuraEnabled)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public int RhythmStatus()
    {
        if (RhythmEnabled)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public int InViewStatus()
    {
        if (inView)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
