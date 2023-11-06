namespace CaptchaSolver.Models;

public class Cookie
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Domain { get; set; }
    public double Expiry { get; set; }
    public string Path { get; set; }
}