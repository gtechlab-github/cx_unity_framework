public enum TSocialType {
    Undef,
    Google,
    Apple,
    EMail,
    Kakao,
    Facebook,

    Guest = 10
}

public class TSocialUserModel
{
    public TSocialType socialType;
    public string socialKey;
    public string nickname;
    public string email;
    public string photoURL;
}
