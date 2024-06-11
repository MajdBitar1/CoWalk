using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class SampleAroundSpline : MonoBehaviour
{
    [SerializeField] private int resolution;
    private List<Vector3> m_vertsP1,m_vertsP2;
    [SerializeField] private SplineSampler m_splineSampler;
    [SerializeField] private MeshFilter m_meshFilter;
    private List<Vector2> uvs;
    

    // Update is called once per frame
    private void Update()
    {
        Rebuild();
    }
    private void GetVerts()
    {
        m_vertsP1 = new List<Vector3>();
        m_vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;
        Vector3 p1;
        Vector3 p2;
        for (int j =0; j < m_splineSampler.NumSplines; j++)
        { 
            for (int i = 0; i < resolution; i++)
            {
                float t = step * i;
                m_splineSampler.SampleSplineWidth(j,t,m_splineSampler.m_width,out p1, out p2);
                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
            }
            m_splineSampler.SampleSplineWidth(j,1f,m_splineSampler.m_width, out p1, out p2);
            m_vertsP1.Add(p1);
            m_vertsP2.Add(p2);
        }
    }

    private void BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<int> trisB = new List<int>();
        
        m.subMeshCount = 2;
        int offset = 0;
        float uvOffset = 0;
        int length = m_vertsP1.Count;
        uvs = new List<Vector2>(length * 4);

        for (int currentSplineIndex = 0; currentSplineIndex < m_splineSampler.NumSplines; currentSplineIndex++)
        {
            int splineOffset = resolution * currentSplineIndex;
            splineOffset += currentSplineIndex;
            for (int currentSplinePoint = 1; currentSplinePoint < resolution + 1; currentSplinePoint++)
            {
                int vertoffset = splineOffset + currentSplinePoint;
                Vector3 p1 = m_vertsP1[vertoffset - 1] - gameObject.transform.position;
                Vector3 p2 = m_vertsP2[vertoffset - 1] - gameObject.transform.position;
                Vector3 p3 = m_vertsP1[vertoffset] - gameObject.transform.position;
                Vector3 p4 = m_vertsP2[vertoffset] - gameObject.transform.position;

                offset = 4 * resolution * currentSplineIndex;
                offset += 4 * (currentSplinePoint - 1);

                int t1 = offset + 0;
                int t2 = offset + 2;
                int t3 = offset + 3;

                int t4 = offset + 3;
                int t5 = offset + 1;
                int t6 = offset + 0;

                verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
                tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });
                float distance = Vector3.Distance(p1, p3) / 4f;
                float uvDistance = uvOffset + distance;
                uvs.AddRange(new List<Vector2> { new Vector2(uvOffset, 0), new Vector2(uvOffset, 1), new Vector2(uvDistance, 0), new Vector2(uvDistance, 1) });
                uvOffset += distance;
            }
        }
        int numVerts = verts.Count;
        
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.SetTriangles(trisB, 1);
        m.SetUVs(0, uvs);
        m.RecalculateNormals();
        m_meshFilter.mesh = m;
    }
    private void Rebuild()
    {
        GetVerts();
        BuildMesh();
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        GetVerts();
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        Rebuild();
    }

    private void OnDrawGizmos()
    {
        /*
        for (int i = 0; i < m_vertsP1.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_vertsP1[i], 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_vertsP2[i], 0.3f);
        }   
        */
    }

}
