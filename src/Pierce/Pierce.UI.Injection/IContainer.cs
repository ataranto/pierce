namespace Pierce.UI.Injection
{
    public interface IContainer
    {
        T Get<T>() where T : class;
        Syntax<TView> View<TView>() where TView : class, IView;
        Syntax<TView> View<TView>(TView view) where TView : IView;
    }
}

