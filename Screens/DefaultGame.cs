using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AndroidSpaceShip.Core;
using AndroidSpaceShip.Entities;
using AndroidSpaceShip.Gui;
using Microsoft.Xna.Framework.Input.Touch;


namespace AndroidSpaceShip.Screens
{
    public class DefaultGame : Screen
    {
        public Player player = new Player();
        public Background background = new Background();

        public EnemyManager enemyManager = new EnemyManager();
        public BulletManager bulletManager = new BulletManager();
        public ExplosionManager explosionManager = new ExplosionManager();
        public CloudManager cloudManager = new CloudManager();
        public Camera camera { get; private set; }

        public float time { get; private set; } = 0f;
        public const float enemySpeedFactor = 60 * 2f;
        public float GameSpeed { get; private set; } = 1f;

        private bool dieAnimation = false;
        private float slowDie = 5f;

        private bool paused = false;
        private IconButton pauseBtn = new IconButton();
        private IconButton resumeBtn = new IconButton();
        private Button quitBtn = new Button();

        private Texture2D pauseEffect;

        public DefaultGame(GameRoot root) : base(root)
        {
            this.camera = new Camera(this.game.GraphicsDevice.Viewport, this.game.cameraViewSize);
            this.camera.UpdateMatrix();
        }

        public override void Load()
        {
            this.player.Initialize(this.game.spriteBatch, this.game.Content, this.camera, this.bulletManager);
            this.background.Initialize(this.game.spriteBatch, this.game.Content, this.camera);
            this.enemyManager.Initialize(this.game.spriteBatch, this.game.Content, this.camera, this.explosionManager, this.bulletManager, this.player);
            this.bulletManager.Initialize(this.game.spriteBatch, this.game.Content, this.camera, this.enemyManager, this.player);
            this.explosionManager.Initialize(this.game.spriteBatch, this.game.Content, this.camera);
            this.cloudManager.Initialize(this.game.spriteBatch, this.game.Content, this.camera, this.player);

            this.player.Load();
            this.background.Load();
            this.enemyManager.Load();
            this.bulletManager.Load();
            this.explosionManager.Load();
            this.cloudManager.Load();



            float iconMargin = 10f;
            float iconGuiScale = 1f;

            this.pauseBtn.Initialize(this.game.spriteBatch, this.game.Content, this.camera);
            this.pauseBtn.Load();
            
            Texture2D pauseIcon = this.game.Content.Load<Texture2D>("pause");
            this.pauseBtn.Set(new Vector2(this.camera.Size.X - iconMargin * this.game.GuiScale * iconGuiScale, iconMargin + 10f), pauseIcon, this.game.GuiScale * iconGuiScale, true);


            this.resumeBtn.Initialize(this.game.spriteBatch, this.game.Content, this.camera);
            this.resumeBtn.Load();

            Texture2D resumeIcon = this.game.Content.Load<Texture2D>("resume");
            this.resumeBtn.Set(new Vector2(this.camera.Size.X - iconMargin * this.game.GuiScale * iconGuiScale, iconMargin + 10f), resumeIcon, this.game.GuiScale * iconGuiScale, true);

            Vector2 quitBtnPos = new Vector2(iconMargin, iconMargin + 10f);
            this.CreateButton(this.quitBtn, "Abandonner", quitBtnPos, false);
            this.quitBtn.ChangeColor(Color.Red);

            this.pauseEffect = new Texture2D(this.game.graphicsDevice.GraphicsDevice, (int)this.camera.Size.X, (int)this.camera.Size.Y);
            Color[] data = new Color[this.pauseEffect.Width * this.pauseEffect.Height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color(0, 0, 0, 150);
            }
            this.pauseEffect.SetData(data);
        }

        public override int Update(float deltatime, TouchCollection touchLocations)
        {
            int pauseResult = this.UpdatePauseMode(deltatime, touchLocations);
            if(pauseResult != -1)
            {
                return pauseResult;
            }

            this.time += deltatime;
            if (!this.player.InitAnimation())
            {
                this.background.Update(deltatime, touchLocations);
                this.player.Update(deltatime, touchLocations);
            }

            if (this.player.Alive())
            {
                this.UpdatePlayerAlive(deltatime, touchLocations);
            }
            else if(!this.dieAnimation)
            {
                this.UpdateGameScoreAfterDeath(deltatime, touchLocations);
            }
            else if(this.dieAnimation)
            {
                int animResult = this.UpdatePlayerDeathAnimation(deltatime, touchLocations);
                if(animResult != -1)
                {
                    return animResult;
                }
            }

            return (int)GameScreens.NONE;
        }

        public override void Draw()
        {
            if (!this.dieAnimation)
            {
                this.background.Draw();
                this.cloudManager.Draw();

                this.explosionManager.Draw();
                this.bulletManager.Draw();

                this.player.Draw();
                this.enemyManager.Draw();
            }
            else
            {
                this.background.Draw();
                this.cloudManager.Draw();
                this.explosionManager.Draw();
                this.enemyManager.Draw();
            }



            this.DrawPauseMode();
        }



        private void UpdatePlayerAlive(float deltatime, TouchCollection touchLocations)
        {
            this.camera.ResetPosition();
            this.GameSpeed = (this.time / enemySpeedFactor) + 1;

            this.enemyManager.Speed = this.GameSpeed;
            this.cloudManager.Speed = this.GameSpeed;

            this.background.Update(deltatime, touchLocations);
            if (!pauseBtn.isOperated() && !this.resumeBtn.isOperated())
            {
                this.player.Update(deltatime, touchLocations);
            }
            this.enemyManager.Update(deltatime, touchLocations);

            this.bulletManager.Update(deltatime, touchLocations);
            this.explosionManager.Update(deltatime, touchLocations);
            this.cloudManager.Update(deltatime, touchLocations);
        }

        private void UpdateGameScoreAfterDeath(float deltatime, TouchCollection touchLocations)
        {
            this.camera.ResetPosition();

            this.dieAnimation = true;

            this.bulletManager.Reset();
            this.explosionManager.Reset();

            this.explosionManager.Add(this.player.position);

            this.background.Update(deltatime, touchLocations);
            this.enemyManager.Update(deltatime, touchLocations);
            this.cloudManager.Update(deltatime, touchLocations);

            if (this.player.Kill >= this.game.settings.bestKill)
            {
                this.game.settings.bestKill = this.player.Kill;
                this.game.storage.Write(this.game.settings);
            }

            if (this.time >= this.game.settings.bestTime)
            {
                this.game.settings.bestTime = this.time;
                this.game.storage.Write(this.game.settings);
            }
        }

        private int UpdatePlayerDeathAnimation(float deltatime, TouchCollection touchLocations)
        {
            this.background.Update(deltatime, touchLocations);
            this.enemyManager.Update(deltatime, touchLocations);
            this.cloudManager.Update(deltatime, touchLocations);
            this.explosionManager.Update(deltatime, touchLocations);

            if (this.explosionManager.explosions.Count <= 0)
            {
                this.slowDie -= deltatime * 10;
            }

            if (this.slowDie <= 0)
            {
                return (int)GameScreens.GAMEOVER;
            }

            return -1;
        }

        private int UpdatePauseMode(float deltatime, TouchCollection touchLocations)
        {
            if (this.paused)
            {
                this.resumeBtn.Update(deltatime, touchLocations);
                this.quitBtn.Update(deltatime, touchLocations);

                if (this.resumeBtn.isClick())
                {
                    this.paused = false;
                }

                if (this.quitBtn.isClick())
                {
                    return (int)GameScreens.HOME;
                }

                return (int)GameScreens.NONE;
            }
            else
            {
                this.pauseBtn.Update(deltatime, touchLocations);
                if (this.pauseBtn.isClick())
                {
                    this.camera.ResetPosition();
                    this.paused = true;
                    return (int)GameScreens.NONE;

                }
            }

            return -1;
        }

        private void DrawPauseMode()
        {
            if (this.paused)
            {
                this.game.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
                this.game.spriteBatch.Draw(this.pauseEffect, this.pauseEffect.Bounds, Color.White);
                this.game.spriteBatch.End();

                this.resumeBtn.Draw();
                this.quitBtn.Draw();

                this.game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, null, null, null, this.camera.Transform);

                Vector2 textpos = (this.camera.Size / 2) - new Vector2(80f, 250f);
                this.game.spriteBatch.DrawString(this.game.font, $"Score {(int)((this.player.Kill + 1) * this.time * 10)}", textpos + new Vector2(0, 100), Color.White, 0, Vector2.Zero, this.game.FontScale, SpriteEffects.None, 0f);
                this.game.spriteBatch.DrawString(this.game.font, $"Kill {this.player.Kill}", textpos + new Vector2(0, 150), Color.White, 0, Vector2.Zero, this.game.FontScale, SpriteEffects.None, 0f);
                this.game.spriteBatch.DrawString(this.game.font, $"Temps {Math.Round(this.time, 2)} s", textpos + new Vector2(0, 200), Color.White, 0, Vector2.Zero, this.game.FontScale, SpriteEffects.None, 0f);
                this.game.spriteBatch.DrawString(this.game.font, $"Vitesse {Math.Round(this.GameSpeed, 2)}x", textpos + new Vector2(0, 250), Color.White, 0, Vector2.Zero, this.game.FontScale, SpriteEffects.None, 0f);

                this.game.spriteBatch.End();

            }
            else
            {
                this.pauseBtn.Draw();
            }
        }

        private void CreateButton(Button button, string text, Vector2 position, bool center = true)
        {
            button.Initialize(this.game.spriteBatch, this.game.Content, this.game.font, this.camera);
            button.Load();

            button.fontScale = this.game.FontScale;
            float width = 30 * this.game.GuiScale;

            float textWidth = this.game.font.MeasureString(text).X * this.game.FontScale;
            int textX = (int)(((width - textWidth) / 2f) / this.game.GuiScale);

            button.Set(position, text, this.game.GuiScale, center, textX, width);
        }



        public override void OnPause()
        {
            this.camera.ResetPosition();
            this.paused = true;
        }

        public override void OnResume()
        {
        }

        public override void Reset()
        {
            this.player.Reset();
            this.background.Reset();

            this.enemyManager.Reset();
            this.bulletManager.Reset();
            this.explosionManager.Reset();
            this.cloudManager.Reset();
            
            this.camera.ResetPosition();
            this.time = 0f;
            this.dieAnimation = false;
            this.GameSpeed = 1f;
            this.slowDie = 5f;

            this.paused = false;

            if(this.game.settings.vibration)
            {
                this.player.Vibrate = this.game.Vibrate;
            }
            else
            {
                this.player.Vibrate = (duration) => { };
            }
        }
    }
}