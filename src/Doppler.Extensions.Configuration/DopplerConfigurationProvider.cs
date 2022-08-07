using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Doppler.Extensions.Configuration
{
    /// <summary>
    /// A Doppler based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public class DopplerConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly TimeSpan? _reloadInterval;
        private readonly DopplerClient _client;
        private ConcurrentDictionary<string, string>? _loadedSecrets;
        private Task? _pollingTask;
        private  bool _shouldReload;
        private readonly CancellationTokenSource _cancellationToken;

        /// <summary>
        /// Creates a new instance of <see cref="DopplerConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="DopplerClient"/> to use for retrieving values.</param>
        /// <param name="options">The <see cref="DopplerConfigurationOptions"/> to configure provider behaviors.</param>
        public DopplerConfigurationProvider(DopplerClient client, DopplerConfigurationOptions options)
        {
           
            _client = client;
            _pollingTask = null;
            _cancellationToken = new CancellationTokenSource();
            _reloadInterval = options.ReloadInterval;
        }

        /// <summary>
        /// Load secrets into this provider.
        /// </summary>
        public override void Load() => LoadAsync().GetAwaiter().GetResult();

        private async Task PollForSecretChangesAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                await WaitForReload().ConfigureAwait(false);
                try
                {
                    await LoadAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }

        protected virtual Task WaitForReload()
        {
            // WaitForReload is only called when the _reloadInterval has a value.
            return Task.Delay(_reloadInterval!.Value, _cancellationToken.Token);
        }

        private async Task LoadAsync()
        {
            var fromSource = await _client.LoadSecretsAsync().ConfigureAwait(false);
            var oldLoadedSecrets = Interlocked.Exchange(ref _loadedSecrets, null);
            var newLoadedSecrets = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var secret in fromSource)
            {
                if (oldLoadedSecrets != null &&
                    oldLoadedSecrets.TryGetValue(secret.Key, out var existingSecret) &&
                    IsUpToDate(existingSecret, secret.Value))
                {
                    _shouldReload = true;
                    oldLoadedSecrets.Remove(secret.Key, out _);
                    newLoadedSecrets.TryAdd(secret.Key, secret.Value);
                }
                else
                {
                    newLoadedSecrets.TryAdd(secret.Key, secret.Value);
                }
            }

            if (_shouldReload== false &&(oldLoadedSecrets?.Count!= newLoadedSecrets.Count))
            {
                _shouldReload = true;
            }

            _loadedSecrets = newLoadedSecrets;

            // Reload is needed if we are loading secrets that were not loaded before or
            // secret that was loaded previously is not available anymore
            if (newLoadedSecrets.Any())
            {
                Data = newLoadedSecrets;
                if (_shouldReload)
                {
                     OnReload();
                    _shouldReload = false;
                }
            }

            // schedule a polling task only if none exists and a valid delay is specified
            if (_pollingTask == null && _reloadInterval != null)
            {
                _pollingTask = PollForSecretChangesAsync();
            }
        }

        /// <summary>
        /// Frees resources held by the <see cref="DopplerConfigurationProvider"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees resources held by the <see cref="DopplerConfigurationProvider"/> object.
        /// </summary>
        /// <param name="disposing">true if called from <see cref="Dispose()"/>, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationToken.Cancel();
                _cancellationToken.Dispose();
            }
        }

        private static bool IsUpToDate(string current, string updated)=> !string.Equals(current, updated, StringComparison.Ordinal);
    }
}