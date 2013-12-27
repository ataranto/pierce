using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Pierce.Json
{
    public class DefaultSerializerSettings : JsonSerializerSettings
    {
        public DefaultSerializerSettings(IContractResolver contract_resolver = null)
        {
            NullValueHandling = NullValueHandling.Ignore;
            ContractResolver = contract_resolver ?? new UnderscoreContractResolver();
        }
    }
}
