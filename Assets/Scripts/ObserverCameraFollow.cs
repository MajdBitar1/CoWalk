using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;
using UnityEngine;

public class ObserverCameraFollow : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.OnPlayerListUpdated += UpdateTarget;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= UpdateTarget;
    }
    public GameObject Target;

    private Vector3 _TargetLookDirection;
    public Vector3 Offset;
    public float amp = 1;
    public float smoothSpeed = 0.01f;

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }
        _TargetLookDirection = Target.GetComponent<NetworkPlayerInfo>().Direction;
        Vector3 desiredPosition = Target.transform.position + _TargetLookDirection * amp + Offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(Target.transform);
    }

    private void UpdateTarget()
    {
        if (GameManager.PlayerTwo != null)
        {
            Target = GameManager.PlayerTwo;
        }
    }

}
