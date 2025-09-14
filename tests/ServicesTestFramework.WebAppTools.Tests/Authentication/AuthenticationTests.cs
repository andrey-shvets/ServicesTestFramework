using System.Net;
using FluentAssertions;
using RestEase;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Authentication;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Tests.Authentication;

public class AuthenticationTests : BaseTest, IClassFixture<WebApplicationBuilder<Startup>>
{
    private IFirstController Client { get; }

    public AuthenticationTests(WebApplicationBuilder<Startup> builder, ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var httpClient = builder
            .AddMockAuthentication()
            .AddXUnitLogger(OutputHelper)
            .CreateClient();

        Client = httpClient.ClientFor<IFirstController>();
    }

    [Fact]
    public async Task FromSubjectId_SetsSpecifiedIdToAuthenticatedUser()
    {
        var expectedUserId = Guid.NewGuid();
        var userId = await Client.GetUserId(FakeToken.WithJwtId(expectedUserId));

        userId.Should().Be(expectedUserId.ToString());
    }

    [Fact]
    public async Task FromSubjectId_SetsSpecifiedPolicyToAuthenticatedUser()
    {
        var expectedUserId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<ApiException>(async () => await Client.GetUserIdWithPolicy(FakeToken.WithJwtId(expectedUserId)));
        ex.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var requiredPolicy = "TestPolicy";
        var userId = await Client.GetUserIdWithPolicy(FakeToken.WithJwtId(expectedUserId).And(requiredPolicy));

        userId.Should().Be(expectedUserId.ToString());
    }
}
