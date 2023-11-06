using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using OpenQA.Selenium;
using System.Drawing;
using CaptchaSolver.Utilities;
using OpenQA.Selenium.Interactions;

namespace CaptchaSolver.Services;

public class CaptchaSolverService
{
    private readonly IWebDriver _driver;
    private readonly TelegramService _telegramService;
    private readonly List<string> _linksBlackList;
    private readonly string _path;
    private readonly string _name;
    private Random _rnd;
    private Logger _logger;
    private int _successCount = 0;

    public Rectangle solverCaptcha(string screenshotPath)
    {
        Rectangle match1 = new Rectangle(105, 220, 149, 26);
        try
        {
            Image<Bgr, byte> source = new Image<Bgr, byte>($"{_path}\\{_name}_captcha.png");
            Image<Bgr, byte> template = new Image<Bgr, byte>($"{_path}\\captcha.png");
            using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcorrNormed))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                if (maxValues[0] > 0.9)
                {
                    Rectangle match = new Rectangle(maxLocations[0], template.Size);
                    return match;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error solver");
            Console.WriteLine(e.InnerException);
        }
        try
        {
            var error = _driver.FindElement(By.ClassName("error"));
            Console.WriteLine(error.Text);
            _telegramService.SendProfileAsync(_name + "\n" + error.Text);
        }
        catch
        {
            Console.WriteLine("Position not found, trying default 105, 220.");
        }
        return match1;
    }
    public Rectangle solver()
    {
        Rectangle match1 = new Rectangle(105, 220, 149, 26);
        try
        {
            Image<Bgr, byte> source = new Image<Bgr, byte>($"{_path}\\{_name}_page.png");
            Image<Bgr, byte> template = new Image<Bgr, byte>($"{_path}\\party.png");
            using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcorrNormed))
            {
                double[] minValues, maxValues;
                System.Drawing.Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                if (maxValues[0] > 0.9)
                {
                    Rectangle match = new Rectangle(maxLocations[0], template.Size);
                    return match;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error solver");
            Console.WriteLine(e.InnerException);
        }
        try
        {
            var error = _driver.FindElement(By.ClassName("error"));
            Console.WriteLine(error.Text);
            _telegramService.SendProfileAsync(_name + "\n" + error.Text);
        }
        catch
        {
            Console.WriteLine("Position not found, trying default 105, 220.");
        }
        return match1;
    }
    public void ParticipateCaptcha()
    {
        try
        {
            Thread.Sleep(500);
            Actions actions = new Actions(_driver);
            Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();
            ss.SaveAsFile($"{_path}\\{_name}_page.png", ScreenshotImageFormat.Png);
            Thread.Sleep(1000);
            
            Rectangle res = solver();
            int randomX = _rnd.Next(res.X, res.X + res.Width);
            int randomY = _rnd.Next(res.Y, res.Y + res.Height);
            
            ActionBuilder actionBuilder = new ActionBuilder();
            PointerInputDevice mouse = new PointerInputDevice(PointerKind.Mouse);
            actionBuilder.AddAction(mouse.CreatePointerMove(CoordinateOrigin.Viewport, randomX, randomY, TimeSpan.Zero));
            actionBuilder.AddAction(mouse.CreatePointerDown(MouseButton.Left));
            actionBuilder.AddAction(mouse.CreatePointerUp(MouseButton.Left));
            ((IActionExecutor)_driver).PerformActions(actionBuilder.ToActionSequenceList());

            Thread.Sleep(_rnd.Next(500, 1500));
            
            _successCount++;
            _logger.PrintSuccess($"Success participation! -=- {_successCount} -=-");
        }
        catch (Exception e)
        {
            _logger.PrintException(e);
        }
    }
    public string TakeScreenshot(string fileName)
    {
        string screenshotPath = Path.Combine(_path, $"{fileName}.png");
        
        Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();
        ss.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
        
        return screenshotPath;
    }

    public void ScrollToContestElementAndParticipate()
    {
        Thread.Sleep(500);
        IWebElement element = _driver.FindElement(By.ClassName("contestThreadBlock"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);

        string screenshotPath = TakeScreenshot($"_{_name}_page");
        Rectangle captchaArea = solverCaptcha(screenshotPath);

        int randomX = _rnd.Next(captchaArea.X, captchaArea.X + captchaArea.Width);
        int randomY = _rnd.Next(captchaArea.Y, captchaArea.Y + captchaArea.Height);

        MoveToAndClick(randomX, randomY);

        Thread.Sleep(_rnd.Next(2000, 10000));
    }
    
    public void ScrollToElementAndClick(string cssSelector, int yOffset)
    {
        var element = _driver.FindElement(By.CssSelector(cssSelector));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", element);
        ((IJavaScriptExecutor)_driver).ExecuteScript($"window.scrollBy(0, {yOffset});");

        var action = new Actions(_driver);
        action.MoveToElement(element).Click().Perform();
    }

    public void CheckParticipationOutcome(string link)
    {
        try
        {
            if (_driver.FindElement(By.ClassName("LztContest--alreadyParticipating")).Displayed)
            {
                _successCount++;
                _logger.PrintSuccess($"Success participation! Total successes: {_successCount}");
                _linksBlackList.Add(link);
            }
        }
        catch (NoSuchElementException ex)
        {
            _logger.PrintError(ex.Message);
        }
    }

    public void MoveToAndClick(int x, int y)
    {
        Actions actions = new Actions(_driver);
        actions.MoveByOffset(x, y).Click().Perform();
        actions.MoveByOffset(-x, -y).Perform();
    }
}