# Doppler Secrets configuration provider for Microsoft.Extensions.Configuration

The `Doppler.Extensions.Configuration` package allows storing configuration values using Doppler Secrets.

## Getting started

### Install the package
Install the package with Nuget:

```dotnetcli
dotnet add package Doppler.Extensions.Configuration
```

### Prerequisites
You need a [Doppler](https://doppler.com/join?invite=6009451B) account to use this package.

## Examples

To load initialize configuration from Doppler secrets call the `AddDoppler` on `ConfigurationBuilder`:

```C# Snippet:ConfigurationDoppler
ConfigurationBuilder builder = new ConfigurationBuilder();
builder.AddDoppler(builder.Configuration.GetValue<string>("Doppler:ServiceToken"))

IConfiguration configuration = builder.Build();
Console.WriteLine(configuration["MySecret"]);
```