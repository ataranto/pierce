namespace Pierce.Logging
{
    public interface ILogManager
    {
        ILogger GetLogger(string name = null);
    }
}
