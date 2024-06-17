using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeedbackManager : MonoBehaviour
{
    [SerializeField] GameObject m_AuraObj,m_MenuObj,m_TuningMenuObj,m_InteractionObj;
    [SerializeField] Material m_AuraMat;
    [SerializeField] GameObject LeftGestureLock, RightGestureLock;

    [Header("Constants To Tune")]
    [SerializeField] float SafeSeparationZone = 5;
    [SerializeField] float MaxSeparationZone = 10;


    private JukeBox RemoteFeedback;
    private PlayerController m_PlayerController;
    private JukeBox m_JukeBox;
    private bool _ShowMenu = false;
    public bool ToTheRight = false;
    private bool AuraBroken = false;
    private int counter = 0;

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
        RemoteFeedback = GameManager.RemotePlayerObject.GetComponent<JukeBox>();
    }

    private void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
    }


    public void PlayRemoteFootstepSound()
    {
        if (!RhythmEnabled)
        {
            return;
        }
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
        float value = Mathf.Min( 1, (distance - SafeSeparationZone) / MaxSeparationZone ) ;
        if (value > 0 && !AuraBroken)
        {
            
            float angle = Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, new Vector3 (0,0,1)));
            angle = Mathf.Rad2Deg * angle;

            float distancetoRight = Vector3.Distance(GameManager.RemotePlayerObject.transform.position, m_PlayerController.GetRightHand().gameObject.transform.position);
            float distancetoLeft = Vector3.Distance(GameManager.RemotePlayerObject.transform.position, m_PlayerController.GetLeftHand().gameObject.transform.position);

            if (distancetoRight < distancetoLeft)
            {
                ToTheRight = true;
                m_AuraObj.gameObject.transform.rotation = Quaternion.Euler(0, 80 + angle, 0);
            }
            else
            {
                ToTheRight = false;
                m_AuraObj.gameObject.transform.rotation = Quaternion.Euler(0, -80 + angle, 0);
            }
            m_AuraMat.SetFloat("_Fade", Aura_Brightness * 4f);
            m_AuraObj.SetActive(true);

            //BREAK BUBBLE IF ABOVE MAX DISTANCE OF SEPARATION
            if (value >= 1f)
            {
                AuraBroken = true;
                m_AuraObj.SetActive(false);
            }
            //FLICKER EFFECT
            if (value > 0.60f)
            {
                if (counter < 40)
                {
                    Debug.Log("[FEEDBACKMAN] Value: " + value + "Current Fade is LOW");
                    m_AuraMat.SetColor("_Color", Color.cyan);
                    m_AuraMat.SetFloat("_Fade", 0.1f);
                }
                if (counter < 80)
                {
                    Debug.Log("[FEEDBACKMAN] Value: " + value + "Current Fade is HIGH");
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
                m_AuraMat.SetFloat("_Fade", 1f);
            }
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

            m_MenuObj.gameObject.transform.forward = Camera.main.transform.forward.normalized;
            m_MenuObj.gameObject.transform.position = transform.position + Camera.main.transform.forward.normalized * 3  + new Vector3(0, 3f, 0);

            Vector3 myvec = Vector3.Cross(Camera.main.transform.forward.normalized, Camera.main.transform.up.normalized).normalized;
            m_TuningMenuObj.gameObject.transform.forward = myvec;
            m_TuningMenuObj.gameObject.transform.position = transform.position + myvec * 3 + new Vector3(0, 3f, 0);

            m_MenuObj.SetActive(true);
            m_TuningMenuObj.SetActive(true);
        }
        else
        {
            m_MenuObj.SetActive(false);
            m_TuningMenuObj.SetActive(false);
            m_InteractionObj.SetActive(false);
        
        }
    }
    public void SetJukeBox(JukeBox jukeBox)
    {
        m_JukeBox = jukeBox;
    }
}
