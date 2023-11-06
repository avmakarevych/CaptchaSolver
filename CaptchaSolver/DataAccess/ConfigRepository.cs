using CaptchaSolver.Models;
using Newtonsoft.Json;

namespace CaptchaSolver.DataAccess;

public class ConfigRepository
{
    private readonly string _basePath;

    public ConfigRepository(string basePath)
    {
        _basePath = basePath;
    }

    public Config LoadConfig(string account)
    {
        Config config = new Config();
        string filePath = Path.Combine(_basePath, "accounts", account, "config.txt");

        using (StreamReader reader = new StreamReader(filePath))
        {
            string json = reader.ReadToEnd();
            config = JsonConvert.DeserializeObject<Config>(json);
        }

        return config;
    }
    
    public Config LoadConfig(Config config)
    {
        using (StreamReader r = new StreamReader("config.txt"))
        {
            string json = r.ReadToEnd();
            config = JsonConvert.DeserializeObject<Config>(json);
        }
        return config;
    }
    public Config LoadConfig(Config config, string Account)
    {
        using (StreamReader r = new StreamReader($"{_basePath}\\accounts\\{Account}\\config.txt"))
        {
            string json = r.ReadToEnd();
            config = JsonConvert.DeserializeObject<Config>(json);
        }
        return config;
    }
}