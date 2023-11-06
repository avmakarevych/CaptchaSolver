using System.Diagnostics;
using System.Drawing;
using CaptchaSolver.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace CaptchaSolver.Services;

public class ContestService
{
    private readonly IWebDriver _driver;
    private readonly string _host;
    private readonly Random _rnd;
    private readonly Logger _logger;
    private readonly string _name;
    private readonly NavigationService _navigationService;
    private readonly List<string> _linksBlackList;
    private readonly CaptchaSolverService _captchaSolverService;
    private int _successCount = 0;

    public ContestService(IWebDriver driver,
        string host,
        Logger logger,
        string name,
        NavigationService navigationService,
        CaptchaSolverService captchaSolverService,
        List<string> linksBlackList = null)
    {
        _driver = driver;
        _host = host;
        _rnd = new Random();
        _logger = logger;
        _name = name;
        _navigationService = navigationService;
        _linksBlackList = linksBlackList ?? new List<string>();
        _captchaSolverService = captchaSolverService;
    }
    public void ParticipateInContests()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        _logger.PrintSuccess("Loading contests.");

        while (true)
        {
            try
            {
                TimeSpan ts = stopWatch.Elapsed;
                if (ts.TotalSeconds > 3000)
                {
                    _logger.PrintInfo("60 minutes have passed. Let's pretend we're not there! We go to bed for 5-10 minutes.");
                    _driver.Navigate().GoToUrl("https://www.google.com/");
                    Thread.Sleep(_rnd.Next(300000, 600000));
                    stopWatch.Restart();
                }

                Thread.Sleep(_rnd.Next(1000));
                _navigationService.NavigateTo(_host + "/forums/contests/");
                List<string> links = GetContestsUrls();

                if (links.Count == 0)
                {
                    _logger.PrintInfo("Contests not found! Going sleep for 10-15 min.");
                    Thread.Sleep(_rnd.Next(600000, 900000));
                    continue;
                }

                foreach (var link in links)
                {
                    Participate(link);
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                _logger.PrintError("Error ParticipateInContests. Reloading web driver in 8 minutes");
                _logger.PrintException(e);
                _driver.Quit();
                Thread.Sleep(480000);
                _navigationService.RestartApplication();
            }
        }
    }
    
    public void ParticipateInContestsUltraSpeed()
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    _logger.PrintSuccess("Loading contests.");

    while (true)
    {
        try
        {
            TimeSpan ts = stopWatch.Elapsed;
            if (ts.TotalSeconds > 3600) // 60 minutes
            {
                _logger.PrintInfo("60 minutes have passed. Let's pretend we're not there! We go to bed for 5-10 minutes.");
                _driver.Navigate().GoToUrl("https://www.google.com/");
                Thread.Sleep(_rnd.Next(300000, 600000)); // 5-10 minutes
                stopWatch.Restart();
            }

            Thread.Sleep(_rnd.Next(1000));
            _navigationService.NavigateTo(_host + "/forums/contests/");
            var links = GetContestsUrlsUltraSpeed();

            _logger.PrintInfo($"Links count: {links.Count}");

            List<string> windowHandles = new List<string>();
            int j = 0;
            foreach (var link in links)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript($"window.open('{_host}/threads/{link}/', '_blank');");
                windowHandles.Add(_driver.WindowHandles.Last());
                Thread.Sleep(300);
                j++;
                if (j % 10 == 0)
                {
                    _logger.PrintInfo($"Already opened: {j} links");
                }
            }

            Thread.Sleep(5000);
            foreach (var handle in windowHandles)
            {
                _driver.SwitchTo().Window(handle);
                Participate();
                _driver.Close();
                Thread.Sleep(100);
            }
            _driver.SwitchTo().Window(_driver.WindowHandles.First());

            if (links.Count == 0)
            {
                _logger.PrintInfo("Contests not found! Exiting.");
                _driver.Quit();
                _navigationService.RestartApplication();
                break;
            }
        }
        catch (Exception e)
        {
            _logger.PrintException(e);
            _driver.Quit();
            Thread.Sleep(480000); // 8 minutes
            _navigationService.RestartApplication();
        }
    }
}

    
    public void ParticipateInContestsNewTab()
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        _logger.PrintInfo("Loading contests in new tab.");

        while (true)
        {
            try
            {
                TimeSpan ts = stopWatch.Elapsed;
                if (ts.TotalSeconds > 3600) // 60 minutes
                {
                    _logger.PrintInfo("60 minutes have passed. Time to rest for 40-50 minutes.");
                    _driver.Navigate().GoToUrl("https://www.google.com/");
                    Thread.Sleep(_rnd.Next(2400000, 3000000)); // 40-50 minutes
                    stopWatch.Restart();
                }

                Thread.Sleep(_rnd.Next(1000)); // Random short delay
                NavigateToContestsNewTab();

                if (DateTime.UtcNow.Hour >= 22 || DateTime.UtcNow.Hour <= 4)
                {
                    _logger.PrintInfo("It's late. Time to rest for 6-7 hours.");
                    Thread.Sleep(_rnd.Next(21600000, 25200000)); // 6-7 hours
                }
            }
            catch (Exception e)
            {
                _logger.PrintException(e);
                Thread.Sleep(5000);
                _navigationService.RestartApplication();
            }
        }
    }
    
    public void ParticipateInContestsBypass()
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        _logger.PrintInfo("Loading contests using Bypass.");

        while (true)
        {
            try
            {
                Thread.Sleep(2000);
                _navigationService.NavigateTo(_host);
                List<string> links = GetContestsUrlsBypass();

                if (links.Count == 0)
                {
                    _logger.PrintInfo("Contests not found! Going to sleep for 10-15 minutes.");
                    Thread.Sleep(_rnd.Next(600000, 900000)); // 10-15 minutes
                    continue;
                }

                foreach (var link in links)
                {
                    Participate(link);
                    Thread.Sleep(10000); // Wait for 10 seconds after each participation attempt
                }
            }
            catch (Exception e)
            {
                _logger.PrintException(e);
                Thread.Sleep(5000); // Wait for 5 seconds on error
                _navigationService.RestartApplication(); // Restart application instead of recursive call
            }
        }
    }
    
    private void NavigateToContestsNewTab()
    {
        _driver.Navigate().GoToUrl(_host + "/forums/contests/");
        _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
        var linksEl = _driver.FindElements(By.ClassName("discussionListItem")).ToList();

        if (linksEl.Count == 0)
        {
            _logger.PrintInfo("Contests not found! Going to sleep for 8-10 minutes.");
            Thread.Sleep(_rnd.Next(480000, 600000)); // 8-10 minutes
            return;
        }

        foreach (var l in linksEl)
        {
            OpenLinkInNewTab(l);
        }
    }

    private void OpenLinkInNewTab(IWebElement link)
    {
        var action = new Actions(_driver);
        action.ScrollToElement(link).Perform();
        Thread.Sleep(200);

        action.KeyDown(Keys.Control).Click(link).KeyUp(Keys.Control).Perform();
        Thread.Sleep(200);

        _driver.SwitchTo().Window(_driver.WindowHandles.Last());
        try
        {
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            Participate();
        }
        finally
        {
            _driver.Close();
            _driver.SwitchTo().Window(_driver.WindowHandles.First());
        }
    }

    public List<string> GetContestsUrlsNewTab()
    {
        List<string> links = new List<string>();
        try
        {
            Actions action = new Actions(_driver);
            _driver.Navigate().GoToUrl(_host + "/forums/766/");
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            string originalWindow = _driver.CurrentWindowHandle;
            List<IWebElement> linksEl = _driver.FindElements(By.ClassName("discussionListItem")).ToList();
            foreach (var l in linksEl)
            {
                action.ScrollToElement(l).Perform();
                Thread.Sleep(200);
                action.KeyDown(Keys.Control).MoveToElement(l, _rnd.Next(1, 140), _rnd.Next(1, 25)).Click().Perform();
                Thread.Sleep(200);
                _driver.SwitchTo().Window(_driver.WindowHandles.Last());
                Thread.Sleep(200);
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
                Thread.Sleep(200);
                Participate();
                _driver.Close();
                Thread.Sleep(200);
                _driver.SwitchTo().Window(_driver.WindowHandles.First());
                Thread.Sleep(200);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Thread.Sleep(5000);
            GetContestsUrlsNewTab();
        }
        return links;
    }

    public List<IWebElement> GetBypassLinksIWebElement()
    {
        List<IWebElement> links = new List<IWebElement>();
        List<IWebElement> linksEl = _driver.FindElements(By.ClassName("discussionListItem")).ToList();
        string[] temp;
        foreach (var l in linksEl)
        {
            try
            {
                string link = l.GetAttribute("id");
                temp = link.Split('-');
                if (!_linksBlackList.Contains(temp[1]))
                {
                    links.Add(l);
                }
            }
            catch
            {
                continue;
            }
        }
        return links;
    }

    public List<string> GetContestsUrlsBypass()
    {
        List<string> links = new List<string>();
        try
        {
            Actions actions = new Actions(_driver);
            for (int i = 0; i < _rnd.Next(1, 4); i++)
            {
                actions.SendKeys(Keys.PageDown);
                actions.Perform();
                Thread.Sleep(_rnd.Next(100, 500));
            }         
            List<IWebElement> linksEl = _driver.FindElements(By.ClassName("discussionListItem")).ToList();
            string[] temp;
            foreach (var l in linksEl)
            {
                try
                {
                    string link = l.GetAttribute("id");
                    temp = link.Split('-');
                    if (!_linksBlackList.Contains(temp[1]))
                    {
                        links.Add(temp[1]);
                    }
                }
                catch
                {
                    continue;
                }
            }
            foreach (var item in links)
            {
                Console.WriteLine(item);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Thread.Sleep(5000);
            GetContestsUrls();
        }
        Thread.Sleep(30000);
        return links;
    }

    public List<string> GetContestsUrlsUltraSpeed()
    {
        List<string> links = new List<string>();
        try
        {
            List<IWebElement> linksEl = _driver.FindElements(By.ClassName("discussionListItem")).ToList();
            string[] temp;
            foreach (var l in linksEl)
            {
                string link = l.GetAttribute("id");
                temp = link.Split('-');
                links.Add(temp[1]);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Thread.Sleep(5000);
            GetContestsUrls();
        }
        return links;
    }

    public List<string> GetContestsUrls()
    {
        List<string> links = new List<string>();
        try
        {
            List<IWebElement> linksEl = _driver.FindElements(By.ClassName("discussionListItem")).ToList();
            string[] temp;
            foreach (var l in linksEl)
            {
                string link = l.GetAttribute("id");
                temp = link.Split('-');
                links.Add(temp[1]);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Thread.Sleep(5000);
            GetContestsUrls();
        }
        Thread.Sleep(10000);
        return links;
    }
     public void Participate()
    {
        Actions actions = new Actions(_driver);
        Thread.Sleep(500);

        string screenshotPath = _captchaSolverService.TakeScreenshot($"_{_name}_page");
        Rectangle captchaArea = _captchaSolverService.solverCaptcha(screenshotPath);
        
        int randomX = _rnd.Next(captchaArea.X, captchaArea.X + captchaArea.Width);
        int randomY = _rnd.Next(captchaArea.Y, captchaArea.Y + captchaArea.Height);

        _captchaSolverService.MoveToAndClick(randomX, randomY);
    }

    public void Participate(string link)
    {
        try
        {
            _logger.PrintInfo($"Opening link: {link}");
            string fullLink = $"{_host}/threads/{link}/";
            _navigationService.NavigateTo(fullLink);

            _captchaSolverService.ScrollToContestElementAndParticipate();
            _captchaSolverService.CheckParticipationOutcome(link);
        }
        catch (Exception e)
        {
            _logger.PrintError("Error in Participate method");
            _logger.PrintException(e);
            Thread.Sleep(10000);
        }
    }
}