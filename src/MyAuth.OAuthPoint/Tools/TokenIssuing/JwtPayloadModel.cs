using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools.TokenIssuing
{
    class JwtPayloadModel : Dictionary<string, List<ClaimValue>>
    {

        public void Add(IDictionary<string, ClaimValue> claims)
        {
            foreach (var claim in claims)
                Add(claim.Key, claim.Value);
        }

        public void Add(string claimKey, ClaimValue claimValue)
        {
            var list = RetrieveList(claimKey);
            list.Add(claimValue);
        }

        List<ClaimValue> RetrieveList(string key)
        {
            if (!TryGetValue(key, out var list))
            {
                list = new List<ClaimValue>();
                Add(key, list);
            }

            return list;
        }

        public Dictionary<string, object> ToModelObject()
        {
            var newObj = new Dictionary<string, object>();

            foreach (var item in this)
            {
                newObj.Add(item.Key, item.Value.Count > 1
                    ? (object)item.Value.ToArray()
                    : (object)item.Value.First());
            }

            return newObj;
        }
    }
}