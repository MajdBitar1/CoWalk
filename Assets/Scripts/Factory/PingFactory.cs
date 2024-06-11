using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PingFactory : NetworkBehaviour, IFactory
{
    [SerializeField] GameObject PingVFXPrefab;
    private bool cooldown = false;
    public Queue<NetworkObject> ObjPool = new Queue<NetworkObject>();
    
    public void CreateParticleEffect(Vector3 point, float prefabLifetime)
    {
        //Debug.Log("[PF] Create EFFECT ENTER");
        var projectile = FactoryCreate(PingVFXPrefab, point, Quaternion.identity, prefabLifetime);
        StartCoroutine(DestroyAfterLifetime(projectile, prefabLifetime));
    }
    public NetworkObject FactoryCreate(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
    {

        cooldown = true;
        if (ObjPool.Count > -1)
        {
            //Debug.Log("[PF] RUNNER SPAWN");
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
        //Debug.Log("[PF] RUNNER DESPAWN");
        Runner.Despawn(obj);
        //obj.gameObject.SetActive(false);
    }

}
