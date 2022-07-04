using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AndroidSpaceShip.Core;
using AndroidSpaceShip.Gui;
using Microsoft.Xna.Framework.Input.Touch;
using AndroidSpaceShip.Entities;

namespace AndroidSpaceShip.Screens
{
    public class GameOver : Screen
    {
        private Background background = new Background();
        private Button restart = new Button();
        private Button home = new Button();

        public Camera camera;
        public float backgroundY;

        private Vector2 textpos;

        private float textSize = 0.2f;
        private float targetSize = 0f;

        public int score;
        private float scoreAnim = 0f;

        public int kill;

        public float time;


        public GameOver(GameRoot root) : base(root)
        {
            this.camera = new Camera(this.game.GraphicsDevice.Viewport, this.game.cameraViewSize);
            this.camera.UpdateMatrix();
        }

        public override void Load()
        {
            this.background.Initialize(this.game.spriteBatch, this.game.Content, this.camera);
            this.background.Load();

            this.background.MoveY = this.backgroundY;
            this.targetSize = this.game.FontScale * 3;

            this.CreateButton(this.home, "Retour au menu",  this.camera.Size/2 + new Vector2(0, 160f));
            this.CreateButton(this.restart, "Rejouer", this.camera.Size/2 + new Vector2(0, 80f));
        }

        private bool AnimationInit()
        {
            return this.textSize <= this.targetSize;
        }

        public override int Update(float deltatime, TouchCollection touchLocations)
        {
            if (this.AnimationInit())
            {
                this.textSize += deltatime/2f;
            }
            else
            {
                if(this.scoreAnim < this.score)
                {
                    
                    this.scoreAnim += deltatime * 1000f;
                    if(this.scoreAnim < this.score * 0.95f)
                    {
                        this.scoreAnim = this.score * 0.95f;
                    }

                    if (this.scoreAnim >= this.score)
                    {
                        this.scoreAnim = this.score;
                    }
                }
            }

            textpos = (this.camera.Size / 2) - ((this.game.font.MeasureString("GameOver") * this.textSize) / 2) - new Vector2(0, 200f);

            this.home.Update(deltatime, touchLocations);
            this.restart.Update(deltatime, touchLocations);

            if(this.home.isClick())
            {
                return (int)GameScreens.HOME;
            }

            if(this.restart.isClick())
            {
                return (int)GameScreens.DEFAULT_GAME;
            }

            return (int)GameScreens.NONE;
        }

        private void CreateButton(Button button, string text, Vector2 position)
        {
            button.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            button.Load();

            button.fontScale = this.game.FontScale;
            float width = 35 * this.game.GuiScale;

            float textWidth = this.game.font.MeasureString(text).X * this.game.FontScale;
            int textX = (int)(((width - textWidth) / 2f) / this.game.GuiScale);

            button.Set(position, text, this.game.GuiScale, true, textX, width);
        }

        public override void Draw()
        {
            this.background.Draw();

            this.game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, null, null, null, this.camera.Transform);
            this.game.spriteBatch.DrawString(this.game.font, "GameOver", textpos, Color.White, 0, Vector2.Zero, this.textSize, SpriteEffects.None, 0f);
            
            if(!this.AnimationInit())
            {
                this.game.spriteBatch.DrawString(this.game.font, $"Score {(int)this.scoreAnim}", textpos + new Vector2(0, 100), Color.White, 0, Vector2.Zero, this.game.FontScale, SpriteEffects.None, 0f);
                this.game.spriteBatch.DrawString(this.game.font, $"Kill {this.kill}", textpos + new Vector2(0,150), Color.White, 0, Vector2.Zero, this.game.FontScale, SpriteEffects.None, 0f);
                this.game.spriteBatch.DrawString(this.game.font, $"Temps {this.time} s", textpos + new Vector2(0, 200) , Color.White, 0, Vector2.Zero, this.game.FontScale, SpriteEffects.None, 0f);
            }

            this.game.spriteBatch.End();

            this.restart.Draw();
            this.home.Draw();
        }

        public override void Reset()
        {
            backgroundY = 0f;
            textSize = 0.2f;
            score = 0;
            scoreAnim = 0f;
            kill = 0;
            time = 0f;
        }
    }
}