using System;
using System.Threading.Tasks;
using Xunit;

namespace Pierce.Test
{
    public static class TaskAssert
    {
        // http://stackoverflow.com/a/14103924/64290
        // https://github.com/octokit/octokit.net/blob/master/Octokit.Tests/Helpers/AssertEx.cs
        public static async Task<T> ThrowsAsync<T>(Func<Task> func)
            where T : Exception
        {
            try
            {
                await func();
                Assert.Throws<T>(() => { });
            }
            catch (T ex)
            {
                return ex;
            }

            return null;
        }
    }
}
