using System;
using UnityEngine;

[Serializable]
public class RoomData
{
    //public string roomName;
    public int health;
    public Vector3 position;

    public RoomData(string roomName)
    {
        //this.roomName = roomName;
        this.health = 90;
        this.position = Vector3.zero;
    }
}
