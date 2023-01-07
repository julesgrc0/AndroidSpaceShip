using System;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Gui
{
    public class IconButton : Entity
    {
        private Rectangle[] defaultRects;
        private Rectangle[] pressedRects;
        private Rectangle[] defaultRenderRect;
        private Rectangle[] pressedRenderRect;

        private bool isPressed = false;
        public Vector2 Size { get; private set; }
        

        private Texture2D icon;
        private Vector2 iconSize;
        private Vector2[] iconPosition;

        private Camera camera;
        private bool wasClick;
        public float fontScale = 0.8f;

        public IconButton()
        {

        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera)
        {
            this.camera = camera;
            base.Initialize(spriteBatch, contentManager);
        }

        public override void Load()
        {
            this.texture = this.contentManager.Load<Texture2D>("button");
        }

        public void Set(Vector2 position, Texture2D icon, float scale, bool buttonCenterX = false)
        {

            if (buttonCenterX)
            {
                this.position = position - new Vector2(((14 * scale)) / 2f, 0);
            }
            else
            {
                this.position = position;
            }

            this.icon = icon;
            this.iconSize = new Vector2(14f * scale, 14f * scale);

            this.defaultRects = new Rectangle[3];
            this.defaultRects[0] = new Rectangle(0, 0, 5, 14);
            this.defaultRects[1] = new Rectangle(5, 0, 10, 14);
            this.defaultRects[2] = new Rectangle(15, 0, 5, 14);

            this.defaultRenderRect = new Rectangle[3];
            this.defaultRenderRect[0] = new Rectangle((int)this.position.X, (int)this.position.Y, (int)(5 * scale), (int)(14 * scale));
            this.defaultRenderRect[1] = new Rectangle(this.defaultRenderRect[0].X + this.defaultRenderRect[0].Width, (int)this.position.Y, (int)(4 * scale), (int)(14 * scale));
            this.defaultRenderRect[2] = new Rectangle(this.defaultRenderRect[1].X + this.defaultRenderRect[1].Width, (int)this.position.Y, this.defaultRenderRect[0].Width, this.defaultRenderRect[0].Height);

            this.Size = new Vector2(this.defaultRenderRect[0].Width * 2 + this.defaultRenderRect[1].Width, this.defaultRenderRect[0].Height);

            this.pressedRects = new Rectangle[3];
            this.pressedRects[0] = new Rectangle(0, 14, 5, 13);
            this.pressedRects[1] = new Rectangle(5, 14, 10, 13);
            this.pressedRects[2] = new Rectangle(15, 14, 5, 13);

            int pixelPressedMargin = (int)scale;

            this.pressedRenderRect = new Rectangle[3];
            this.pressedRenderRect[0] = new Rectangle((int)this.position.X, (int)this.position.Y + pixelPressedMargin, (int)(5 * scale), (int)(13 * scale));
            this.pressedRenderRect[1] = new Rectangle(this.pressedRenderRect[0].X + this.pressedRenderRect[0].Width, (int)this.position.Y + pixelPressedMargin, (int)(4 * scale), (int)(13 * scale));
            this.pressedRenderRect[2] = new Rectangle(this.pressedRenderRect[1].X + this.pressedRenderRect[1].Width, (int)this.position.Y + pixelPressedMargin, this.pressedRenderRect[0].Width, this.pressedRenderRect[0].Height);

            this.iconPosition = new Vector2[2];
            this.iconPosition[0] = this.position - (this.iconSize - new Vector2(14f * scale, 12*scale))/2f; 
            this.iconPosition[1] = this.position - (this.iconSize - new Vector2(14f * scale, (12 * scale) + pixelPressedMargin)) / 2f;
        }

        public bool isOperated()
        {
            return this.isPressed;
        }

        public bool isClick()
        {
            bool val = this.wasClick;
            if (val)
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
                        this.wasClick = true;   
                    }
                    this.isPressed = false;
                    break;
                }
                else if (touch.State == TouchLocationState.Pressed && this.CheckCollision(relpos))
                {
                    this.isPressed = true;
                    break;
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
                  Color.White);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.pressedRenderRect[1],
                sourceRectangle: this.pressedRects[1],
                Color.White);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.pressedRenderRect[2],
                sourceRectangle: this.pressedRects[2],
                Color.White);

                this.spriteBatch.Draw(this.icon, new Rectangle((int)this.iconPosition[1].X, (int)this.iconPosition[1].Y, (int)this.iconSize.X, (int)this.iconSize.Y), Color.White);

                this.spriteBatch.End();

            }
            else
            {
                this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);

                this.spriteBatch.Draw(this.texture,
                  destinationRectangle: this.defaultRenderRect[0],
                  sourceRectangle: this.defaultRects[0],
                  Color.White);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.defaultRenderRect[1],
                sourceRectangle: this.defaultRects[1],
                Color.White);

                this.spriteBatch.Draw(this.texture,
                destinationRectangle: this.defaultRenderRect[2],
                sourceRectangle: this.defaultRects[2],
                Color.White);

                this.spriteBatch.Draw(this.icon, new Rectangle((int)this.iconPosition[0].X, (int)this.iconPosition[0].Y, (int)this.iconSize.X, (int)this.iconSize.Y), Color.White);

                this.spriteBatch.End();
            }


        }
    }
}
