namespace Pierce.Json
{
    public interface IJsonSerializer
    {
        string Serialize(object @object);
        T Deserialize<T>(string json);
    }
}
