using System;
using System.Text;

namespace Doppler.Extensions.Configuration
{
    /// <summary>
    /// Represents Doppler specific Uri.
    /// </summary>
    public class DopplerUri : Uri
    {
        private DopplerUri(string uriString)
            : base(uriString)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl">Base Url of the Doppler API. <para>Default Value : <c>https://api.doppler.com/</c></para> </param>
        /// <param name="version">Version of ther Doppler API. <para> Default Value : <c>v3</c></para></param>
        /// <returns><see cref="DopplerUri"/></returns>
        public static DopplerUri Create(string baseUrl = "https://api.doppler.com/", string version = "v3")
        {
            return new DopplerUri($"{baseUrl}{version}/");
        }
    }

    internal class DopplerInternalUri : Uri
    {
        private DopplerInternalUri(Uri baseUri, string relativeUri)
            : base(baseUri, relativeUri)
        {
        }

        public static string GetDownloadSecretUrl(DopplerUri dopplerUri, string? project, string? config, string nameTransformer = "")
        {
            var baseUrl = new StringBuilder(dopplerUri.ToString());
            baseUrl.Append("configs/config/secrets/download?");
            
            if (!string.IsNullOrEmpty(project))
            {
                baseUrl.Append("project=")
                    .Append(project);
            }
            if (!string.IsNullOrEmpty(config))
            {
                baseUrl.Append("&config=")
                    .Append(config);
            }
            baseUrl.Append("&format=json");
            baseUrl.Append("&name_transformer=dotnet"); 


            return baseUrl.ToString();

        }
    }
}