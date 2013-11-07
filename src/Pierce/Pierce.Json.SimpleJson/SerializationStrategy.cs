using System.Text.RegularExpressions;

namespace Pierce.Json.SimpleJson
{
    class SerializationStrategy : PocoJsonSerializerStrategy
    {
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            // https://github.com/jonnii/chinchilla/blob/master/src/Chinchilla.Api/RabbitJsonSerializerStrategy.cs
            return Regex.Replace(clrPropertyName, "([a-z])([A-Z])", "$1_$2").ToLower();
        }
    }
}
