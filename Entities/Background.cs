using System;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Entities
{
    public class Background : Entity
    {
        private Rectangle SourceRect;
        public Vector2 Size { get; private set; }
        public int Speed { get; private set; } = 60;
        
        public float MoveY { get; set; } = 0f;
        private Camera camera;

        public Background()
        {
            

        }

        public  void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera)
        {
            this.camera = camera;
            base.Initialize(spriteBatch, contentManager);
        }

        public override void Load()
        {

            this.texture = this.contentManager.Load<Texture2D>("desert-backgorund-looped");
            this.MoveY = 304f;

            this.SourceRect = new Rectangle(0, (int)this.MoveY, 256, 304);
            this.Size = this.camera.Size;
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            this.MoveY -= deltatime * Speed;
            if(this.MoveY <= 0.2f)
            {
                this.MoveY = 304;
            }
            this.SourceRect = new Rectangle(0, (int)this.MoveY, this.SourceRect.Width, this.SourceRect.Height);
        }

        public override void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);

            this.spriteBatch.Draw(this.texture,
                destinationRectangle: new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.Size.X, (int)this.Size.Y),
               sourceRectangle: SourceRect,
               Color.White);

            this.spriteBatch.End();
        }

        public override void Reset()
        {
            this.MoveY = 0;
            this.Speed = 60;
        }
    }
}
