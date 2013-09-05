using MonoMac.Foundation;
using MonoMac.AppKit;
using Pierce.Example.Models;
using Pierce.Example.Views;
using Pierce.Example.Mac.Views;
using Pierce.Example.Presenters;
using System;
using Ninject;
using Pierce.Injection;

namespace Pierce.Example.Mac
{
    public partial class AppDelegate : NSApplicationDelegate
    {
        MainWindowController mainWindowController;


        public override void FinishedLaunching(NSObject notification)
        {
            var module = new Module(this);
            var kernel = new StandardKernel(module);
            var container = kernel.Get<IContainer>();

            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(this);

            var vertical_box = new VerticalBox();
            var horizontal_box = new HorizontalBox();

            NSButton button;

            button = CreateButton("Model: Utc | View: Digital", delegate
            {
                var view = container.
                    GetView<DigitalDateTimeView>().
                    WithModel<IDateTimeModel>().
                    WithPresenter<IDateTimeView, DateTimePresenter>().
                    ToView();

                vertical_box.AddSubview(view, 100);
                vertical_box.Update();
            });
            horizontal_box.AddSubview(button);

            button = CreateButton("Model: Utc | View: Analog", delegate
            {
                var view = container.
                    GetView<AnalogDateTimeView>().
                    WithModel<IDateTimeModel>().
                    WithPresenter<IDateTimeView, DateTimePresenter>().
                    ToView();

                vertical_box.AddSubview(view, 100);
                vertical_box.Update();
            });
            horizontal_box.AddSubview(button);

            button = CreateButton("Model: Local | View: Digital", (sender, e) =>
            {
                var model = new LocalDateTimeModel();
                var view = container.
                    GetView<DigitalDateTimeView>().
                    WithModel<IDateTimeModel>(model).
                    WithPresenter<IDateTimeView, DateTimePresenter>().
                    ToView();

                vertical_box.AddSubview(view, 100);
                vertical_box.Update();
            });
            horizontal_box.AddSubview(button);

            button = CreateButton("Model: Local | View: Analog", delegate
            {
                var model = new LocalDateTimeModel();
                var view = container.
                    GetView<AnalogDateTimeView>().
                    WithModel<IDateTimeModel>(model).
                    WithPresenter<IDateTimeView, DateTimePresenter>().
                    ToView();

                vertical_box.AddSubview(view, 100);
                vertical_box.Update();
            });
            horizontal_box.AddSubview(button);

            vertical_box.AddSubview(horizontal_box, button.Frame.Height);
            mainWindowController.Window.ContentView = vertical_box;
        }

        private static NSButton CreateButton(string title, EventHandler handler)
        {
            var button = new NSButton
            {
                Title = title,
            };

            button.Activated += handler;
            button.SizeToFit();
            return button;
        }
    }
}

