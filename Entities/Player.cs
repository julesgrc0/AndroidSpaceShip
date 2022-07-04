using System;
using System.IO;

using AndroidSpaceShip.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;


namespace AndroidSpaceShip.Entities
{
    public class Player : Entity
    {
        private Rectangle[] SourceRect;
        public Vector2 Size { get; private set; }
        
        public const int Speed = 4000;
        public const int Friction = -5;
        public const int FireInterval = 40;

        public int Life { get; private set; } = 100;
        public int Kill { get; private set; } = 0;

        public float Animation { get; private set; } = 0f;
        public float FireTime { get; private set; }  = 0f;
        public float ShakeTime { get; private set; } = 0f;

        private bool initAnimation = false;
        private bool animationBack = false;
        private Vector2 rangeAnimation = Vector2.Zero;
        public Vector2 velocity { get; private set; } = Vector2.Zero;

        private Camera camera;
        private BulletManager bulletManager;

        public Action<long> Vibrate = (duration) => { };

        public Player()
        {
            
        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera, BulletManager bulletManager)
        {
            this.camera = camera;
            this.bulletManager = bulletManager;
            
            base.Initialize(spriteBatch, contentManager);
        }

        public override void Load()
        {
#if TEST
            this.texture = this.contentManager.Load<Texture2D>("ships/test");
#else
            this.texture = this.contentManager.Load<Texture2D>("ships/red");
#endif
            this.SourceRect = new Rectangle[10];

            int tileW = 16, tileH = 24;

            this.Size = new Vector2(tileW * 3.5f, tileH * 3.5f);
            int margin = 20;
            this.position = new Vector2((this.camera.Size.X - this.Size.X) / 2, this.camera.Size.Y + this.Size.Y);

            this.rangeAnimation = new Vector2(this.camera.Size.Y - this.Size.Y - margin, this.camera.Size.Y/4 + this.Size.Y);

            for (int x = 0;x < 5; x++)
            {
                for(int y = 0;y < 2;y++)
                {
                    this.SourceRect[y * 5 + x] = new Rectangle(x * tileW, y * tileH, tileW, tileH);
                }
            }
        }
        
        public bool Alive()
        {
            return this.Life > 0;
        }

        public bool InitAnimation()
        {
            return this.initAnimation;
        }

        public void AddKill()
        {
            this.Kill++;
        }

        public void Damage(int damage)
        {
            this.Life -= damage;
            this.ShakeTime = 50f;
            this.Vibrate(100);

        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            this.Animation += deltatime * 10;
            this.Animation %= 10;
            
            if (!this.initAnimation)
            {
                if(!this.animationBack)
                {
                    this.position -= new Vector2(0, deltatime * 500);
                    if (this.position.Y <= this.rangeAnimation.Y)
                    {
                        this.animationBack = true;
                    }
                }
                else
                {
                    this.position += new Vector2(0, deltatime * 200);
                    if (this.position.Y >= this.rangeAnimation.X)
                    {
                        this.position = new Vector2(this.position.X, this.rangeAnimation.X);
                        this.initAnimation = true;
                    }
                }

                return;
            }

            this.FireTime += deltatime * 100;
            if (this.ShakeTime > 0)
            {
                this.ShakeTime -= deltatime * 100;
                this.camera.ResetPosition();
                this.camera.Shake(5);
            }
            else
            {
                this.ShakeTime = 0f;
            }


            Vector2 acceleration = Vector2.Zero;
            bool move = false;
            

            foreach (TouchLocation touch in touchLocations)
            {
                if (touch.State == TouchLocationState.Moved || touch.State == TouchLocationState.Pressed)
                {
                    Vector2 worldPosition = this.camera.Point(touch.Position);

                    if (worldPosition.X <= this.camera.Size.X / 2)
                    {
                        move = true;
                        acceleration.X = -Speed;
                    }
                    else
                    {
                        move = true;
                        acceleration.X = Speed;
                    }
                }
            }

            if (this.FireTime > FireInterval)
            {
                this.FireTime = 0f;
                this.bulletManager.Add(new Vector2(position.X + Size.X/2, position.Y), Direction.FromPlayer, BulletTypeName.BULLET);
            }


            acceleration += velocity * Friction;
            velocity += acceleration * deltatime;
            position += (velocity * deltatime) + (acceleration * 0.5f) * (deltatime * deltatime);

            if (Math.Abs(velocity.X) < 0.1 && !move)
            {
                velocity = Vector2.Zero;
            }

            if (position.X < 0)
            {
                position = new Vector2(0, position.Y);
                velocity = Vector2.Zero;
            }
            else if (position.X > this.camera.Size.X - this.Size.X)
            {
                position = new Vector2(this.camera.Size.X - this.Size.X, position.Y);
                velocity = Vector2.Zero;
            }
        }

        public override void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
            
            this.spriteBatch.Draw(this.texture,
                destinationRectangle: new Rectangle((int)this.position.X,(int)this.position.Y, (int)this.Size.X, (int)this.Size.Y),
                sourceRectangle: SourceRect[(int)this.Animation],
                Color.White);
            
            this.spriteBatch.End();
        }

        public override void Reset()
        {
            this.Kill = 0;
            this.ShakeTime = 0;
            this.FireTime = 0;
            this.Animation = 0;
            this.Life = 100;

            int margin = 80;

            this.velocity = Vector2.Zero;
            this.position = new Vector2((this.camera.Size.X - this.Size.X) / 2, this.camera.Size.Y + this.Size.Y);
            this.initAnimation = false;
            this.animationBack = false;
            this.rangeAnimation = new Vector2(this.camera.Size.Y - this.Size.Y - margin, this.camera.Size.Y*0.7f + this.Size.Y);
        }
    }
}
