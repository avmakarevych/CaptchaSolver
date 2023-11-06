using OpenQA.Selenium;

namespace CaptchaSolver.Utilities;

public class BrowserActionHelper
{
    private readonly IWebDriver _driver;

    // JavaScript for smooth scrolling
    static string smoothScrollToScript = @"
var smoothScrollTo = function(element, duration) {
    var start = window.pageYOffset,
        target = element.offsetTop,
        distance = target - start,
        startTime = Date.now(),
        easeInOutQuad = function (t, b, c, d) {
            t /= d/2;
            if (t < 1) return c/2*t*t + b;
            t--;
            return -c/2 * (t*(t-2) - 1) + b;
        };

    var animation = function() {
        var timeElapsed = Date.now() - startTime,
            percentage = Math.min(timeElapsed / duration, 1);
        window.scrollTo(0, easeInOutQuad(percentage, start, distance, 1));
        if(percentage < 1) {
            requestAnimationFrame(animation);
        }
    };
    requestAnimationFrame(animation);
};

// Call the function with the provided arguments
smoothScrollTo(arguments[0], arguments[1]);
";

    public BrowserActionHelper(IWebDriver driver)
    {
        _driver = driver;
    }
    
    public void SmoothScrollToElement(IWebElement element, int duration = 2000)
    {
        IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)_driver;
        jsExecutor.ExecuteScript(smoothScrollToScript, element, duration);
    }

}
