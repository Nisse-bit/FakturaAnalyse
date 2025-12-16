using FakturaAnalyse;
using Microsoft.Extensions.Configuration;

public static class AzureKeyManager
{
    public static (string endpoint, string key) GetCredentials()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<MainForm>(); // Program is your entry assembly

        var configuration = builder.Build();

        string endpoint = configuration["Endpoint"];
        string key = configuration["FormKey"];

        return (endpoint, key);
    }
}