using System;

namespace Doppler.Extensions.Configuration
{
    public class DopplerConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the ServiceToken. Either <see cref="ServiceToken"/> or <see cref="PersonalToken"/> is required. <see cref="ServiceToken"/> takes precedence.
        /// </summary>
        public string? ServiceToken { get; set; }

        /// <summary>
        /// Gets or sets the PersonalToken. Either <see cref="ServiceToken"/> or <see cref="PersonalToken"/> is required. <see cref="ServiceToken"/> takes precedence.
        /// </summary>
        public string? PersonalToken { get; set; }

        /// <summary>
        /// Gets or sets the Project. Required, if <see cref="PersonalToken"/> is in use.
        /// </summary>
        public string? Project { get; set; }

        /// <summary>
        /// Gets or sets the Project. Required, if <see cref="PersonalToken"/> is in use.
        /// </summary>
        public string? Config { get; set; }

        /// <summary>
        /// Gets or sets the timespan to wait between attempts at polling the Doppler for changes. <code>null</code> to disable reloading.
        /// </summary>
        public TimeSpan? ReloadInterval { get; set; }
    }
}