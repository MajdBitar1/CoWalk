using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineSampler : MonoBehaviour
{
    [SerializeField] private SplineContainer m_splineContainer;
    public int NumSplines;
    [SerializeField] private int m_splineIndex;
    [SerializeField] public float m_width;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_time;

    float3 p1, p2;
    float3 position;
    float3 tanget;
    float3 upVector;

     void Update()
    {
        NumSplines = m_splineContainer.Splines.Count;
        m_splineContainer.Evaluate(m_splineIndex, m_time, out position, out tanget, out upVector);
        float3 right = Vector3.Cross(tanget, upVector).normalized;
        p1 = position + (right * m_width);
        p2 = position - (right * m_width);
        //Debug.Log(p1);
    }

    public void SampleSplineWidth(int m_splineIndex, float t,float m_width ,out Vector3 p1, out Vector3 p2)
    {
        m_splineContainer.Evaluate(m_splineIndex, t, out position, out tanget, out upVector);
        float3 right = Vector3.Cross(tanget, upVector).normalized;
        p1 = position + (right * m_width);
        p2 = position - (right * m_width);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p1, 0.3f);
        Gizmos.DrawSphere(p2, 0.3f);
    }
}
