using System.Net;
using RestEase;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Authentication;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;

namespace ServicesTestFramework.WebAppTools.Tests.Authentication;

public class AuthenticationTests
{
    private static IFirstController Client { get; set; }

    [Before(Class)]
    public static void ClassSetup()
    {
        var httpClient = new WebApplicationBuilder<Startup>()
            .AddMockAuthentication()
            .CreateClient();

        Client = httpClient.ClientFor<IFirstController>();
    }

    [Test]
    public async Task FromSubjectId_SetsSpecifiedIdToAuthenticatedUser()
    {
        var expectedUserId = Guid.NewGuid();
        var userId = await Client.GetUserId(FakeToken.WithJwtId(expectedUserId));

        userId.Should().Be(expectedUserId.ToString());
    }

    [Test]
    public async Task FromSubjectId_SetsSpecifiedPolicyToAuthenticatedUser()
    {
        var expectedUserId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<ApiException>(() => Client.GetUserIdWithPolicy(FakeToken.WithJwtId(expectedUserId)));
        ex.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var requiredPolicy = "TestPolicy";
        var userId = await Client.GetUserIdWithPolicy(FakeToken.WithJwtId(expectedUserId).And(requiredPolicy));

        userId.Should().Be(expectedUserId.ToString());
    }
}
