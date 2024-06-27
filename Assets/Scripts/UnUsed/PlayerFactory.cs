using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class PlayerFactory : NetworkBehaviour
{
    private List<NetworkObject> m_objects;
    public NetworkObject FactoryCreatesPrefab(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
    {
        var obj = Runner.Spawn(prefab, position, rotation, Runner.LocalPlayer);
        DontDestroyOnLoad(obj);
        m_objects.Add(obj);
        return obj;
    }
}
