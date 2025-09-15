using System.Security.Claims;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Authentication;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;

namespace ServicesTestFramework.WebAppTools.Tests.Authentication;

public class MockAuthenticationFakeTokenTests : BaseTest
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
    public async Task FakeToken_WithJwtId_SetsJwtNameIdentifier()
    {
        var expectedUserId = Guid.NewGuid();
        var userId = await Client.GetClaimByType(ClaimTypes.NameIdentifier, FakeToken.WithJwtId(expectedUserId));

        userId.Should().Be(expectedUserId.ToString());
    }

    [Test]
    public async Task FakeToken_WithClaim_SetsSpecifiedClaimWithProvidedValue()
    {
        var claimType = "customType";
        var claimValue = "value";
        var claim = new Claim(claimType, claimValue);

        var actualClaimValue = await Client.GetClaimByType(claimType, FakeToken.WithClaim(claim));

        actualClaimValue.Should().Be(claimValue);
    }

    [Test]
    public async Task FakeToken_WithMultipleClaims_SetsSpecifiedClaims()
    {
        var claimType = "customType";
        var additionalClaimType = "additionalType";
        var token = FakeToken.WithClaim(claimType).AndClaim(claimType, "42").AndClaim(additionalClaimType, "value").AndJwtId();

        var claimValues = await Client.GetClaimsByType(claimType, token);
        var additionalClaimValue = await Client.GetClaimByType(additionalClaimType, token);
        var userIdValue = await Client.GetClaimByType(ClaimTypes.NameIdentifier, token);

        claimValues.Should().BeEquivalentTo(string.Empty, "42");
        additionalClaimValue.Should().Be("value");
        Guid.TryParse(userIdValue, out var userId).Should().BeTrue();
        userId.Should().NotBe(Guid.Empty);
    }

    [Test]
    public async Task FakeToken_WithUserId_SetsUserIdClaim()
    {
        var userIdKey = "UserId";
        var userId = 42u;
        var token = FakeToken.WithUserId(userId);
        var actualUserId = await Client.GetClaimByType(userIdKey, token);

        actualUserId.Should().Be(userId.ToString());
    }

    [Test]
    public async Task FakeToken_AndUserId_SetsUserIdClaim()
    {
        var userIdKey = "UserId";
        var userId = 42u;
        var token = FakeToken.WithJwtId().AndUserId(userId);
        var actualUserId = await Client.GetClaimByType(userIdKey, token);

        actualUserId.Should().Be(userId.ToString());
    }
}
