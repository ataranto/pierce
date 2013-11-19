namespace Pierce.Net
{
    public interface INetwork
    {
        NetworkResponse Execute(Request request);
    }
}
