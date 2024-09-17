## Playwright Setup And Teardown with IsolatedContexts
This is just the setup and teardown procedures for some Playwright tests. I've included the use of an `IsolatedContext` attribute to manage tests that require separate browser contexts, like UI authentication tests.

**IsolatedContext Attribute**

The `IsolatedContextAttribute` is used to mark tests that need a separate browser context, like an login page UI test. 

**Setup and Teardown**

The `OneTimeSetup` and `OneTimeTearDown` methods handle the initial setup of the browser and context, as well as cleanup after all tests are finished. `TestSetup` and `TestTearDown` manage the setup and teardown for each individual test.

**Authentication State**

The setup checks for a saved authentication state and uses it if available, otherwise, it performs a login and saves the authentication state for future tests.

In the setup, we also a check for the `IsolatedContextAttribute`. If that attribute is present, then we do not reuse the saved authentication state. 
