using MonoMac.Foundation;
using MonoMac.AppKit;
using Pierce.Example.Containers;
using Pierce.Example.Models;
using Pierce.Example.Views;
using Pierce.Example.Mac.Views;
using Pierce.Example.Presenters;
using SimpleInjector;
using System;

namespace Pierce.Example.Mac
{
    public partial class AppDelegate : NSApplicationDelegate
    {
        MainWindowController mainWindowController;

        public AppDelegate()
        {
        }

        public override void FinishedLaunching(NSObject notification)
        {
            Func<Action<Action>> invoke = () => x =>
                BeginInvokeOnMainThread(new NSAction(() => x()));

            var simple_injector = new SimpleInjectorContainer();
            simple_injector.Container.
                Register<Action<Action>>(invoke, Lifestyle.Singleton);
            simple_injector.Container.
                Register<IDateTimeModel, DateTimeModel>(Lifestyle.Singleton);
            simple_injector.Container.
                Register<IDateTimeView, TextDateTimeView>();

            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(this);

            var model = simple_injector.Get<IDateTimeModel>();

            var view = simple_injector.
                GetView<TextDateTimeView>().
                WithModel(model).
                //WithModel<IDateTimeModel>().
                WithPresenter<IDateTimeView, DateTimePresenter>().
                ToView();
            mainWindowController.Window.ContentView = view;
        }
    }
}

