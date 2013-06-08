using System;
using Pierce.Example.Views;
using MonoMac.AppKit;
using System.Drawing;

namespace Pierce.Example.Mac.Views
{
    public class AnalogDateTimeView : NSView, IDateTimeView
    {
        private const float hour_hand_radius = 0.6f;
        private const float minute_hand_radius = 0.8f;
        private const float second_hand_radius = 0.9f;

        private DateTime date_time = DateTime.Now;

        public AnalogDateTimeView()
        {
        }

        public void SetValue(DateTime value)
        {
            date_time = value;
            SetNeedsDisplayInRect(Bounds);

            Console.WriteLine(value);
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);

            NSBezierPath path = new NSBezierPath
            {
                LineWidth = 1,
            };

            var square = GetSquare(Bounds, 6);
            path.AppendPathWithOvalInRect(square);

            NSColor.Black.SetStroke();
            path.Stroke();

            var radius = square.Width / 2;
            var center = new PointF
            {
                X = Bounds.Width / 2,
                Y = Bounds.Height / 2,
            };

            DrawHand(path, center, (float)date_time.Hour / 24, radius);
            DrawHand(path, center, (float)date_time.Minute / 60, radius);
            DrawHand(path, center, (float)date_time.Second / 60, radius);

        }

        private static void DrawHand(NSBezierPath path, PointF center, float angle, float radius)
        {
            path.MoveTo(center);
            path.LineTo(new PointF
            {
                X = center.X + (float)Math.Cos(angle) * radius,
                Y = center.Y + (float)Math.Sin(angle) * radius,
            });
            path.Stroke();
        }

        private static RectangleF GetSquare(RectangleF rectangle, float border = 0)
        {
            var side_length = Math.Min(rectangle.Width, rectangle.Height);

            var x = rectangle.Width - side_length;
            x -= x / 2;

            var y = rectangle.Height - side_length;
            y -= y / 2;

            return new RectangleF
            {
                X = x + border,
                Y = y + border,
                Width = side_length - border * 2,
                Height = side_length - border * 2,
            };
        }
    }
}

