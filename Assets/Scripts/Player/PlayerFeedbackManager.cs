using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeedbackManager : MonoBehaviour
{
    [SerializeField] GameObject m_AuraObj,MenuObj;
    [SerializeField] Material m_AuraMat;
    [SerializeField] GameObject LeftGestureLock, RightGestureLock;
    private PlayerController m_PlayerController;
    private JukeBox m_JukeBox;

    private bool ToTheRight = false;
    private bool AuraBroken = false;
    private int counter = 0;

    private void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
    }
    public void PlayFootstepSound()
    {
        if(m_JukeBox != null)
        {
        m_JukeBox.PlayFootstepSound();
        }
        else
        {
            m_JukeBox = m_PlayerController.GetLocalJukeBox();
        }
    }

    public void Aura(float value)
    {
        if (value > 0 && !AuraBroken)
        {
            if (transform.InverseTransformPoint(GameManager.RemotePlayerObject.transform.position).x > 0)
            {
                ToTheRight = true;
            }
            else 
            {
                ToTheRight = false;
            }
            float angle = - 40 * value + 90;
            if (!ToTheRight)
            {
                angle *= -1;
            }
            m_AuraObj.gameObject.transform.rotation.eulerAngles.Set(0, angle, 0);


            m_AuraObj.SetActive(true);
            if (value >= 1f)
            {
                AuraBroken = true;
                m_AuraObj.SetActive(false);
            }
            if (value > 0.75f)
            {
                if (counter < 40)
                {
                    m_AuraMat.SetFloat("Alpha_Fade", 0.5f);
                }
                if (counter < 80)
                {
                    m_AuraMat.SetFloat("Alpha_Fade", 1f);
                }
                else
                {
                    counter = 0;
                }
                counter++;
            }
        }
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
