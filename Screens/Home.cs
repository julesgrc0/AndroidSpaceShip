using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AndroidSpaceShip.Core;
using AndroidSpaceShip.Gui;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Screens
{
    public class Home : Screen
    {

        public Button startBtn = new Button();
        public Button settingBtn = new Button();
        public Button progression = new Button();

        public Texture2D background;

        public Camera camera;
        public Home(GameRoot root) : base(root)
        {
            this.camera = new Camera(this.game.GraphicsDevice.Viewport, this.game.cameraViewSize);
            this.camera.UpdateMatrix();
        }

        public override void Load()
        {
    

            Vector2 startBtnPos = (this.camera.Size / 2) - new Vector2(0f, this.camera.Size.Y / 10);
            this.CreateButton(this.startBtn,
                "Jouer",
                startBtnPos);
            this.CreateButton(this.progression,
               "Progression", startBtnPos + new Vector2(0f, this.startBtn.Size.Y * 1.5f));
            this.CreateButton(this.settingBtn,
                "Options", startBtnPos + new Vector2(0f, (this.progression.Size.Y + this.startBtn.Size.Y) * 1.5f));


            this.background = new Texture2D(this.game.graphicsDevice.GraphicsDevice, (int)this.camera.Size.X, (int)this.camera.Size.Y);
            Color[] data = new Color[this.background.Width * this.background.Height];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = new Color(38,43,68);
            }
            this.background.SetData(data);
        }

        private void CreateButton(Button button, string text, Vector2 position)
        {
            button.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            button.Load();

            button.fontScale = this.game.FontScale;
            float width = 30 * this.game.GuiScale;

            float textWidth = this.game.font.MeasureString(text).X * this.game.FontScale;
            int textX = (int)(((width - textWidth)/2f)/this.game.GuiScale);

            button.Set(position, text, this.game.GuiScale, true, textX, width);
        }


        public override int Update(float deltatime, TouchCollection touchLocations)
        {
            this.settingBtn.Update(deltatime, touchLocations);
            this.progression.Update(deltatime, touchLocations);
            this.startBtn.Update(deltatime, touchLocations);
            

            if (this.startBtn.isClick())
            {
                return (int)GameScreens.DEFAULT_GAME;
            }

            if(this.settingBtn.isClick())
            {
                return (int)GameScreens.SETTING;
            }

        
            return (int)GameScreens.NONE;
        }

        public override void Draw()
        {
            this.game.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
            this.game.spriteBatch.Draw(this.background, this.background.Bounds, Color.White);
            this.game.spriteBatch.End();
            
            this.startBtn.Draw();
            this.progression.Draw();
            this.settingBtn.Draw();

        }
    }
}