namespace Pierce.Injection
{
    public interface IContainer
    {
        T Get<T>() where T : class;
        Syntax<TView> GetView<TView>() where TView : class;
    }
}

