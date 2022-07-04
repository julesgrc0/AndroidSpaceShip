using System;
using System.IO;
using System.Diagnostics;


using AndroidSpaceShip.Core;
using AndroidSpaceShip.Entities;
using AndroidSpaceShip.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AndroidSpaceShip.Gui;
using Microsoft.Xna.Framework.Input.Touch;


namespace AndroidSpaceShip
{
    public enum GameScreens
    {
        NONE = 0,
        HOME = 1,
        DEFAULT_GAME = 2,
        SETTING = 3,
        GAMEOVER = 4,
    }


    public class GameSettings
    {
        public float aspectRatio = 1f;
        public bool vibration = true;
        public bool audio = true;

        public int bestKill = 0;
        public int bestScore = 0;
        public float bestTime = 0;

        public bool init = false;
    }
    public class GameRoot : Game
    {
        public GraphicsDeviceManager graphicsDevice { get; private set; }
        public SpriteBatch spriteBatch { get; private set; }
        public SpriteFont font { get; private set; }
        public Camera hudCamera  { get; private set;  }
        public Vector2 cameraViewSize { get; private set; } = new Vector2(400, 1000);

        public Action<long> Vibrate = (duration) => { };

        public DefaultGame ScreenDefaultGame;
        public Home ScreenHome;
        public Setting ScreenSetting;
        public GameOver ScreenGameOver;

        public Screen currentScreen;
        public Storage storage;
        public GameSettings settings;

        public float GuiScale = 3.5f;
        public float FontScale = 0.25f;

#if TEST || DEBUG
        private bool debugMode = false;
        private IconButton debugButton = new IconButton();
#endif

        public GameRoot()
        {
            graphicsDevice = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                PreferHalfPixelOffset = true,
                PreferMultiSampling = true,
                HardwareModeSwitch = true,
                SynchronizeWithVerticalRetrace = true,

                GraphicsProfile = GraphicsProfile.HiDef,
                SupportedOrientations = DisplayOrientation.Portrait,

                /*
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
                */
            };

            graphicsDevice.ApplyChanges();

            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsFixedTimeStep = false;

            //IsFixedTimeStep = true;
            //TargetElapsedTime = System.TimeSpan.FromSeconds(1d / 60);
        }


        protected override void Initialize()
        {

            this.graphicsDevice.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.graphicsDevice.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphicsDevice.ApplyChanges();

            this.hudCamera = new Camera(GraphicsDevice.Viewport, cameraViewSize);
            //this.hudCamera.Zoom = 0.5f;
            this.hudCamera.UpdateMatrix();

            this.ScreenDefaultGame = new DefaultGame(this);
            this.ScreenDefaultGame.Id = (int)GameScreens.DEFAULT_GAME;

            this.ScreenHome = new Home(this);
            this.ScreenHome.Id = (int)GameScreens.HOME;

            this.ScreenSetting = new Setting(this);
            this.ScreenSetting.Id = (int)GameScreens.SETTING;

            this.ScreenGameOver = new GameOver(this);
            this.ScreenGameOver.Id = (int)GameScreens.GAMEOVER;

            this.currentScreen = ScreenHome;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("font");

            settings = new GameSettings();
            settings.init = true;
            settings.aspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

            storage = new Storage();
            if(storage.Init())
            {
                storage.Write(settings);
            }
            else
            {
                try
                {
                    GameSettings testSettings = storage.Read().ToObject<GameSettings>();
                    if (testSettings.init)
                    {
                        settings = testSettings;
                        if (settings.aspectRatio != GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio)
                        {
                            int[] sizes = this.calcAspectRatio(this.graphicsDevice.PreferredBackBufferWidth, this.settings.aspectRatio);
                            this.ChangeViewport(sizes);
                        }
                    }
                    else
                    {
                        storage.Write(settings);
                    }
                }
                catch
                {
                    storage.Write(settings);
                }
            }

            this.ScreenDefaultGame.Load();
            this.ScreenHome.Load();
            this.ScreenSetting.Load();
            this.ScreenGameOver.Load();

#if TEST || DEBUG
            Texture2D debugIcon = Content.Load<Texture2D>("debug");
            debugButton.Initialize(this.spriteBatch, this.Content, this.hudCamera);
            debugButton.Load();
            debugButton.Set(new Vector2(this.hudCamera.Size.X - 30f, this.hudCamera.Size.Y - 60f), debugIcon, this.GuiScale, true);
#endif
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            TouchCollection touchLocations = TouchPanel.GetState();
            GameScreens next;         

            if ((next = (GameScreens)this.currentScreen.Update(deltatime, touchLocations)) != GameScreens.NONE)
            {
                if(next == GameScreens.HOME)
                {
                    this.currentScreen = this.ScreenHome;
                }else if(next == GameScreens.DEFAULT_GAME)
                {
                    this.currentScreen = this.ScreenDefaultGame;
                    this.currentScreen.Reset();
                }else if(next == GameScreens.SETTING)
                {
                    this.currentScreen = this.ScreenSetting;
                }else if(next == GameScreens.GAMEOVER)
                {
                    this.ScreenGameOver.Reset();
                    if(this.currentScreen.Id == (int)GameScreens.DEFAULT_GAME)
                    {
                        this.ScreenGameOver.backgroundY = this.ScreenDefaultGame.background.MoveY - (deltatime * this.ScreenDefaultGame.background.Speed) * 2;
                        this.ScreenGameOver.time = this.ScreenDefaultGame.time;
                        this.ScreenGameOver.kill = this.ScreenDefaultGame.player.Kill;
                        this.ScreenGameOver.score = (int)((this.ScreenDefaultGame.player.Kill + 1) * this.ScreenDefaultGame.time * 10);
                    }                    
                    this.currentScreen = this.ScreenGameOver;
                }
            }

#if TEST || DEBUG
            if (this.currentScreen.Id == (int)GameScreens.HOME)
            {
                this.debugButton.Update(deltatime, touchLocations);
                if (this.debugButton.isClick())
                {
                    this.debugMode = !this.debugMode;
                }
            }
#endif

            base.Update(gameTime);
        }

        public void ChangeViewport(int[] sizes)
        {
            this.graphicsDevice.PreferredBackBufferWidth = sizes[0];
            this.graphicsDevice.PreferredBackBufferHeight = sizes[1];

            this.graphicsDevice.ApplyChanges();

            this.ScreenHome.camera.ChangeViewport(GraphicsDevice.Viewport);
            this.ScreenSetting.camera.ChangeViewport(GraphicsDevice.Viewport);
            this.ScreenDefaultGame.camera.ChangeViewport(GraphicsDevice.Viewport);
            this.ScreenGameOver.camera.ChangeViewport(GraphicsDevice.Viewport);
        }

        public int[] calcAspectRatio(int width, float aspectRatio)
        {
            int[] size = new int[2];
            size[0] = width;
            size[1] = (int)(width / aspectRatio);
            return size;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            this.currentScreen.Draw();

#if TEST || DEBUG
            float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (deltatime != 0)
            {

                if (this.debugMode)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.hudCamera.Transform);

                    string info = $@" Graphics Adapter {GraphicsDevice.Adapter.Description}
 Display Mode {GraphicsDevice.Adapter.CurrentDisplayMode}
 FPS {1 / deltatime}
 Deltatime {deltatime}
 Resolution {graphicsDevice.PreferredBackBufferWidth}x{graphicsDevice.PreferredBackBufferHeight}
 Aspect Ratio {(float)graphicsDevice.PreferredBackBufferWidth / (float)graphicsDevice.PreferredBackBufferHeight}
 Screen Id {this.currentScreen.Id}
 Storage Error {!storage.StorageOK}
 Loading Settings {storage.Loading}";

                    Process proc = Process.GetCurrentProcess();
                    info += $@"
 Name {proc.ProcessName}
 Threads {proc.Threads.Count}
 Memory {proc.PrivateMemorySize64 / (1024.0 * 1024.0)} Mo";
                    proc.Dispose();

                    info += $@"

 Game
 Position {this.ScreenDefaultGame.player.position}
 Velocity {this.ScreenDefaultGame.player.velocity}
 Life {this.ScreenDefaultGame.player.Life}
 Kill {this.ScreenDefaultGame.player.Kill}
 Time {this.ScreenDefaultGame.time}
 Game Speed x{this.ScreenDefaultGame.GameSpeed}
 New Cloud {this.ScreenDefaultGame.cloudManager.addTime}
 New Enemy {this.ScreenDefaultGame.enemyManager.addTime}
    ";
#if DEBUG
                    info += "\n Mode DEBUG";
#else
                    info += "\n Mode TEST";
#endif
info +=  $"\n Version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";

                    spriteBatch.DrawString(font, info, Vector2.Zero, Color.White, 0, Vector2.Zero, FontScale / 2f, SpriteEffects.None, 0);

                    spriteBatch.End();
                }

                if (this.currentScreen.Id == (int)GameScreens.HOME)
                {
                    this.debugButton.Draw();
                }

            }
#endif

                    //SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, ScreenMatrix);
                    //Game Objects

                    //SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, ScreenMatrix);
                    //Foreground Tile Layers

                    //SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, ScreenMatrix);
                    //Normal Effects & Particles

                    //SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null, ScreenMatrix);
                    //Other Effects & Particles

                    base.Draw(gameTime);
        }

        public void OnPause()
        {
            if (this.currentScreen != null)
            {
                this.currentScreen.OnPause();
            }
        }

        public void OnResume()
        {
            if(this.currentScreen != null)
            {
                this.currentScreen.OnResume();
            }
        }

        public void OnRestart()
        {

        }
    }
}
