using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Authentication.Extensions
{
    public static class TestOutputHelperExtensions
    {
        public static string TestName(this ITestOutputHelper testOutputHelper)
        {
            if (testOutputHelper == null)
                throw new ArgumentNullException(nameof(testOutputHelper));

            var test = (ITest)testOutputHelper.GetTestMethod().GetValue(testOutputHelper);

            return test!.TestCase.TestMethod.Method.Name;
        }

        public static Dictionary<string, List<string>> Traits(this ITestOutputHelper testOutputHelper)
        {
            if (testOutputHelper == null)
                throw new ArgumentNullException(nameof(testOutputHelper));

            var test = (ITest)testOutputHelper.GetTestMethod().GetValue(testOutputHelper);

            return test!.TestCase.Traits;
        }

        public static bool HasTrait(this ITestOutputHelper testOutputHelper, string traitName, string value)
        {
            var hasTrait = testOutputHelper.Traits().TryGetValue(traitName, out var traitValues);
            return hasTrait && traitValues.Contains(value);
        }

        private static FieldInfo GetTestMethod(this ITestOutputHelper testOutputHelper)
        {
            var testOutputType = testOutputHelper.GetType();
            var testMember = testOutputType.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);

            if (testMember == null)
                throw new($"Unable to find 'test' field on {testOutputType.FullName}");

            return testMember;
        }
    }
}
