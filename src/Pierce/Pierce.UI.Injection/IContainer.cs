namespace Pierce.UI.Injection
{
    public interface IContainer
    {
        T Get<T>() where T : class;
        Syntax<TView> GetView<TView>() where TView : class, IView;
        Syntax<TView> GetView<TView>(TView view) where TView : IView;
    }
}

