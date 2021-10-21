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
}
