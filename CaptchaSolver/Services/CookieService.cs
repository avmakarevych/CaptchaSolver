using Newtonsoft.Json;
using OpenQA.Selenium;

namespace CaptchaSolver.Services;

public class CookieService
{
    private readonly IWebDriver _driver;
    private readonly string _basePath;
    public void UpdateCookie(string Account)
    {
        try
        {
            var cookies = _driver.Manage().Cookies.AllCookies;
            var json = JsonConvert.SerializeObject(cookies);
            File.WriteAllText($"{_basePath}\\accounts\\{Account}\\cookie.txt", json);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString());
            Console.ResetColor();
            Console.WriteLine(" |   INFO  | Cookie updated successfully");
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DateTime.Now.ToString());
            Console.ResetColor();
            Console.WriteLine(" |   INFO  | Update Cookie Error");
            Thread.Sleep(10000);
            UpdateCookie(Account);
        }        
    }
    public void UpdateCookie()
    {
        try
        {
            var cookies = _driver.Manage().Cookies.AllCookies;
            var json = JsonConvert.SerializeObject(cookies);
            File.WriteAllText("cookie.txt", json);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString());
            Console.ResetColor();
            Console.WriteLine(" |   INFO  | Cookie updated successfully");
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DateTime.Now.ToString());
            Console.ResetColor();
            Console.WriteLine(" |   INFO  | Update Cookie Error");
            Thread.Sleep(10000);
            UpdateCookie();
        }    
    }
}