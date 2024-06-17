using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerOneTP : MonoBehaviour
{
    public delegate void PlayerEnterPlatform();
    public delegate void PlayerExitPlatform();
    public static event PlayerEnterPlatform OnPlayerEnterPlatform;
    public static event PlayerExitPlatform OnPlayerExitPlatform;
    private Collider m_collider;

    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //RAISE EVENT THAT P1 IS READY TO TELEPORT
            GameManager.DefinePlayerOne(other.gameObject);
            OnPlayerEnterPlatform();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //RAISE EVENT THAT P1 IS NOT READY ANYMORE TO TELEPORT
            OnPlayerExitPlatform();
        }
    }
}
