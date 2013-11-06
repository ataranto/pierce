using System;
using Xunit;

namespace Pierce.Net.Test
{
    public class RetryPolicyTest
    {
        [Fact]
        public void DefaultConstructor_InitialValuesAreNonZero()
        {
            var retry_policy = new RetryPolicy();
            Assert.True(retry_policy.CurrentRetryCount == 0);
            Assert.True(retry_policy.CurrentTimeoutMs > 0);
        }
    }
}

