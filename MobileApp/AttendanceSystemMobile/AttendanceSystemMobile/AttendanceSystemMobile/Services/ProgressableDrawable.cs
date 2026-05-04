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
            float stroke = 15;
            float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - stroke;

            PointF center = new(dirtyRect.Center.X, dirtyRect.Center.Y);

            // background
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = stroke;
            canvas.DrawCircle(center, radius);

            if (_progress <= 0) return;

            // progress arc
            canvas.StrokeColor = Colors.Green;

            float sweepAngle = 360f * _progress;

            canvas.DrawArc(
                dirtyRect.Center.X - radius,
                dirtyRect.Center.Y - radius,
                radius * 2,
                radius * 2,
                -90,
                sweepAngle,
                false,
                false);
        }
    }
}