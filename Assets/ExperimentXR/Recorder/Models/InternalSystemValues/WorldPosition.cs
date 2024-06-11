using System;
using System.Globalization;
using UnityEngine;

namespace XPXR.Recorder.Models
{
    public class WorldPosition : Jsonable
    {
        private Vector3 Position;
        private Quaternion Rotation;
        public WorldPosition(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }
        public string GetName()
        {
            return "WorldPosition";
        }

        public string ToJSON()
        {
            return "{"+ String.Format(CultureInfo.InvariantCulture, "\"position\":[{0},{1},{2}],\"rotation\":[{3},{4},{5}]",
                    this.Position.x,
                    this.Position.y,
                    this.Position.z,
                    this.Rotation.eulerAngles.x,
                    this.Rotation.eulerAngles.y,
                    this.Rotation.eulerAngles.z
                ) + "}";
        }
    }
}