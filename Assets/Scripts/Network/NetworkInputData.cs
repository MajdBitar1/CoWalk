using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 Position;
    public Vector3 Direction;
    public float Speed;
    public float CycleDuration;
}
