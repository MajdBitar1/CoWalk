using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PointingRayCaster))]
public class PointingTracker : MonoBehaviour
{
    [SerializeField] PlayerController m_playercontroller;
    [SerializeField] PointingRayCaster raycaster;
    private GameObject lefthand,righthand;
    private LineRenderer m_lineRenderer;

    void Start()
    {
        m_playercontroller = GetComponentInParent<PlayerController>();
        raycaster = GetComponent<PointingRayCaster>();
        lefthand = m_playercontroller.GetLeftHand();
        righthand = m_playercontroller.GetRightHand();
        m_lineRenderer = GetComponent<LineRenderer>();
    }

    public void Update()
    {   
        if (raycaster == null) return;

        if (m_playercontroller.IsPointingLeft)
        {
            // raycaster.CastRay(lefthand);
            EnableRayVisual(lefthand);
        }

        if (m_playercontroller.IsPointingRight)
        {
            // raycaster.CastRay(righthand);
            EnableRayVisual(righthand);
        }
        if(m_playercontroller.ReleasedLeftPointing)
        {
            m_lineRenderer.enabled = false;
            m_playercontroller.IsPointingLeft = false;
            raycaster.CastRay(lefthand);
            m_playercontroller.ReleasedLeftPointing = false;
        }
        if(m_playercontroller.ReleasedRightPointing)
        {
            m_lineRenderer.enabled = false;
            m_playercontroller.IsPointingRight = false;
            raycaster.CastRay(righthand);
            m_playercontroller.ReleasedRightPointing = false;
        }
    }

    private void EnableRayVisual(GameObject Source)
    {
        m_lineRenderer.enabled = true;
        m_lineRenderer.SetPositions(new Vector3[] { Source.transform.position, Source.transform.position + Source.transform.forward * 100 });
    }
}
