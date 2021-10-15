using System;

namespace MyAuth.OAuthPoint.Models
{
    public class LoginSession
    {
        public string Id { get; set; }

        public string ClientId { get; set; }
        public DateTime Expiry { get; set; }

        public LoginInitDetails InitDetails { get; set; }

        public string SubjectId { get; set; }
        public bool IsSubjectAuthorized { get; set; }

        public AuthorizedSubjectInfo AuthorizedSubjectInfo { get; set; }
    }
}