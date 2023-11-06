using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaptchaSolver.Services
{
    public class NotificationService
    {
        private readonly IWebDriver _driver;
        private readonly TelegramService _telegramService;
        private readonly List<string> _notifBlackList;
        private readonly string _host;

        public NotificationService(IWebDriver driver, TelegramService telegramService, string host)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _telegramService = telegramService ?? throw new ArgumentNullException(nameof(telegramService));
            _notifBlackList = new List<string>();
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }
        
        public void CheckNotifications()
        {
            try
            {
                var alertsCounter = _driver.FindElement(By.Id("AlertsMenu_Counter")).FindElement(By.ClassName("Total")).Text;
                Console.WriteLine("Alerts Counter: " + alertsCounter);
                if (int.Parse(alertsCounter) != 0)
                {
                    _driver.Navigate().GoToUrl(_host + "/account/alerts");
                    _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
                    Thread.Sleep(1000);
                    Console.WriteLine(_driver.FindElement(By.Id("AlertsMenu_Counter")).Text);
                    var alerts = _driver.FindElements(By.ClassName("alertGroup"))[0];
                    var root = alerts.FindElements(By.TagName("li"));
                    foreach (var item in root)
                    {
                        if (IsBlackListedNotification(item.Text))
                        {
                            DoBlackList(item.Text);
                        }
                        else
                        {
                            if (!_notifBlackList.Contains(item.Text))
                            {
                                _notifBlackList.Add(item.Text);
                                _telegramService.SendProfileAsync(item.Text).Wait();
                            }
                        }
                    }
                    _driver.Navigate().GoToUrl(_host + "/forums/contests/");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Error Check Notif");
            }
        }

        private bool IsBlackListedNotification(string notificationText)
        {
            return notificationText.Contains("root отправил") ||
                   notificationText.Contains("root has sent") ||
                   notificationText.Contains("Привет, ты выиграл");
        }

        private void DoBlackList(string notification)
        {
            var notifCut = ExtractNotificationId(notification);
            if (!_notifBlackList.Contains(notifCut))
            {
                _notifBlackList.Add(notifCut);
                _telegramService.SendAsync(notification).Wait();
            }
        }

        private string ExtractNotificationId(string notification)
        {
            var notifCut = notification.Split("threads/")[1];
            notifCut = notifCut.Split('/')[0];
            return notifCut;
        }
    }
}
