using System;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Gui
{
    public class Label : Entity
    {

        private SpriteFont font;
        public string text { get; private set; } = string.Empty;
        private Camera camera;

        public float fontScale = 0.8f;
        public Label(Vector2 position, string text, bool centerX)
        {
            this.Set(position, text, centerX);
        }

        public Label()
        {

        }

        public void Set(Vector2 position, string text, bool centerX)
        {
            this.text = text;
            if (centerX)
            {
                Vector2 size = this.font.MeasureString(this.text) * this.fontScale;
                this.position = position - new Vector2(size.X/2, 0f);
            }
            else
            {
                this.position = position;
            }
       
        }
        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, SpriteFont font, Camera camera)
        {
            this.camera = camera;
            this.font = font;
            base.Initialize(spriteBatch, contentManager);
        }

        public override void Load()
        {
            
        }

        public override void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
            this.spriteBatch.DrawString(this.font, this.text, this.position, Color.White, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
            this.spriteBatch.End();
        }
    }
}