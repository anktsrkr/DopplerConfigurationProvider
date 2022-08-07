using Microsoft.Extensions.Configuration;

namespace Doppler.Extensions.Configuration
{

    /// <summary>
    /// Represents Doppler secrets as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class DopplerConfigurationSource: IConfigurationSource
    {
        private readonly DopplerConfigurationOptions _options;
        private readonly DopplerClient _client;

        public DopplerConfigurationSource(DopplerClient client, DopplerConfigurationOptions options)
        {
            _options = options;
            _client = client;
        }


        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder) 
            => new DopplerConfigurationProvider(_client, _options);
     
    }
}