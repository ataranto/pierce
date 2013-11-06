using Moq;
using Pierce.Json;
using Pierce.Test;
using Xunit;

namespace Pierce.Net.Test
{
    public class JsonRequestFixture : MoqFixture
    {
        [Fact]
        public void ObjectBodySet_SerializesBody()
        {
            var body =  new { foo = "foo", bar = "bar" };
            var serialized_body = "output";

            var mock_serializer = CreateMock<IJsonSerializer>(MockBehavior.Strict);
            var request = new JsonRequest<int>(mock_serializer.Object);

            mock_serializer.
                Setup(m => m.Serialize(body)).
                Returns(serialized_body);

            request.ObjectBody = new { foo = "foo", bar = "bar" };
            Xunit.Assert.Equal(serialized_body, request.Body);
        }
    }
}
