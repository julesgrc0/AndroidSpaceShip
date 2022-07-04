using System;
using System.Collections.Generic;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Gui
{
    public class Select : Entity
    {
        private Rectangle[] sourceRects;
        private List<Tuple<Rectangle, Rectangle>> renderRect = new List<Tuple<Rectangle, Rectangle>>();
        private List<Tuple<string, Vector2>> optionList = new List<Tuple<string, Vector2>>();
        
        public Vector2 Size { get; private set; }
        
        private SpriteFont font;
        private Camera camera;
        private int selectedIndex = -1;
        private bool change = false;

        public float fontScale = 0.8f;

        public Select(Vector2 position, List<Tuple<string, int, int>> options, float scale,int selectedIndex)
        {
            this.Set(position, options, scale, selectedIndex);
        }

        public Select()
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
            this.texture = this.contentManager.Load<Texture2D>("select");
        }

        public int GetIndex()
        {
            return this.selectedIndex;
        }

        public bool HasChange()
        {
            bool val = this.change;
            if(this.change)
            {
                this.change = false;
            }

            return val;
        }
        public void Set(Vector2 position, List<Tuple<string, int, int>>/* text ; btn margin ; text margin */ options, float scale,int selectedIndex, bool selectCenterX = false)
        {
            if (selectCenterX)
            {
                int tmpWidth = (int)(5 * scale * 2);
                int index = 0;
                foreach (Tuple<string, int, int> option in options)
                {
                    tmpWidth += (int)((10 + option.Item2) * scale);

                    if (index + 1 < options.Count)
                    {
                        tmpWidth += (int)scale;
                    }
                    index++;
                }

                this.position = position - new Vector2(tmpWidth/2, 0);
            }
            else
            {
                this.position = position;
            }

            this.selectedIndex = selectedIndex;
            this.sourceRects = new Rectangle[5];

            this.sourceRects[0] = new Rectangle(0, 0, 6, 13); // border
            this.sourceRects[1] = new Rectangle(6, 0, 10, 13); // block
            this.sourceRects[2] = new Rectangle(16, 0, 10, 13); // selected block
            this.sourceRects[3] = new Rectangle(26, 0, 1, 13); // line
            this.sourceRects[4] = new Rectangle(27, 0, 6, 13); // border

            int width = 0;
            
            Tuple<Rectangle, Rectangle> borderLeft = new Tuple<Rectangle, Rectangle>(
                this.sourceRects[0],
                new Rectangle((int)this.position.X, (int)this.position.Y, (int)(5 * scale), (int)(13 * scale)));
            
            this.renderRect.Add(borderLeft);
            width += borderLeft.Item2.Width;
            
            int i = 0;
            float textMarginY = 2 * scale;

            foreach (Tuple<string, int, int> option in options)
            {
                Rectangle source = this.sourceRects[1];
                if(selectedIndex == i)
                {
                    source = this.sourceRects[2];
                }
                Tuple<Rectangle, Rectangle> block = new Tuple<Rectangle, Rectangle>(source,
                    new Rectangle((int)(this.position.X + width), (int)this.position.Y, (int)((10 + option.Item2) * scale), (int)(13 * scale))
                    );

                this.renderRect.Add(block);
                width += block.Item2.Width;

                if(i+1 < options.Count)
                {
                    Tuple<Rectangle, Rectangle> line = new Tuple<Rectangle, Rectangle>(this.sourceRects[3],
                   new Rectangle((int)(this.position.X + width), (int)this.position.Y, (int)scale, (int)(13 * scale))
                   );
                    this.renderRect.Add(line);
                    width += line.Item2.Width;
                }

                
                Vector2 textpos = new Vector2(block.Item2.X + option.Item3 * scale, block.Item2.Y + textMarginY);
                this.optionList.Add(new Tuple<string, Vector2>(option.Item1, textpos));

                i++;
            }

            Tuple<Rectangle, Rectangle> borderRight = new Tuple<Rectangle, Rectangle>(this.sourceRects[4],
                    new Rectangle((int)(this.position.X + width), (int)this.position.Y, (int)(5 * scale), (int)(13 * scale))
                    );
            
            this.renderRect.Add(borderRight);
            width += borderRight.Item2.Width;

            this.Size = new Vector2(width, (int)(13 * scale));
        }

        private void ChangeIndex(int index)
        {
            this.selectedIndex = index;
            for(int i = 0; i < this.renderRect.Count; i++)
            {
                if (this.renderRect[i].Item1 == this.sourceRects[2])
                {
                    this.renderRect[i] = new Tuple<Rectangle,Rectangle>(this.sourceRects[1], this.renderRect[i].Item2);
                    break;
                }
            }

            int select = 2 * (this.selectedIndex) + 1;
            this.renderRect[select] = new Tuple<Rectangle, Rectangle>(this.sourceRects[2], this.renderRect[select].Item2);
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            foreach (TouchLocation touch in touchLocations)
            {
                Vector2 relpos = this.camera.Point(touch.Position);
                if (touch.State == TouchLocationState.Pressed)
                {
                    bool stop = false;
                    for (int i = 1; i < this.renderRect.Count - 1; i += 2)
                    {
                        Vector2 pos = new Vector2(this.renderRect[i].Item2.X, this.renderRect[i].Item2.Y);
                        Vector2 size = new Vector2(this.renderRect[i].Item2.Width, this.renderRect[i].Item2.Height);
                        int index = i / 2;

                        if (this.CheckCollision(relpos, pos, size))
                        {
                            this.ChangeIndex(index);
                            this.change = true;
                            stop = true;
                            break;
                        }
                    }

                    if (stop)
                    {
                        break;
                    }
                }
            }
        }

        public bool CheckCollision(Vector2 touchPosition, Vector2 pos, Vector2 size)
        {
            Vector2 touchSize = new Vector2(10, 10);

            if (touchPosition.X < pos.X + size.X && touchPosition.X + touchSize.X > pos.X && touchPosition.Y < pos.Y + size.Y && touchPosition.Y + touchSize.Y > pos.Y)
            {
                return true;
            }

            return false;
        }

        public override void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);

            foreach (Tuple<Rectangle, Rectangle> rects in this.renderRect)
            {
               
                this.spriteBatch.Draw(this.texture,
                      destinationRectangle: rects.Item2,
                      sourceRectangle: rects.Item1,
                      Color.White);

               
            }

            this.spriteBatch.End();

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
            foreach (Tuple<string, Vector2> text in this.optionList)
            {
                this.spriteBatch.DrawString(this.font, text.Item1, text.Item2, Color.White, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
            }
            this.spriteBatch.End();
           
        }
    }
}
