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

    public partial class ClientDb
    {
        public bool BoolEnabled
        {
            get => Enabled == 'Y';
            set => Enabled = value ? 'Y' : 'N';
        }

        public bool BoolDeleted
        {
            get => Deleted == 'Y';
            set => Deleted = value ? 'Y' : 'N';
        }
    }
}
