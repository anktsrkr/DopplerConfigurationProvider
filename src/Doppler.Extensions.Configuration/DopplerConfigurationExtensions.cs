using System;
using Microsoft.Extensions.Configuration;

namespace Doppler.Extensions.Configuration
{
    public static class DopplerConfigurationExtensions
    {
        /// <summary>
        /// Adds an <seealso cref="IConfigurationProvider"/> that reads configuration values from Doppler.
        /// </summary>
        /// <param name="configurationBuilder"> <seealso cref="IConfigurationBuilder"/></param>
        /// <param name="configureOption">Configure Doppler Options.</param>
        /// <returns>
        /// <seealso cref="IConfigurationProvider"/>
        /// </returns>
        /// <exception cref="DopplerException">When either <paramref name="client"/> or any value of <see cref="DopplerConfigurationOptions"/> is not valid.</exception>
        public static IConfigurationBuilder AddDoppler(
         this IConfigurationBuilder configurationBuilder, Action<DopplerConfigurationOptions> configureOption)
        {
            var options = new DopplerConfigurationOptions();
            configureOption.Invoke(options);

            Console.WriteLine(options.ServiceToken);
            return configurationBuilder.AddDoppler(options);

        }

        /// <summary>
        /// Adds an <seealso cref="IConfigurationProvider"/> that reads configuration values from Doppler.
        /// </summary>
        /// <param name="configurationBuilder"> <seealso cref="IConfigurationBuilder"/></param>
        /// <param name="serviceToken">A Doppler Service Token provides read-only secrets access to a specific config within a project.</param>
        /// <param name="reloadInterval">The timespan to wait between attempts at polling the Azure Key Vault for changes. <code>null</code> to disable reloading. </param>
        /// <returns>
        /// <seealso cref="IConfigurationProvider"/>
        /// </returns>
        /// <exception cref="DopplerException">When either <paramref name="client"/> or any value of <see cref="DopplerConfigurationOptions"/> is not valid.</exception>

        public static IConfigurationBuilder AddDoppler(
            this IConfigurationBuilder configurationBuilder,
            string serviceToken,
             TimeSpan? reloadInterval = null)
        {
            var options = new DopplerConfigurationOptions
            {
                ServiceToken = serviceToken,
                ReloadInterval = reloadInterval

            };
            return configurationBuilder.AddDoppler(options);
        }

        /// <summary>
        /// Adds an <seealso cref="IConfigurationProvider"/> that reads configuration values from Doppler.
        /// </summary>
        /// <param name="configurationBuilder"> <seealso cref="IConfigurationBuilder"/></param>
        /// <param name="personalToken">Personal tokens should only be used when automating tasks that can't be accomplished using <seealso cref="DopplerConfigurationOptions.ServiceToken"/>.</param>
        /// <param name="project">A project in Doppler is where you define the app config. </param>
        /// <param name="config">A Config in Doppler is where you define the Environments.</param>
        /// <param name="reloadInterval">The timespan to wait between attempts at polling the Azure Key Vault for changes. <code>null</code> to disable reloading. </param>
        /// <returns>
        /// <seealso cref="IConfigurationProvider"/>
        /// </returns>
        /// <exception cref="DopplerException">When either <paramref name="client"/> or any value of <see cref="DopplerConfigurationOptions"/> is not valid.</exception>

        public static IConfigurationBuilder AddDoppler(
            this IConfigurationBuilder configurationBuilder,
            string personalToken,
            string project,
            string config,
            TimeSpan? reloadInterval = null)
        {
            var options = new DopplerConfigurationOptions()
            {
                PersonalToken = personalToken,
                Project = project,
                Config = config,
                ReloadInterval = reloadInterval
            };
            return configurationBuilder.AddDoppler(options);
        }

        /// <summary>
        /// Adds an <seealso cref="IConfigurationProvider"/> that reads configuration values from Doppler.
        /// </summary>
        /// <param name="configurationBuilder"> <seealso cref="IConfigurationBuilder"/></param>
        /// <param name="configureOption">Configure Doppler Options</param>
        /// <param name="client">The Doppler Client.</param>
        /// <returns>
        /// <seealso cref="IConfigurationProvider"/>
        /// </returns>  
        /// <exception cref="DopplerException">When either <paramref name="client"/> or any value of <see cref="DopplerConfigurationOptions"/> is not valid.</exception>

        public static IConfigurationBuilder AddDoppler(
            this IConfigurationBuilder configurationBuilder,
            DopplerConfigurationOptions configureOption,
            DopplerClient? client = null
            )
        {

            if (string.IsNullOrEmpty(configureOption.ServiceToken) && string.IsNullOrEmpty(configureOption.PersonalToken))
            {
                throw new DopplerException("Either ServiceToken or PersonalToken is required.");
                
            }
            else
            {
                client ??= new DopplerClient(DopplerUri.Create(), configureOption!.ServiceToken!);
            }

            if (string.IsNullOrEmpty(configureOption.ServiceToken))
            {
                if (!string.IsNullOrEmpty(configureOption.PersonalToken))
                {

                    if (string.IsNullOrEmpty(configureOption.Project))
                    {
                        throw new DopplerException("Project is required. if PersonalToken is in use.");
                    }
                    if (string.IsNullOrEmpty(configureOption.Config))
                    {
                        throw new DopplerException("Config is required. if PersonalToken is in use.");
                    }
                }
                else
                {
                    client ??= new DopplerClient(DopplerUri.Create(), configureOption!.PersonalToken!, configureOption.Project, configureOption.Config);

                }
            }

            if (configureOption.ReloadInterval != null && configureOption.ReloadInterval.Value <= TimeSpan.Zero)
            {
                throw new DopplerException(nameof(configureOption.ReloadInterval) + " must be positive.");
            }
            configurationBuilder.Add(new DopplerConfigurationSource(client, configureOption));

            return configurationBuilder;
        }
    }
}