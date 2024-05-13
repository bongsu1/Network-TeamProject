using System;
using UnityEngine;

namespace Test
{
    [Serializable]
    public class UserData
    {
        public string nickName;
        public float health;
        public Vector3 position;

        public UserData()
        {
            this.nickName = "";
            this.health = 90;
            this.position = Vector3.zero;
        }

        public UserData(string nickName, float health, Vector3 position)
        {
            this.nickName = nickName;
            this.health = health;
            this.position = position;
        }
    }
}
