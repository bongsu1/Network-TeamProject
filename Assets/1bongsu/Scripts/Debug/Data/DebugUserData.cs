using System;

namespace Test
{
    [Serializable]
    public class UserData
    {
        public string nickName;
        public float health;

        public UserData()
        {
            this.nickName = "";
            this.health = 90;
        }

        public UserData(string nickName, float health)
        {
            this.nickName = nickName;
            this.health = health;
        }
    }
}
