using Microsoft.Playwright;

var hh = new HhPages("https://hh.ru");

using var playwright = await Playwright.CreateAsync();
await using IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false,
});

//Прикидываемся хромом:
BrowserNewContextOptions device = playwright.Devices["Desktop Chrome"];
await using IBrowserContext context = await browser.NewContextAsync(device);
IPage page = await context.NewPageAsync();
await Login();

Console.ReadKey();

async Task Login()
{
    await page.GotoAsync(hh.Login);
    await page.ClickAsync(hh.ButtonQa("expand-login-by-password"));
    await page.TypeAsync(hh.InputQa("login-input-username"), "12den24@gmail.com");
    await page.TypeAsync(hh.InputQa("login-input-password"), "24121985den");
    await page.ClickAsync(hh.ButtonQa("account-login-submit"));
    var incorrectPassword = await page.IsVisibleAsync(hh.DivQa("account-login-error"));
    if (incorrectPassword)
    {
        throw new Exception("Incorrect password");
    }
}

class HhPages
{
    public string Host { get; }

    public HhPages(string host)
    {
        Host = host ?? throw new ArgumentNullException(nameof(host));
    }

    public string Login => $"{Host}/account/login";
    public string Resume => $"{Host}/resume";
    public string SearchVacancies => $"{Host}/search/vacancy";

    public string Vacancy(int id) => $"{Host}/vacancy/{id}";
    public string Search(string query)
    {
        var escapedQuery = Uri.EscapeDataString(query);
        return $"{SearchVacancies}?text={escapedQuery}" +
               "&search_field=name&search_field=company_name&search_field=description";
    }

    public string ButtonQa(string dataQa) => $"button[data-qa='{dataQa}']";
    public string InputQa(string dataQa) => $"input[data-qa='{dataQa}']";
    public string DivQa(string dataQa) => $"div[data-qa='{dataQa}']";
}
