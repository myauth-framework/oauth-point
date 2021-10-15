using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using MyLab.ApiClient.Test;

namespace FuncTests
{
    static class TestTools
    {
        public static void TryExtractRedirect(
            TestCallDetails resp,
            out string locationLeftPart,
            out NameValueCollection query)
        {
            if (resp.StatusCode == HttpStatusCode.Redirect)
            {
                var newLocationUrl = resp.ResponseMessage.Headers.Location;

                if (newLocationUrl != null)
                {
                    query = HttpUtility.ParseQueryString(newLocationUrl.Query);
                    locationLeftPart = newLocationUrl.GetLeftPart(UriPartial.Path);

                    return;
                }
            }

            locationLeftPart = null;
            query = new NameValueCollection();
        }
    }
}