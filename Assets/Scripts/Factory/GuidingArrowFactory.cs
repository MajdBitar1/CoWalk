using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class GuidingArrowFactory : NetworkBehaviour, IFactory
{
    [SerializeField] GameObject GuidingLightPrefab;
    private bool cooldown = false;
    public Queue<NetworkObject> ObjPool = new Queue<NetworkObject>();

    public void CreateArrowPrefab(Vector3 point ,SplineContainer container, float offset, float prefabLifetime)
    {
        var projectile = FactoryCreate(GuidingLightPrefab, point, Quaternion.identity, prefabLifetime);
        //Pass Values to SplineAnimate, these values will decide the spline and the starting point of the arrow.
        projectile.GetComponent<SplineAnimate>().Container = container;
        projectile.GetComponent<SplineAnimate>().StartOffset = offset;
        StartCoroutine(DestroyAfterLifetime(projectile, prefabLifetime));
    }
    
    public NetworkObject FactoryCreate(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
    {
        //Debug.Log("FACTORY: Creating prefab");
        cooldown = true;
        

        //To Enable object pooling .Count > 2 or whatever. Currently set to -1 to disable pooling.
        if (ObjPool.Count > -1)
        {
            var obj = Runner.Spawn(prefab, position, rotation, Runner.LocalPlayer);
            return obj;
        }
        else
        {
            var obj = ObjPool.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.gameObject.SetActive(true);
            return obj;
        }
    }
    public bool CheckCooldown()
    {
        return cooldown;
    }
    public IEnumerator DestroyAfterLifetime(NetworkObject obj, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        cooldown = false;
        ObjPool.Enqueue(obj); 
        // There is a bug with the arrow and pooling, where after u disable/enable arrow for 2-3 times, it freezes, I'm not yet sure if it is a bug with the pooling (my code)
        // or a bug with the spline animation OR a network bug. I will try to fix it in the future. for now destroy the arrow and create a new one.
        Runner.Despawn(obj);
        //obj.transform.position = Vector3.zero;
        // obj.transform.rotation = Quaternion.identity;
        // obj.GetComponent<SplineAnimate>().Container = null;
        // obj.GetComponent<SplineAnimate>().StartOffset = 0;
        //obj.gameObject.SetActive(false);
    }

}
