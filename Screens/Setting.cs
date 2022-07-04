using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AndroidSpaceShip.Core;
using AndroidSpaceShip.Gui;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System;

namespace AndroidSpaceShip.Screens
{
    public class Setting : Screen
    {

        private Button backHome = new Button();

        private Label lbAudio = new Label();
        private Label lbAspectRatio = new Label();
        private Label lbVib = new Label();
        

        private Select resolutionSelect = new Select();
        private List<Tuple<string, int, int>> resolutionOptions = new List<Tuple<string, int, int>>();
        private float[] aspectRatios;

        private Select audioSelect = new Select();
        private List<Tuple<string, int, int>> audioOptions = new List<Tuple<string, int, int>>();

        private Select vibSelect = new Select();
        private List<Tuple<string, int, int>> vibOptions = new List<Tuple<string, int, int>>();

        private Texture2D background;

        public Camera camera;
        public Setting(GameRoot root) : base(root)
        {
            this.camera = new Camera(this.game.GraphicsDevice.Viewport, this.game.cameraViewSize);
            this.camera.UpdateMatrix();
        }

        private void CreateButton(Button button, string text, Vector2 position, int textX = 0)
        {
            button.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            button.Load();

            button.fontScale = this.game.FontScale;
            button.AutoSet(position, text, this.game.GuiScale, true);
        }

        public override void Load()
        {
            Vector2 backButtonPos = this.camera.Size / 2 + new Vector2(0, 200f);
            this.CreateButton(this.backHome, "Appliquer & Quitter", backButtonPos, 6);


            this.audioSelect.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            this.resolutionSelect.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            this.vibSelect.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);

            this.lbAspectRatio.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            this.lbAudio.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            this.lbVib.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);

            this.audioSelect.Load();
            this.resolutionSelect.Load();
            this.vibSelect.Load();

            this.lbAspectRatio.Load();
            this.lbAudio.Load();
            this.lbVib.Load();

            this.audioSelect.fontScale = this.game.FontScale;
            this.resolutionSelect.fontScale = this.game.FontScale;
            this.vibSelect.fontScale = this.game.FontScale;

            this.lbAspectRatio.fontScale = this.game.FontScale;
            this.lbAudio.fontScale = this.game.FontScale;
            this.lbVib.fontScale = this.game.FontScale;

            this.aspectRatios = new float[4];
            this.aspectRatios[0] = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;
            this.aspectRatios[1] = 4/3f;
            this.aspectRatios[2] = 16/9f;
            this.aspectRatios[3] = 16/10f;

            resolutionOptions.Add(new Tuple<string, int, int>("natif", 10, 5));
            resolutionOptions.Add(new Tuple<string, int, int>("4:3", 10, 5));
            resolutionOptions.Add(new Tuple<string, int, int>("16:9", 10, 5));
            resolutionOptions.Add(new Tuple<string, int, int>("16:10", 10, 5));

            int aspRtIndex = 0;
            for (aspRtIndex = 0; aspRtIndex < this.aspectRatios.Length; aspRtIndex++)
            {
                if (this.aspectRatios[aspRtIndex] == this.game.settings.aspectRatio)
                {
                    break;
                }
            }

            Vector2 resolutionPos = this.camera.Size / 2 - new Vector2(0, 150f);
            this.resolutionSelect.Set(resolutionPos, this.resolutionOptions, this.game.GuiScale, aspRtIndex, true);

            Vector2 labelResPos = this.camera.Size / 2 - new Vector2(0, 150f + this.resolutionSelect.Size.Y);
            this.lbAspectRatio.Set(labelResPos, "Aspect Ratio", true);
            
            audioOptions.Add(new Tuple<string, int, int>("On", 10, 6));
            audioOptions.Add(new Tuple<string, int, int>("Off", 10, 6));

            int audioIndex = this.game.settings.audio ? 0 : 1;

            Vector2 audioPos = this.camera.Size / 2 - new Vector2(0, 50f);
            this.audioSelect.Set(audioPos, this.audioOptions, this.game.GuiScale, audioIndex, true);

            Vector2 labelAudioPos = this.camera.Size / 2 - new Vector2(0, 50f + this.audioSelect.Size.Y);
            this.lbAudio.Set(labelAudioPos, "Musique & Effets Sonores", true);


            vibOptions.Add(new Tuple<string, int, int>("On", 10, 6));
            vibOptions.Add(new Tuple<string, int, int>("Off", 10, 6));

            int vibIndex = this.game.settings.vibration ? 0 : 1;

            Vector2 vibPos = this.camera.Size / 2 + new Vector2(0, 45f);
            this.vibSelect.Set(vibPos, this.vibOptions, this.game.GuiScale, vibIndex, true);

            Vector2 labelVibPos = this.camera.Size / 2 + new Vector2(0, 5f);
            this.lbVib.Set(labelVibPos, "Vibrations", true);


            this.background = new Texture2D(this.game.graphicsDevice.GraphicsDevice, (int)this.camera.Size.X, (int)this.camera.Size.Y);
            Color[] data = new Color[this.background.Width * this.background.Height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color(38, 43, 68);
            }
            this.background.SetData(data);
        }

        public override int Update(float deltatime, TouchCollection touchLocations)
        {
            this.backHome.Update(deltatime, touchLocations);

            this.resolutionSelect.Update(deltatime, touchLocations);
            this.audioSelect.Update(deltatime, touchLocations);
            this.vibSelect.Update(deltatime, touchLocations);

            if(this.resolutionSelect.HasChange())
            {
                int index = this.resolutionSelect.GetIndex();
                int[] sizes = this.game.calcAspectRatio(this.game.GraphicsDevice.Viewport.Width, this.aspectRatios[index]);

                this.game.ChangeViewport(sizes);
                this.game.settings.aspectRatio = this.aspectRatios[index];
                this.game.storage.Write(this.game.settings);
            }

            if(this.audioSelect.HasChange())
            {
                bool active = this.audioSelect.GetIndex() == 1 ? false : true;
                this.game.settings.audio = active;
                this.game.storage.Write(this.game.settings);
            }

            if(this.vibSelect.HasChange())
            {
                bool active = this.vibSelect.GetIndex() == 1 ? false : true;
                this.game.settings.vibration = active;
                this.game.storage.Write(this.game.settings);
            }

            if(this.backHome.isClick())
            {
                return (int)GameScreens.HOME;
            }

            return (int)GameScreens.NONE;
        }

        public override void Draw()
        {
            this.game.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
            this.game.spriteBatch.Draw(this.background, this.background.Bounds, Color.White);
            this.game.spriteBatch.End();

            this.vibSelect.Draw();
            this.audioSelect.Draw();
            this.resolutionSelect.Draw();
            this.backHome.Draw();

            this.lbAspectRatio.Draw();
            this.lbAudio.Draw();
            this.lbVib.Draw();
        }
    }
}