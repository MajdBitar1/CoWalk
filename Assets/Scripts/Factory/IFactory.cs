using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public interface IFactory
{
    public NetworkObject FactoryCreate(GameObject prefab, Vector3 position, Quaternion rotation, float duration);
    public bool CheckCooldown();
    public IEnumerator DestroyAfterLifetime(NetworkObject obj, float lifetime);
}
