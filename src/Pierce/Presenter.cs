using System;

namespace Pierce
{
    public abstract class Presenter<TView> : IDisposable
    {
        public TView View { protected get; set; }
        public abstract void Initialize();
        public abstract void Dispose();
    }

    public abstract class Presenter<TModel, TView> : Presenter<TView>
    {
        public TModel Model { protected get; set; }
    }
}

