using CaptchaSolver.DataAccess;
using CaptchaSolver.Models;
using CaptchaSolver.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Cookie = CaptchaSolver.Models.Cookie;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;

namespace CaptchaSolver.Services;

public class SeleniumSetupService
{
    private CookieRepository _cookieRepository;
    private ConfigRepository _configRepository;
    private ProxySettings _proxy;
    private IWebDriver _driver;
    private bool _sitter;
    private bool _ultraSpeed;
    private Logger _logger;

    public SeleniumSetupService(
        CookieRepository cookieRepository, 
        ConfigRepository configRepository, 
        ProxySettings proxySettings, 
        bool sitter,
        bool ultraSpeed,
        Logger logger)
    {
        _cookieRepository = cookieRepository;
        _configRepository = configRepository;
        _proxy = proxySettings;
        _sitter = sitter;
        _ultraSpeed = ultraSpeed;
        _logger = logger;
    }
    public IWebDriver CreateWebDriver()
    {
        return new ChromeDriver(SetupChromeOptions());
    }
    public ChromeOptions SetupChromeOptions()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
        
        options.AddHttpProxy(_proxy.Host, _proxy.Port, _proxy.User, _proxy.Password);
        if (_sitter)
            options.AddArgument("window-size=1280,720");
        else
            options.AddArgument("window-size=700,1100");
        if (_ultraSpeed)
            options.AddArgument("window-size=1280,720");
        if (!_sitter)
        {
            options.AddArgument("--host-rules=MAP i.imgur.com 127.0.0.1");
            options.AddArgument("--enable-features=HostRules");
        }
         options.AddArgument("--disable-notifications");;
         options.AddArgument("--disable-blink-features=AutomationControlled");
         options.AddExcludedArgument("enable-automation");
         options.AddExcludedArgument("enable-logging");
         return options;
    }
    
    public void SetupCookie(string Account)
    {
        List<Cookie> cookie = _cookieRepository.LoadCookies(Account);
        foreach (var i in cookie)
        {
            OpenQA.Selenium.Cookie cookie1 = new OpenQA.Selenium.Cookie(i.Name, i.Value);
            _driver.Manage().Cookies.AddCookie(cookie1);
        }
        _logger.PrintSuccess("cookie setup");
    }
      
    public void SetupCookie()
    {
        List<Cookie> cookie = _cookieRepository.LoadCookies();
        foreach (var i in cookie)
        {
            OpenQA.Selenium.Cookie cookie1 = new OpenQA.Selenium.Cookie(i.Name, i.Value);
            _driver.Manage().Cookies.AddCookie(cookie1);
        }
        _logger.PrintSuccess("cookie setup");
    }
      
    public void SetupConfig()
    {
        Config config = new Config();
        config = _configRepository.LoadConfig(config);
        SetupProxy(config);
        _logger.PrintSuccess("config setup");
    }
      
    public void SetupConfig(string Account)
    {
        Config config = new Config();
        config = _configRepository.LoadConfig(config, Account);
        SetupProxy(config);
        _logger.PrintSuccess("config setup");
    }
    public void SetupProxy(Config config)
    {
        if (config.proxy.Contains("@"))
        {
            string[] temp = config.proxy.Split('@');
            string[] temp0 = temp[0].Split(':');
            string[] temp1 = temp[1].Split(':');
            _proxy.User = temp0[0];
            _proxy.Password = temp0[1];
            _proxy.Host = temp1[0];
            _proxy.Port = int.Parse(temp1[1]);
        }
        else
        {
            _proxy.Host = config.proxy.Split(':')[0];
            _proxy.Port = int.Parse(config.proxy.Split(':')[1]);
            _proxy.User = config.proxy.Split(':')[2];
            _proxy.Password = config.proxy.Split(':')[3];
        }
    }
}