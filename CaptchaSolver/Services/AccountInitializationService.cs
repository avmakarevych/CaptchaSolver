using OpenQA.Selenium;

namespace CaptchaSolver.Services
{
    public class AccountInitializationService
    {
        private IWebDriver _driver;
        private readonly SeleniumSetupService _seleniumSetupService;
        private readonly NavigationService _navigationService;
        private readonly CookieService _cookieService;
        private readonly string _host;
        private readonly string _hostMarket;

        public AccountInitializationService(
            IWebDriver driver, 
            SeleniumSetupService seleniumSetupService,
            NavigationService navigationService,
            CookieService cookieService,
            string host,
            string hostMarket)
        {
            _driver = driver;
            _seleniumSetupService = seleniumSetupService;
            _navigationService = navigationService;
            _cookieService = cookieService;
            _host = host;
            _hostMarket = hostMarket;
        }

        public void InitializeAccount()
        {
            _seleniumSetupService.SetupConfig();
            _driver = _seleniumSetupService.CreateWebDriver();
            Thread.Sleep(5000);
            Console.Clear();
            _navigationService.NavigateTo(_host);
        }

        public void InitializeAccount(string accountName)
        {
            Console.WriteLine("Account name: " + accountName);
            _seleniumSetupService.SetupConfig(accountName);
            _driver = _seleniumSetupService.CreateWebDriver();
            Thread.Sleep(5000);
            Console.Clear();
            _navigationService.NavigateTo(_host);
        }

        public void InitializeAccountMarket(string accountName)
        {
            Console.WriteLine("Account name: " + accountName);
            _seleniumSetupService.SetupConfig(accountName);
            _driver = _seleniumSetupService.CreateWebDriver();
            Thread.Sleep(5000);
            Console.Clear();
            _navigationService.NavigateTo(_hostMarket);
        }

        public void UpdateCookieAndNavigate(string accountName = "")
        {
            _navigationService.NavigateTo(_host);
            if (!string.IsNullOrEmpty(accountName))
                _cookieService.UpdateCookie(accountName);
            else
                _cookieService.UpdateCookie();
        }
    }
}
