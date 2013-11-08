using Moq;
using Pierce.Json;
using Pierce.Test;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Xunit;

namespace Pierce.Net.Test
{
    public class JsonRequestTest : MoqFixture
    {
        private readonly Mock<IJsonSerializer> _mock_serializer;
        private readonly JsonRequest<int> _request;

        public JsonRequestTest()
        {
            _mock_serializer = CreateMock<IJsonSerializer>(MockBehavior.Strict);
            _request = new JsonRequest<int>(_mock_serializer.Object);
        }

        [Fact]
        public void ObjectBodySet_SerializesBody()
        {
            var body =  new { foo = "foo", bar = "bar" };
            var serialized_body = "output";

            _mock_serializer.
                Setup(m => m.Serialize(body)).
                Returns(serialized_body);

            _request.Data = new { foo = "foo", bar = "bar" };
            Xunit.Assert.Equal(serialized_body, _request.Body);
        }

        [Fact]
        public void Parse_DeseralizeException_ThrowsParseException()
        {
            var invalid_json = "invalid json";
            var response = new NetworkResponse
            {
                Data = Encoding.UTF8.GetBytes(invalid_json),
                Headers = new WebHeaderCollection(),
            };

            _mock_serializer.
                Setup(m => m.Deserialize<int>(invalid_json)).
                Throws<SerializationException>();

            Xunit.Assert.Throws<ParseException>(() =>
                _request.Parse(response));
        }
    }
}
