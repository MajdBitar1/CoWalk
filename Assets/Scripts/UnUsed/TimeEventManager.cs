using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEventManager : MonoBehaviour
{
    public delegate void TimeEvent();
    public static event TimeEvent OnTimeEvent;

    public float TimeBetweenEvents = 5.0f;
    [SerializeField] private float _LocalTimer = 0;

    private void Update()
    {
        _LocalTimer += Time.deltaTime;
        if (_LocalTimer >= TimeBetweenEvents)
        {
            OnTimeEvent?.Invoke();
            _LocalTimer = 0;
        }
    }
}
