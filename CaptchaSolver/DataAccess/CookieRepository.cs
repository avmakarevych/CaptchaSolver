using CaptchaSolver.Models;
using Newtonsoft.Json;

namespace CaptchaSolver.DataAccess;

public class CookieRepository
{
    private readonly string _basePath;

    public CookieRepository(string basePath)
    {
        _basePath = basePath;
    }

    public List<Cookie> LoadCookies(string account)
    {
        List<Cookie> cookies = new List<Cookie>();
        string filePath = Path.Combine(_basePath, "accounts", account, "cookie.txt");

        using (StreamReader reader = new StreamReader(filePath))
        {
            string json = reader.ReadToEnd();
            cookies = JsonConvert.DeserializeObject<List<Cookie>>(json);
        }

        return cookies;
    }
    public List<Cookie> LoadCookies()
    {
        List<Cookie> cookie = new List<Cookie>();
        using (StreamReader r = new StreamReader("cookie.txt"))
        {
            string json = r.ReadToEnd();
            cookie = JsonConvert.DeserializeObject<List<Cookie>>(json);
        }
        return cookie;
    }
}