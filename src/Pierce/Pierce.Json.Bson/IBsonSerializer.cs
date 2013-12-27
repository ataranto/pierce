namespace Pierce.Json.Bson
{
    public interface IBsonSerializer
    {
        byte[] Serialize(object @object);
        T Deserialize<T>(byte[] data);
    }
}
