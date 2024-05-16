using System;
using UnityEngine;

[Serializable]
public class RoomData
{
    public int health;
    public Vector3 position;

    public RoomData()
    {
        this.health = 90;
        this.position = Vector3.zero;
    }
}
