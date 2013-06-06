using SimpleInjector;

namespace Pierce.Example.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var container = new SimpleInjector.Container();
            container.Register<IDateTime, DateTime>(Lifestyle.Singleton);
            container.Register<IDateTimeView, ConsoleDateTimeView>();
            container.Verify();

            new SimpleInjectorContainer(container).
                GetView<IDateTimeView>().
                WithModel<IDateTime>().
                WithPresenter<DateTimePresenter>();

            System.Console.ReadLine();
        }
    }
}
