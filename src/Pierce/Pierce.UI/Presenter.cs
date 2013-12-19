using Pierce.UI.Injection;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Pierce.UI
{
    public abstract class Presenter<TView> : IDisposable
    {
        private readonly CompositeDisposable _disposables =
            new CompositeDisposable();

        public IContainer Container { protected get; set; }
        public IScheduler UIScheduler { protected get; set; }
        public TView View { protected get; set; }
        public abstract void Initialize();

        public virtual void Dispose()
        {
            _disposables.Dispose();

            Container = null;
            UIScheduler = null;
            View = default(TView);
        }

        protected void AddDisposables(IEnumerable<IDisposable> disposables)
        {
            foreach (var disposable in disposables)
            {
                _disposables.Add(disposable);
            }
        }
    }

    public abstract class Presenter<TModel, TView> : Presenter<TView>
    {
        public TModel Model { protected get; set; }

        public override void Dispose()
        {
            base.Dispose();
            Model = default(TModel);
        }
    }
}

