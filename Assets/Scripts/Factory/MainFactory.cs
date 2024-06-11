using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Splines;

public class MainFactory : Singleton<MainFactory>
{
    [SerializeField] GuidingArrowFactory _GAFac;
    [SerializeField] PingFactory _PingFac;
    [SerializeField] HoloPingFactory _HoloPingFac;

    public void CreatePing(Vector3 point, float prefabLifetime)
    {
        _PingFac.CreateParticleEffect(point, prefabLifetime);
    }

    public void CreateHoloPing(Vector3 point, Vector3 direction, float prefabLifetime)
    {
        _HoloPingFac.CreateParticleEffect(point, direction, prefabLifetime);
    }

    public void CreateGuidingArrow(Vector3 point ,SplineContainer container, float offset, float prefabLifetime)
    {
        _GAFac.CreateArrowPrefab(point ,container, offset, prefabLifetime);
    }
}
