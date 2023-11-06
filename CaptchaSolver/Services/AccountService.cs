using CaptchaSolver.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace CaptchaSolver.Services;

public class AccountService
{
    private readonly IWebDriver _driver;
    private readonly string _basePath;
    private readonly NotificationService _notificationService;
    private Account _account;

    public AccountService(IWebDriver driver, string basePath, NotificationService notificationService, Account account)
    {
        _driver = driver;
        _basePath = basePath;
        _notificationService = notificationService;
        _account = account;
    }
    public void SetupAccount()
    {
        IWebElement nickname_ = _driver.FindElement(By.Id("NavigationAccountUsername"));
        AccountSetup account = new AccountSetup();
        account.nickname = nickname_.Text;
        Actions actions = new Actions(_driver);
        IWebElement element = _driver.FindElement(By.XPath("/html/body/header/div/div[2]/nav/div/div/ul/li[3]/span"));
        actions.MoveToElement(element).Perform();
        Thread.Sleep(100);
        string link = _driver.FindElement(By.XPath("//*[@id='AccountMenu']/ul/li[1]/a")).GetAttribute("href").ToString();
        Console.WriteLine(link);
    }
    public void SetupAccount(string Account)
    {
        IWebElement nickname_ = _driver.FindElement(By.Id("NavigationAccountUsername")); 
        AccountSetup account = new AccountSetup();
        account.nickname = nickname_.Text;
        account.foldername = Account;
        account.link = _driver.FindElement(By.XPath("//*[@id='AccountMenu']/ul/li[1]/a")).GetAttribute("href").ToString();
        Console.WriteLine(account.link);
        var json = JsonConvert.SerializeObject(account);
        File.WriteAllText($"{_basePath}\\Accounts\\{Account}\\accountInfo.txt", json);
    }
    
    public void UpdateTitleStart()
    {
        _account.Balance = int.Parse(GetBalance());
        _account.Name = GetNickname();
        Console.Title = _account.Name + " | " + _account.Balance + " ₽";
        _notificationService.CheckNotifications();
    }
    public string GetBalance()
    {
        string d = "";
        try
        {
            d = _driver.PageSource;
            d = d.Split("class=\"balanceValue\">")[1];
            d = d.Split("</span>")[0];
            return d;
        }
        catch
        {
            Thread.Sleep(5000);
            GetBalance();
        }
        return d;
    }
    public string GetNickname()
    {
        IWebElement nickname = _driver.FindElement(By.Id("NavigationAccountUsername"));
        return nickname.Text;
    }
    public bool RequestUpdateCookieConfirmation()
    {
        Console.Write("Do you want to update the cookie? (y/n): ");
        string input = Console.ReadLine()?.Trim().ToLower();
        return input == "y" || input == "yes";
    }

    public bool RequestSetupAccountConfirmation()
    {
        Console.Write("Do you want to set up the account now? (y/n): ");
        string input = Console.ReadLine()?.Trim().ToLower();
        return input == "y" || input == "yes";
    }
}