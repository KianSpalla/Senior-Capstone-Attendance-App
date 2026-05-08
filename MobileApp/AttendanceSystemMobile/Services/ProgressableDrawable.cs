using Microsoft.Maui.Graphics;

namespace AttendanceSystemMobile.Services
{
    public class CircleProgressDrawable : IDrawable
    {
        private float _progress = 0f;

        public float Progress
        {
            get => _progress;
            set => _progress = Math.Clamp(value, 0f, 1f);
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.Antialias = true;

            float stroke = 15;

            float radius =
                Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - stroke;

            PointF center =
                new(dirtyRect.Center.X, dirtyRect.Center.Y);

            // Background
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = stroke;

            canvas.DrawCircle(center, radius);

            if (_progress <= 0)
                return;
            // Progress
            canvas.StrokeColor = Colors.Green;
            canvas.StrokeSize = stroke;
            canvas.StrokeLineCap = LineCap.Round;

            if (_progress >= 1f)
            {
                // Full circle
                canvas.DrawCircle(center, radius);
            }
            else
            {
                float endAngle = -90 + (360f * _progress);

                canvas.DrawArc(
                    center.X - radius,
                    center.Y - radius,
                    radius * 2,
                    radius * 2,
                    -90,
                    endAngle,
                    false,
                    false);
            }
        }
    }
}
