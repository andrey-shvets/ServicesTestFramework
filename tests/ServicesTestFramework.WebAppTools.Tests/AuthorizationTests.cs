using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestEase;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Authorization.Extensions;
using ServicesTestFramework.WebAppTools.Authorization.Factories;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Tests
{
    public class AuthorizationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> ApplicationFactory { get; }
        private ITestOutputHelper OutputHelper { get; }
        protected IAuthTokenFactory AuthTokenFactory { get; } = TestAuthTokenFactory.Instance;

        public AuthorizationTests(WebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            ApplicationFactory = factory;
            OutputHelper = testOutputHelper;
        }

        [Fact(Skip = "AddMockAuthentication requires very specific conditions, but we are working on it")]
        public async Task FromSubjectId_SetsSpecifiedIdToAuthenticatedUser()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services => services.AddMockAuthentication(),
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var expectedUserId = Guid.NewGuid();
            var userId = await firstClient.GetUserId(AuthTokenFactory.FromSubjectId(expectedUserId));

            userId.Should().Be(expectedUserId.ToString());
        }

        [Fact(Skip = "AddMockAuthentication requires very specific conditions, but we are working on it")]
        public async Task FromSubjectId_SetsSpecifiedPolicyToAuthenticatedUser()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services => services.AddMockAuthentication(),
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var expectedUserId = Guid.NewGuid();

            Func<Task> getWithPolicy = async () => await firstClient.GetUserIdWithPolicy(AuthTokenFactory.FromSubjectId(expectedUserId));
            await getWithPolicy.Should().ThrowAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.Unauthorized);

            var requiredPolicy = "TestPolicy";
            var userId = await firstClient.GetUserIdWithPolicy(AuthTokenFactory.FromSubjectId(expectedUserId, requiredPolicy));
            userId.Should().Be(expectedUserId.ToString());
        }
    }
}
