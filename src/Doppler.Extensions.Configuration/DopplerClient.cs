using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doppler.Extensions.Configuration
{
    /// <summary>
    /// Doppler Client
    /// </summary>
    public class DopplerClient
    {
        private readonly DopplerUri _dopplerUri;
        private readonly string? _project;
        private readonly string? _config;
        private readonly string? _token;
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dopplerUri"><seealso cref="DopplerInternalUri"/></param>
        /// <param name="token">Either ServiceToken or Personal Token.</param>
        /// <param name="project">A project in Doppler. Required if Personal Token is in use.</param>
        /// <param name="config">A Config in Dopple. Required if Personal Token is in use.</param>
        public DopplerClient(DopplerUri dopplerUri, string token,string? project = null, string? config = null)
        {
            _dopplerUri = dopplerUri;
            _project = project;
            _config = config;
            _token = token;
        }

        /// <summary>
        /// Download all secrets from Doppler.
        /// </summary>
        /// <returns> Secrets as Dictionary</returns>
        public async Task<Dictionary<string, string>> LoadSecretsAsync()
        {
            var basicAuthHeaderValue = Convert.ToBase64String(Encoding.Default.GetBytes(_token + ":"));


            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthHeaderValue);
            var dopplerSecret = _httpClient
                                        .GetStreamAsync(DopplerInternalUri.GetDownloadSecretUrl(_dopplerUri, _project, _config))
                                        .ConfigureAwait(false);


            return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await dopplerSecret) ?? new Dictionary<string, string>();
        }
    }
}