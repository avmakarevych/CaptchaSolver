using CaptchaSolver.Services;
using CaptchaSolver.Utilities;
using Newtonsoft.Json;

class Program
{
    private static bool sitter = false;
    private static bool ultraSpeed = false;
    private static string _host;
    private static Logger _logger;
    private static AccountInitializationService _accountInitialization;
    private static SeleniumSetupService _seleniumSetupService;
    private static CookieService _cookieService;
    private static NavigationService _navigationService;
    private static ContestService _contestService;
    private static AccountService _accountService;

    static void Main(string[] args)
    {
        LoadConfiguration();
        try
        {
            ProcessArguments(args);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error Main");
            Console.WriteLine("Error! Sleeping for 8 min\n" + e.Message);
            Thread.Sleep(480000);
            Main(args);
        }
    }

    static void LoadConfiguration()
    {
        try
        {
            var configuration = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("config.json"));
            _host = configuration["Host"];
        }
        catch (FileNotFoundException)
        {
            _logger.PrintInfo("Configuration file not found.");
        }
        catch (JsonException)
        {
            _logger.PrintInfo("Configuration file is not in the correct format.");
        }
        catch (Exception e)
        {
            _logger.PrintInfo($"An error occurred while reading the configuration: {e.Message}");
        }
    }

    static void ProcessArguments(string[] args)
    {
        if (args.Length == 0)
        {
            DefaultRoutine();
            return;
        }

        switch (args.Length)
        {
            case 1:
                sitter = true;
                _logger.PrintSuccess("sitter setup"); 
                _accountInitialization.InitializeAccount(args[0]);
                _seleniumSetupService.SetupCookie(args[0]);
                _cookieService.UpdateCookie(args[0]);
                _navigationService.NavigateTo(_host);
                break;
            case 2:
                HandleTwoArguments(args);
                break;
            default:
                Console.WriteLine("Invalid number of arguments.");
                break;
        }
    }

    static void HandleTwoArguments(string[] args)
    {
        switch (args[1].ToLower())
        {
            case "update":
                sitter = true;
                UpdateRoutine(args[0]);
                break;
            case "setup":
                SetupRoutine(args[0]);
                break;
            case "setupnew":
                SetupNewRoutine(args[0]);
                break;
            case "sitter":
                sitter = true;
                _logger.PrintSuccess("sitter setup");
                _seleniumSetupService.SetupCookie(args[0]);
                RequestUpdateCookie(args[0]);
                break;
            case "ultraspeed":
                ultraSpeed = true;
                _logger.PrintSuccess("ultraspeed setup");
                _accountInitialization.InitializeAccount(args[0]);
                _seleniumSetupService.SetupCookie(args[0]);
                _cookieService.UpdateCookie(args[0]);
                _contestService.ParticipateInContestsUltraSpeed();
                break;
            case "party":
                ParticipateRoutine(args[0], "party");
                break;
            case "bypass":
                ParticipateRoutine(args[0], "bypass");
                break;
            case "market":
            case "marketall":
                ParticipateRoutine(args[0], "market");
                break;
            default:
                Console.WriteLine($"Unknown command: {args[1]}");
                break;
        }
    }

    static void DefaultRoutine()
    {
        sitter = true;
        _logger.PrintSuccess("sitter setup");
        _accountInitialization.InitializeAccount();
        _seleniumSetupService.SetupCookie();
        _navigationService.NavigateTo(_host);
        if (_accountService.RequestUpdateCookieConfirmation())
        {
            _cookieService.UpdateCookie();
        }
    }

    static void UpdateRoutine(string account)
    {
        sitter = true;
        _accountInitialization.InitializeAccount(account);
        _navigationService.NavigateTo(_host);
        if (_accountService.RequestUpdateCookieConfirmation())
        {
            _cookieService.UpdateCookie(account);
        }
    }

    static void SetupRoutine(string account)
    {
        _accountInitialization.InitializeAccount(account);
        _seleniumSetupService.SetupCookie(account);
        _cookieService.UpdateCookie(account);
        _navigationService.NavigateTo(_host);
        _logger.PrintInfo("Account setup complete.");
        _navigationService.QuitDriver();
    }

    static void SetupNewRoutine(string account)
    {
        sitter = true;
        _accountInitialization.InitializeAccount(account);
        _navigationService.NavigateTo(_host);
        if (_accountService.RequestUpdateCookieConfirmation())
        {
            _cookieService.UpdateCookie(account);
        }
        if (_accountService.RequestSetupAccountConfirmation())
        {
            _accountService.SetupAccount(account);
        }
        _logger.PrintInfo("Account setup complete.");
        _navigationService.QuitDriver();
    }
    
    static void RequestUpdateCookie(string account)
    {
        if (_accountService.RequestUpdateCookieConfirmation())
        {
            _cookieService.UpdateCookie(account);
        }
    }
    static void ParticipateRoutine(string account, string mode)
    {
        _accountInitialization.InitializeAccount(account);
        _seleniumSetupService.SetupCookie(account);
        _cookieService.UpdateCookie(account);
        _navigationService.NavigateTo(_host);
        
        switch (mode.ToLower())
        {
            case "party":
                _contestService.ParticipateInContests();
                break;
            case "bypass":
                _contestService.ParticipateInContestsBypass();
                break;
            case "ultraspeed":
                _contestService.ParticipateInContestsUltraSpeed();
                break;
            default:
                Console.WriteLine($"Unknown participation mode: {mode}");
                break;
        }
    }
}
