namespace Pierce.Example.Console
{
    public class ConsoleDateTimeView : IDateTimeView
    {
        public void SetValue(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}

