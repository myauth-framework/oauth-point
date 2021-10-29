using LinqToDB.Mapping;

namespace MyAuth.OAuthPoint.Db
{
    public enum MySqlBool
    {
        [MapValue(null)]
        [MapValue("N")]
        False,
        [MapValue("Y")]
        True
    }

    public enum LoginSessionDbStatus
    {
        [MapValue(null)]
        Pending,
        [MapValue("started")]
        Started,
        [MapValue("failed")]
        Failed,
        [MapValue("revoked")]
        Revoked
    }

    public enum TokenSessionDbStatus
    {
        [MapValue(null)]
        Pending,
        [MapValue("started")]
        Started,
        [MapValue("failed")]
        Failed,
    }
}
