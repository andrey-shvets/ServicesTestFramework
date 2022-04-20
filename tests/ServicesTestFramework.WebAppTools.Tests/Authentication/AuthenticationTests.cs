using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestEase;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Authentication.Extensions;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Tests.Authentication
{
    public class AuthenticationTests : BaseTest, IClassFixture<WebApplicationFactory<Startup>>
    {
        private IFirstController Client { get; }

        public AuthenticationTests(WebApplicationFactory<Startup> factory, ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            var httpClient = factory.WithWebHostConfiguration(
                    servicesConfiguration: services => services.AddMockAuthentication(),
                    testOutputHelper: OutputHelper)
                .CreateClient();

            Client = httpClient.ClientFor<IFirstController>();
        }

        [Fact(Skip = "AddMockAuthentication requires very specific conditions, but we are working on it")]
        public async Task FromSubjectId_SetsSpecifiedIdToAuthenticatedUser()
        {
            var expectedUserId = Guid.NewGuid();
            var userId = await Client.GetUserId(FakeToken.WithJwtId(expectedUserId));

            userId.Should().Be(expectedUserId.ToString());
        }

        [Fact(Skip = "AddMockAuthentication requires very specific conditions, but we are working on it")]
        public async Task FromSubjectId_SetsSpecifiedPolicyToAuthenticatedUser()
        {
            var expectedUserId = Guid.NewGuid();

            Func<Task> getWithPolicy = async () => await Client.GetUserIdWithPolicy(FakeToken.WithJwtId(expectedUserId));
            await getWithPolicy.Should().ThrowAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.Unauthorized);

            var requiredPolicy = "TestPolicy";
            var userId = await Client.GetUserIdWithPolicy(FakeToken.WithJwtId(expectedUserId).And(requiredPolicy));
            userId.Should().Be(expectedUserId.ToString());
        }
    }
}
