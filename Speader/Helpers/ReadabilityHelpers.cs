using System;
using System.Linq;
using Windows.Security.Authentication.Web;
using Cimbalino.Toolkit.Extensions;
using ScottIsAFool.Windows.Core.Extensions;

namespace Speader.Helpers
{
    public static class ReadabilityHelpers
    {
        public static string GetVerificationToken(WebAuthenticationResult result)
        {
            var url = result.ResponseData;
            var uri = new Uri(url);
            var query = uri.QueryDictionary();
            var verification = query["oauth_verifier"];
            return verification;
        }
    }
}
