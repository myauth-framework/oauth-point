namespace MyAuth.OAuthPoint.Db
{
    public partial class SessionScopeDb
    {
        public bool BoolRequired
        {
            get => Required == 'Y';
            set => Required = value ? 'Y' : 'N';
        }
    }
}
