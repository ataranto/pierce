using System;
using System.Threading.Tasks;
using Xunit;

namespace Pierce.Net.Test
{
    public class RequestFixture
    {
        [Fact]
        public async Task GetResultAsync_NotAddedToQueue_ThrowsException()
        {
            var request = new StringRequest();
            await Pierce.Test.Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await request.GetResultAsync());
        }
    }
}

