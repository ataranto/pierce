using System;

namespace Pierce.UI
{
    public abstract class Presenter<TView> : IDisposable
    {
        public TView View { protected get; set; }
        public abstract void Initialize();

        public virtual void Dispose()
        {

        }
    }

    public abstract class Presenter<TModel, TView> : Presenter<TView>
    {
        public TModel Model { protected get; set; }
    }
}

