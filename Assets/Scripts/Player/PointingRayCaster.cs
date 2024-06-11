using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

[RequireComponent(typeof(GestureRecognizer))]
public class PointingRayCaster : MonoBehaviour
{

    [SerializeField] GuidingArrowFactory _GAFac;
    [SerializeField] PingFactory _PingFac;

    [SerializeField] HoloPingFactory _HoloPingFac;
    public float pingduration = 4f;
    public float arrowduration = 5f;
    private Vector3 m_point = Vector3.zero;
    private GestureRecognizer m_gestureRecognizer;

    

    private void Start()
    {
        m_gestureRecognizer = GetComponent<GestureRecognizer>();
        m_gestureRecognizer.SetFactory(_GAFac);
    }

    public void CastRay(GameObject HandOrigin)
    {
        Ray ray = new Ray(HandOrigin.transform.position, HandOrigin.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            m_point = hit.point;
            if (hit.transform.gameObject.layer == 7)
            {
                if (_PingFac.CheckCooldown()) return;
                Debug.Log("Hit Ground");
                _PingFac.CreateParticleEffect(m_point, pingduration);
                // if (_HoloPingFac.CheckCooldown()) return;
                // Debug.Log("Hit Ground");
                // _HoloPingFac.CreateParticleEffect(m_point, HandOrigin.transform.forward,pingduration);
            }
            if (hit.transform.gameObject.layer == 14)
            {
                if (_GAFac.CheckCooldown()) return;
                Debug.Log("Hit a Spline");
                m_gestureRecognizer.GetSplineToDrawVFX(m_point,arrowduration);
            }
        }
        //m_lineRenderer.enabled = false;
    }
        private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_point , 2f);
    }

}
