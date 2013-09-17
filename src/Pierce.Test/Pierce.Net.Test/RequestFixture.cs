using System;
using Xunit;
using Pierce.Test;

namespace Pierce.Net.Test
{
    public class RequestFixture
    {
        [Fact]
        public void GetResultAsync_WhenNotAddedToQueue_ThrowsException()
        {
            var request = new StringRequest();
            Assert.Throws<InvalidOperationException>(() => request.GetResultAsync());
        }
    }
}

