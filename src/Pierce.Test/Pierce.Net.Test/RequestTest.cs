using Pierce.Test;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pierce.Net.Test
{
    public class RequestTest
    {
        [Fact]
        public async Task GetResultAsync_NotAddedToQueue_ThrowsException()
        {
            var request = new StringRequest();
            await TaskAssert.ThrowsAsync<InvalidOperationException>(async () =>
                await request.GetResponseAsync());
        }
    }
}

