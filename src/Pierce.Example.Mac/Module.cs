using System;
using MonoMac.Foundation;
using Ninject.Modules;
using Pierce.Example.Containers;
using Pierce.Example.Models;
using Pierce.Example.Views;
using Pierce.Example.Mac.Views;
using Pierce.Injection;

namespace Pierce.Example.Mac
{
    public class Module : NinjectModule
    {
        private readonly NSObject _ns_object;

        public Module(NSObject ns_object)
        {
            _ns_object = ns_object;
        }

        public override void Load()
        {
            Action<Action> invoke = x =>
                _ns_object.BeginInvokeOnMainThread(new NSAction(() => x()));

            Bind<IContainer>().
                To<NinjectContainer>().
                InSingletonScope();
            Bind<Action<Action>>().
                ToConstant(invoke);

            Bind<IDateTimeModel>().
                To<UtcDateTimeModel>().
                InSingletonScope();
            Bind<IDateTimeView>().
                To<DigitalDateTimeView>().
                InSingletonScope();
        }
    }
}

