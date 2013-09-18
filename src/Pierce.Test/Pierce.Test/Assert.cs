using System;
using System.Threading.Tasks;

namespace Pierce.Test
{
    public static class Assert
    {
        public static void Fail(string message = null)
        {
            Xunit.Assert.True(false, message);
        }

        // http://stackoverflow.com/a/14103924/64290
        public static async Task<Exception> ThrowsAsync<T>(Func<Task> func)
        {
            try
            {
                await func();
                Assert.Fail("No exception was thrown");
                return null;
            }
            catch (Exception ex)
            {
                Xunit.Assert.Equal(typeof(T), ex.GetType());
                return ex;
            }
        }
    }
}
