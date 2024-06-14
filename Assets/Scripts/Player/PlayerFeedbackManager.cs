using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeedbackManager : MonoBehaviour
{
    [SerializeField] GameObject m_AuraObj,MenuObj;
    [SerializeField] Material m_AuraMat;
    [SerializeField] GameObject LeftGestureLock, RightGestureLock;

    [Header("Constants To Tune")]
    [SerializeField] float SafeSeparationZone = 5;
    [SerializeField] float MaxSeparationZone = 10;


    private JukeBox RemoteFeedback;
    private PlayerController m_PlayerController;
    private JukeBox m_JukeBox;
    public bool ToTheRight = false;
    private bool AuraBroken = false;
    private int counter = 0;

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
        float value = Mathf.Min( 1, (distance - SafeSeparationZone) / MaxSeparationZone ) ;
        if (value > 0 && !AuraBroken)
        {
            // Plane myplane = new Plane( Vector3.Cross(m_PlayerController.GetDirection(),Vector3.up), transform.position ); //m_PlayerController.GetDirection()
            // ToTheRight = myplane.GetSide(GameManager.RemotePlayerObject.transform.position);
            float distancetoRight = Vector3.Distance(GameManager.RemotePlayerObject.transform.position, m_PlayerController.GetRightHand().gameObject.transform.position);
            float distancetoLeft = Vector3.Distance(GameManager.RemotePlayerObject.transform.position, m_PlayerController.GetLeftHand().gameObject.transform.position);
            float angle = Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, new Vector3 (0,0,1)));
            angle = Mathf.Rad2Deg * angle;
            Debug.Log("[FEEDBACKMAN] Angle: " + angle);
            // if ( Mathf.Abs(Vector3.Dot(Camera.current.transform.forward, new Vector3 (1,0,0))) < Mathf.Abs( Vector3.Dot(Camera.current.transform.forward, new Vector3 (0,0,1) ) )  )
            // {
            //     angle = 90;
            // }
            if (distancetoRight < distancetoLeft)
            {
                ToTheRight = true;
            }
            else
            {
                ToTheRight = false;
            }
            if (ToTheRight)
            {
                m_AuraObj.gameObject.transform.rotation = Quaternion.Euler(0, 80 + angle, 0);
            }
            else
            {
                m_AuraObj.gameObject.transform.rotation = Quaternion.Euler(0, -80 + angle, 0);
            }
            // float angle = - 40 * value + 90;
            // if (ToTheRight)
            // {
            //     angle *= -1;
            // }
            // m_AuraObj.gameObject.transform.rotation = Quaternion.Euler(0, angle, 0);
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
                Debug.Log("[FEEDBACKMAN] Value: " + value);
                if (counter < 40)
                {
                    m_AuraMat.SetColor("_Color", Color.cyan);
                    m_AuraMat.SetFloat("_Fade", 0.5f);
                }
                if (counter < 80)
                {
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
        MenuObj.gameObject.transform.forward = transform.forward;
        MenuObj.gameObject.transform.position = transform.position + transform.forward * 3 + new Vector3(0, 3f, 0);
        MenuObj.SetActive(true);
    }

    public void SetJukeBox(JukeBox jukeBox)
    {
        m_JukeBox = jukeBox;
    }
}
