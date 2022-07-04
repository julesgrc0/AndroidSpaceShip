using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AndroidSpaceShip.Core
{
    public class Camera
    {
        public float Zoom { get; set; } = 1f;
        public Vector2 Position { get; set; } = Vector2.Zero;

        public Vector2 Size { get; set; } = Vector2.Zero;
        public Rectangle Bounds { get; protected set; } = Rectangle.Empty;
        public Rectangle VisibleArea { get; protected set; } = Rectangle.Empty;
        public Matrix Transform { get; protected set; }

        public Random Random { get; private set; } = new Random();

        public Camera(Viewport viewport,Vector2 size)
        {
            Bounds = viewport.Bounds;
            Size = size;
        }

        public void Shake(int force)
        {
            int val = this.Random.Next(-force, force);
            this.Position += new Vector2(val, val);

            this.UpdateMatrix();
        }

        public void ResetPosition()
        {
            this.Position = Vector2.Zero;

            this.UpdateMatrix();
        }

        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);
            
            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public void UpdateMatrix()
        {
            Transform = 
                Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) 
                * Matrix.CreateScale(Bounds.Width / Size.X, Bounds.Height / Size.Y, 1f) 
                * Matrix.CreateScale(Zoom);
            
            //* Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0)); (center by default)
            UpdateVisibleArea();
        }

        public void ChangeViewport(Viewport viewport)
        {
            this.Bounds = viewport.Bounds;
            this.UpdateMatrix();
        }

        public Vector2 Point(in Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(Transform);
            return Vector2.Transform(point, invertedMatrix);
        }
    }
}
