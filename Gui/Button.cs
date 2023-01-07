using System;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Gui
{
    public class Button : Entity
    {
        private Rectangle[] defaultRects;
        private Rectangle[] pressedRects;
        private Rectangle[] defaultRenderRect;
        private Rectangle[] pressedRenderRect;

        private bool isPressed = false;
        public Vector2 Size { get; private set; }
        private Vector2[] textPosition;
        private Color buttonFilter = Color.White;

        private SpriteFont font;
        public string text { get; private set; } = string.Empty;
        private Camera camera;
        private bool wasClick;
        public float fontScale = 0.8f;
        public Button(Vector2 position, string text, float scale)
        {
            this.Set(position, text, scale);
        }

        public Button()
        {

        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, SpriteFont font, Camera camera)
        {
            this.camera = camera;
            this.font = font;
            base.Initialize(spriteBatch, contentManager);
        }

        public override void Load()
        {
            this.texture = this.contentManager.Load<Texture2D>("button");
        }

        public void AutoSet(Vector2 position, string text, float scale, bool buttonCenterX = false)
        {
            float buttonWidth = this.font.MeasureString(text).X * this.fontScale;
            this.Set(position, text, scale, buttonCenterX, 0, buttonWidth);
        }

        public void ChangeColor(Color c)
        {
            this.buttonFilter = c;
        }

        public void Set(Vector2 position, string text, float scale, bool buttonCenterX = false, int textMarginX = 0, float buttonWidth = 20)
        {
            if (buttonCenterX)
            {
                this.position = position - new Vector2((buttonWidth + (2 * 5 * scale))/2f, 0);
            }
            else
            {
                this.position = position;
            }

            this.text = text;

            this.defaultRects = new Rectangle[3];
            this.defaultRects[0] = new Rectangle(0, 0, 5, 14);
            this.defaultRects[1] = new Rectangle(5, 0, 10, 14);
            this.defaultRects[2] = new Rectangle(15, 0, 5, 14);

            this.defaultRenderRect = new Rectangle[3];
            this.defaultRenderRect[0] = new Rectangle((int)this.position.X, (int)this.position.Y, (int)(5 * scale), (int)(14 * scale));
            this.defaultRenderRect[1] = new Rectangle(this.defaultRenderRect[0].X + this.defaultRenderRect[0].Width, (int)this.position.Y, (int)buttonWidth, (int)(14 * scale));
            this.defaultRenderRect[2] = new Rectangle(this.defaultRenderRect[1].X + this.defaultRenderRect[1].Width, (int)this.position.Y, this.defaultRenderRect[0].Width, this.defaultRenderRect[0].Height);

            this.Size = new Vector2(this.defaultRenderRect[0].Width * 2 + this.defaultRenderRect[1].Width, this.defaultRenderRect[0].Height);

            this.pressedRects = new Rectangle[3];
            this.pressedRects[0] = new Rectangle(0, 14, 5, 13);
            this.pressedRects[1] = new Rectangle(5, 14, 10, 13);
            this.pressedRects[2] = new Rectangle(15, 14, 5, 13);

            int pixelPressedMargin = (int)scale;

            this.pressedRenderRect = new Rectangle[3];
            this.pressedRenderRect[0] = new Rectangle((int)this.position.X, (int)this.position.Y + pixelPressedMargin, (int)(5 * scale), (int)(13 * scale));
            this.pressedRenderRect[1] = new Rectangle(this.pressedRenderRect[0].X + this.pressedRenderRect[0].Width, (int)this.position.Y + pixelPressedMargin, (int)buttonWidth, (int)(13 * scale));
            this.pressedRenderRect[2] = new Rectangle(this.pressedRenderRect[1].X + this.pressedRenderRect[1].Width, (int)this.position.Y + pixelPressedMargin, this.pressedRenderRect[0].Width, this.pressedRenderRect[0].Height);

            int textMargin = 4;
            this.textPosition = new Vector2[2];
            this.textPosition[0] = this.position + new Vector2((5 + textMarginX) * scale, textMargin);
            this.textPosition[1] = this.position + new Vector2((5 + textMarginX) * scale, textMargin + pixelPressedMargin);
        }

        public bool isClick()
        {
            bool val = this.wasClick;
            if(val)
            {
                this.wasClick = false;
            }

            return val;
        }

        
        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            foreach (TouchLocation touch in touchLocations)
            {
                Vector2 relpos = this.camera.Point(touch.Position);
                if (this.isPressed && touch.State == TouchLocationState.Released)
                {
                    if (this.CheckCollision(relpos))
                    {
                        this.isPressed = false;
                        this.wasClick = true;
                        break;
                    }
                    else
                    {
                        this.isPressed = false;
                        break;
                    }
                }
                else
                {
                    if (touch.State == TouchLocationState.Pressed)
                    {
                        
                        if(this.CheckCollision(relpos))
                        {
                            this.isPressed = true;
                            break;
                        }
                    }
                }
            }
        }

        public bool CheckCollision(Vector2 touchPosition)
        {
            Vector2 touchSize = new Vector2(10, 10);

            if (touchPosition.X < this.position.X + this.Size.X && touchPosition.X + touchSize.X > this.position.X && touchPosition.Y < this.position.Y + this.Size.Y && touchPosition.Y + touchSize.Y > this.position.Y)
            {
                return true;
            }

            return false;
        }

        public override void Draw()
        {



            if (this.isPressed)
            {
                this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
                this.spriteBatch.Draw(this.texture,
                  destinationRectangle: this.pressedRenderRect[0],
                  sourceRectangle: this.pressedRects[0],
                  this.buttonFilter);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.pressedRenderRect[1],
                sourceRectangle: this.pressedRects[1],
               this.buttonFilter);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.pressedRenderRect[2],
                sourceRectangle: this.pressedRects[2],
               this.buttonFilter);
                this.spriteBatch.End();

                this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
                this.spriteBatch.DrawString(this.font, this.text, this.textPosition[1], Color.White, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
                this.spriteBatch.End();
            }
            else
            {
                this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);

                this.spriteBatch.Draw(this.texture,
                  destinationRectangle: this.defaultRenderRect[0],
                  sourceRectangle: this.defaultRects[0],
                  this.buttonFilter);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.defaultRenderRect[1],
                sourceRectangle: this.defaultRects[1],
                this.buttonFilter);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.defaultRenderRect[2],
                sourceRectangle: this.defaultRects[2],
               this.buttonFilter);
                this.spriteBatch.End();

                this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
                this.spriteBatch.DrawString(this.font, this.text, this.textPosition[0], Color.White, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
                this.spriteBatch.End();
            }


        }
    }
}
