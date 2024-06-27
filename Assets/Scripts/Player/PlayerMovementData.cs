using UnityEngine;

public struct PlayerMovementData
{
    public Vector3 Position;
    public Vector3 Direction;
    public float Speed;
    public float CycleDuration;

    public PlayerMovementData(Vector3 Position, Vector3 Direction, float Speed, float CycleDuration)
    {
        this.Position = Position;
        this.Direction = Direction;
        this.Speed = Speed;
        this.CycleDuration = CycleDuration;
    }
}
