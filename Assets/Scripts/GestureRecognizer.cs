using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Fusion;
using System.ComponentModel;

public class GestureRecognizer : MonoBehaviour
{
    [Header("Provide These Objects")]
    
    [SerializeField] SplineContainer m_SplineContainer;
    [SerializeField] SplineContainer ContainerToPass;

    private GuidingArrowFactory Factory;
    private Vector3 SplinePosition, PlayerPos;
    private float3 estimationpoint;

    // Start is called before the first frame update
    void Start()
    {
        
        SplinePosition = m_SplineContainer.gameObject.transform.position;
        ContainerToPass.gameObject.transform.position = SplinePosition;
        PlayerPos = transform.position;
    }
    public void GetSplineToDrawVFX(Vector3 point,float arrowduration)
    {
        //Convert Vector3 to float3 and then convert from worldspace to spline local space (required for GetNearestPoint)
        float3 PointinSplineSpace = new float3(point.x, point.y, point.z);
        PointinSplineSpace = PointinSplineSpace - new float3(SplinePosition.x, SplinePosition.y, SplinePosition.z);

        //we get the nearest spline index and t value (starting point) at that spline
        float2 nearestsplineindex = nearestspline(PointinSplineSpace);
        

        //To specify the spline for the arrow to aniamte on, we use an empty container, we add to this container the specific spline
        //by default the empty container has an empty spline, so we remove that spline thus the spline we added is now the main spline. 
        SplineUtility.AddSpline(ContainerToPass, m_SplineContainer.Splines[(int)nearestsplineindex.x]);
        SplineUtility.RemoveSplineAt(ContainerToPass, 0);
        //Check Arrow Direction, if t value is >1 then we are at the edge of the spline, and thus we have to reverse the flow.
        //Other case is that we are NOT at the edge, and thus we evaluate the spline at t+0.01, 
        //if this new point is CLOSER to the user, means the arrow will be moving towards user, which is wrong
        //thus we need to reverse the flow!
        if (nearestsplineindex.y >= 1)
        {
            SplineUtility.ReverseFlow(ContainerToPass.Spline);
            SplineUtility.GetNearestPoint(ContainerToPass.Spline, PointinSplineSpace, out _, out nearestsplineindex.y);
        }
        else
        {
            float3 tempfloat;
            float tempt = nearestsplineindex.y + 0.01f;
            if (tempt > 1)
            {
                tempt = 1;
            }
            SplineUtility.Evaluate(m_SplineContainer.Splines[(int)nearestsplineindex.x], tempt, out tempfloat, out _, out _);
            float distance1 = math.distancesq(tempfloat, (float3)(PlayerPos - SplinePosition));
            float distance2 = math.distancesq(estimationpoint, (float3)(PlayerPos - SplinePosition));
            if (distance1 < distance2)
            {
                SplineUtility.ReverseFlow(ContainerToPass.Spline);
                SplineUtility.GetNearestPoint(ContainerToPass.Spline, PointinSplineSpace, out _, out nearestsplineindex.y);
            }
        }
        Factory.CreateArrowPrefab(point,ContainerToPass, nearestsplineindex.y, arrowduration);
    }

    private float2 nearestspline(Vector3 mypoint)
    {
        //Init values
        float2 mini = new float2(0,0);
        float minimumvalue = -1;
        int index = 0;

        if (m_SplineContainer == null)
        {
            return -1;
        }
        else
        {
            //Loop over the Splines in Container, find the distance of our point from each spline, pick closest spline,
            //return the spline index in container and the t value where the point is closest to the spline.
            foreach (var spline in m_SplineContainer.Splines)
            {
                float3 nearestpoint;
                float t;
                float value = SplineUtility.GetNearestPoint(spline, mypoint, out nearestpoint, out t);

                if (value < minimumvalue || minimumvalue == -1)
                {
                    mini.x = index;
                    mini.y = t;
                    estimationpoint = nearestpoint;
                    minimumvalue = value;
                }
                index++;
            }
            return mini;
        }
    }
    
    public void SetFactory(GuidingArrowFactory factory)
    {
        Factory = factory;
    }

}
