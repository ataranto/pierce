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

            DrawHand(path, center, GetRadians(date_time.Hour, 24), radius * hour_hand_radius);
            DrawHand(path, center, GetRadians(date_time.Minute, 60), radius * minute_hand_radius);
            DrawHand(path, center, GetRadians(date_time.Second, 60), radius);
        }

        private static double GetRadians(double numerator, double denominator)
        {
            return
                Math.PI *
                (numerator / denominator) /
                180.0 * 360.0;
        }

        private static void DrawHand(NSBezierPath path, PointF center, double radians, float radius)
        {
            path.MoveTo(center);
            path.LineTo(new PointF
            {
                X = center.X + (float)Math.Sin(radians) * radius,
                Y = center.Y + (float)Math.Cos(radians) * radius,
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

