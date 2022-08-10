using eXtensionSharp;

namespace SqlQuerySample.Sql;

public class AuthProvider
{
    private Dictionary<string, int> _auth = new Dictionary<string, int>();
    public AuthProvider()
    {
        _auth.Add("test1", 1);
        _auth.Add("test2", 3);
        _auth.Add("test3", 7);
    }

    public bool HasAuth(string name)
    {
        var exist = _auth.FirstOrDefault(m => m.Key == name);
        foreach (var v in Enum.GetValues(typeof(AuthProviderType)))
        {
            if ((exist.Value.xValue<int>() & v.xValue<int>()) <= 0)
            {
                return false;
            }
        }

        return true;
    }
}

public enum AuthProviderType
{
    READ = 1,
    WRITE = 2,
    REPLY = 4,
    UPLOAD = 8,
    DELETE = 16
}