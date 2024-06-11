using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UIData
{
    public float PlayerOneCycle;
    public float PlayerTwoCycle;
    public float PlayerOneSpeed;
    public float PlayerTwoSpeed;
    public float DeltaSpeed;
    public float SeparationDistance;
    public int ASamp;

    public UIData(float cycle1, float cycle2, float speed1, float speed2, float deltaspeed, float separationdistance)
    {
        this.PlayerOneCycle = cycle1;
        this.PlayerTwoCycle = cycle2;
        this.PlayerOneSpeed = speed1;
        this.PlayerTwoSpeed = speed2;
        this.DeltaSpeed = deltaspeed;
        this.SeparationDistance = separationdistance;
        this.ASamp = 1;
    }
}