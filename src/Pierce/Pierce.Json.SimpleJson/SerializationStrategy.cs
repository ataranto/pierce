namespace Pierce.Json.SimpleJson
{
    class SerializationStrategy : PocoJsonSerializerStrategy
    {
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return clrPropertyName.ToLowerInvariant();
        }
    }
}
