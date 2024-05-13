using System;

[Serializable]

public class UserData
{
    public string nickName;
    public bool isLogin;

    public UserData()
    {
        this.nickName = "";
        this.isLogin = false;
    }

    public UserData(string name, bool isLogin)
    {
        this.nickName = name;
        this.isLogin = isLogin;
    }
}