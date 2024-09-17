using Microsoft.Playwright;
using Newtonsoft.Json;
using NUnit.Framework;


namespace PlaywrightTests.Pages
{
    public class TestBase
    {
        protected IPage Page { get; set; }
        protected IBrowser Browser { get; private set; }
        protected TestData TestData { get; set; }
        protected IBrowserContext SharedContext { get; private set; }
        private bool _useIsolatedContext = false;


        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var playwright = await Playwright.CreateAsync();
            Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                //SlowMo = 1000
            });

            SharedContext = await Browser.NewContextAsync();
            Page = await SharedContext.NewPageAsync();
            await Page.SetViewportSizeAsync(1920, 1080);

            if (!_useIsolatedContext)
            {
                var authStateStorePath = Path.Combine("auth_states", "auth_state.json");

                // Check if saved authentication state exists
                if (File.Exists(authStateStorePath))
                {
                    var authState = JsonConvert.DeserializeObject<List<Cookie>>(File.ReadAllText(authStateStorePath));
                    await SharedContext.AddCookiesAsync(authState);
                    await Page.GotoAsync(TestData.BaseURL);

                }
                else
                {
                    // Perform UI based login and save authentication state
                    await Page.GotoAsync(TestData.BaseURL + "/login.aspx");

                    //var lp = new LoginPage(Page);
                    //await lp.LoginAsync(TestData.TestUser);

                    var authState = await SharedContext.CookiesAsync();
                    File.WriteAllText(authStateStorePath, JsonConvert.SerializeObject(authState));
                }
            }

            Page.SetDefaultTimeout(90000);
        }


        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await SharedContext.CloseAsync();
            await Browser.CloseAsync();
        }

        [SetUp]
        public async Task TestSetup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = GetType().GetMethod(testMethod);
            _useIsolatedContext = methodInfo.GetCustomAttributes(typeof(IsolatedContextAttribute), true).Any();

            if (_useIsolatedContext)
            {
                var isolatedContext = await Browser.NewContextAsync();
                Page = await isolatedContext.NewPageAsync();
                await Page.GotoAsync(TestData.BaseURL + "/login.aspx");
            }
        }

        [TearDown]
        public async Task TestTearDown()
        {
            if (_useIsolatedContext)
            {
                await Page.Context.CloseAsync();
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class IsolatedContextAttribute : Attribute
        {
        }
    }

    public class TestData
    {
        public string BaseURL { get; set; }

        public TestUser TestUser { get; set; }
    }

    public class TestUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
