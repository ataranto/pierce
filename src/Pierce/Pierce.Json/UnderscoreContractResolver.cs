using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace Pierce.Json
{
    public class UnderscoreContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return Regex.Replace(propertyName, "([a-z])([A-Z])", "$1_$2").
                ToLowerInvariant();
        }
    }
}
