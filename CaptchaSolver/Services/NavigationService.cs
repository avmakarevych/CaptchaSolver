using CaptchaSolver.Utilities;
using OpenQA.Selenium;

namespace CaptchaSolver.Services;

public class NavigationService
{
    private readonly IWebDriver _driver;
    private Logger _logger;
    public bool IsGotError()
    {
        try
        {
            var d = _driver.PageSource;
            Thread.Sleep(1000);
            var title = _driver.Title;
            if (d.Contains("Bad gateway error"))
            {
                Console.WriteLine("Bad gateway error");
                return true;
            }
            else if (d.Contains("Проверьте своё интернет-соединение и попробуйте снова."))
            {
                Console.WriteLine("Check your internet connection and try again.");
                return true;
            }
            else if (title == "Error 502")
            {
                Console.WriteLine("Error 502");
                return true;
            }
            else if (title == "Site Maintenance")
            {
                Console.WriteLine("Site Maintenance");
                return true;
            }
            return false;
        }
        catch (NoSuchElementException)
        {
            return true;
        }
        catch
        {
            return true;
        }
    }
    public void NavigateTo(string Host)
    {
        NavigateTo(Host, "Error NavigateTo(string Host)");
    }

    public void NavigateToContestLink(string Host)
    {
        NavigateTo(Host, "Error NavigateToContestLink(string Host)");
    }

    public void NavigateToLinkBoost(string Host)
    {
        NavigateTo(Host, "Error NavigateToLinkBoost(string Host)");
    }
    
    private void NavigateTo(string Host, string errorMessage)
    {
        try
        {
            _driver.Navigate().GoToUrl(Host);
            try
            {
                Thread.Sleep(1000);
                IWebElement button = _driver.FindElement(By.CssSelector(".btn"));
                if (button != null)
                {
                    Console.WriteLine("I`m not robot - click");
                    button.Click();
                }
                Thread.Sleep(5000);
            }
            catch (Exception)
            {
            }
                
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(100);
            Thread.Sleep(2000);
            if (IsGotError())
            {
                throw new Exception();
            }
            Thread.Sleep(3000);
        }
        catch (StackOverflowException)
        {
            HandleNavigationError("StackOverflowException");
        }
        catch (NoSuchElementException)
        {
            HandleNavigationError("NoSuchElementException");
        }
        catch
        {
            Console.WriteLine(errorMessage);
            HandleNavigationError("Error");
        }
    }
    private void HandleNavigationError(string errorType)
    {
        _logger.PrintError($"Failed to load ({errorType}). Sleeping for 600 seconds before retrying.");
        Thread.Sleep(TimeSpan.FromMinutes(10)); // Wait for 10 minutes before retrying

        try
        {
            _driver.Quit(); // Close the browser and dispose of the WebDriver
        }
        catch (Exception ex)
        {
            _logger.PrintError($"Error quitting driver: {ex.Message}");
        }
        
        RestartApplication();
    }

    public void RestartApplication()
    {
        var filePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        System.Diagnostics.Process.Start(filePath);

        Environment.Exit(0);
    }
    public void QuitDriver()
    {
        try
        {
            if (_driver != null)
            {
                _logger.PrintInfo("Closing the web driver.");
                _driver.Quit();
            }
        }
        catch (Exception e)
        {
            _logger.PrintError("An error occurred while trying to close the web driver.");
            _logger.PrintException(e);
        }
    }
}