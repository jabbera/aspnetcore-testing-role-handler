# AspNetCore.Testing.Authentication.ClaimInjector

![Build Status](https://img.shields.io/azure-devops/build/mike-barry/aspnetcore-testing-role-handler/9.svg) 
![Test Status](https://img.shields.io/azure-devops/tests/mike-barry/aspnetcore-testing-role-handler/9.svg)
![Coverage](https://img.shields.io/azure-devops/coverage/mike-barry/aspnetcore-testing-role-handler/9.svg)
![NuGet](https://img.shields.io/nuget/v/AspNetCore.Testing.Authentication.ClaimInjector.svg)

Excercising claim based logic using Microsoft.AspNetCore.Mvc.Testing is now as simple as:

```
    [Fact]
    public async Task RoleCustomizationWorksTest()
    {
        this._factory.RoleConfig.Roles = new[] {"Reader"};

        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/Values/RequireRoleReader");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task NoAuthorizationHeaderReturnsUnauthorizedTest()
    {
        this._factory.RoleConfig.AnonymousRequest = true;

        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/Values/AllowAuthorized");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    private readonly CustomRoleWebApplicationFactory<Startup> _factory;

    public UnitTests(CustomRoleWebApplicationFactory<Startup> factory)
    {
        this._factory = factory;
        this._factory.RoleConfig.Reset();
    }
```

For more examples see the unit tests [here](https://github.com/jabbera/aspnetcore-testing-role-handler/blob/master/AspNetCore.Testing.Authentication.ClaimInjector.Test/UnitTests.cs).

After learning about this great new library [Microsoft.AspNetCore.Mvc.Testing](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2) I was excited to try it. Then I found out it has little to little to no built in support for testing controllers with Role based Authorization. 

* [How to create authenticated request?](https://github.com/aspnet/Docs/issues/688)
* [add Integration Testing chapter for the Contoso University example code](https://github.com/aspnet/Docs/issues/3438)
* [Create sample for Integration testing with user claims.](https://github.com/aspnet/Docs/issues/3833)
* [Document how to replace the Authentication/Identity related middlewares in the integration tests](https://github.com/aspnet/Docs/issues/9957)
* [How to test Web API with Jwt Bearer Authentication](https://github.com/aspnet/Docs/issues/8796)


But there are workarounds (they all stink):

* make controllers anonymous for integration testing
* using integrated windows auth
* host a separate identity server

None of these are both simple and allow for exercising the role based logic without a ton of complexity. This solution solves that.

