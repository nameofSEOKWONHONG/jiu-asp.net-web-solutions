namespace SqlQuerySampleApp.SampleEntity;

public class TB_USER
{
    public int ID { get; set; }
    public string NAME { get; set; }
}

public class TB_PHONE
{
    public int ID { get; set; }
    public int USER_ID { get; set; }
    public string NUMER { get; set; }
    public ENUM_PHONE_TYPE PHONE_TYPE { get; set; }

    public enum ENUM_PHONE_TYPE
    {
        MOBILE,
        TEL
    }
}

public class TB_GRADE
{
    public int ID { get; set; }
    public int USER_ID { get; set; }
    public string GRADE { get; set; }
}