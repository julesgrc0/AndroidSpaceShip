using System;
using System.Collections.Generic;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Entities
{

    public class CloudManager : Entity {

        public List<Cloud> clouds = new List<Cloud>();
        
        public float addTime { get; private set; } = 100f;
        public float Speed = 0f;
        public const float targetSpeed = 1.8f;

        private Random random = new Random();
        private Camera camera;
        private Player player;

        public CloudManager()
        {

        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera, Player player)
        {
            this.camera = camera;
            this.player = player;
            base.Initialize(spriteBatch, contentManager);
        }

        public override void Load()
        {
            this.texture = this.contentManager.Load<Texture2D>("clouds");
        }

        public bool Active()
        {
            return this.Speed >= targetSpeed;
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            if(this.Active())
            {

                this.addTime -= deltatime * 5;
                if (this.addTime <= 0)
                {
                    this.addTime = this.random.Next(10, 15) * 15f;

                    bool direction = this.random.Next(0, 10) > 5;
                    System.Diagnostics.Debug.WriteLine(direction);

                    Cloud cloud = new Cloud();
                    cloud.Initialize(this.spriteBatch, this.contentManager, this.camera, this.player);
                    cloud.Load(this.texture, direction);

                    this.clouds.Add(cloud);
                }


                List<Cloud> remove = new List<Cloud>();
                foreach (Cloud cloud in clouds)
                {
                    cloud.Update(deltatime, touchLocations);
                    if (cloud.position.Y - cloud.Size.Y > this.camera.Size.Y)
                    {
                        remove.Add(cloud);
                    }
                }

                foreach (Cloud cloud in remove)
                {
                    clouds.Remove(cloud);
                }
                remove.Clear();
            }
        }

        public override void Draw()
        {
            if (this.Active())
            {
                foreach (Cloud cloud in clouds)
                {
                    cloud.Draw();
                }
            }
        }

        public override void Reset()
        {
            this.clouds.Clear();
            this.addTime = 100f;
            this.Speed = 0f;
        }
    }

    public class Cloud : Entity
    {
       
        public Vector2 Size { get; private set; }
        private Camera camera;
        private Player player;
        private float moveX = 0f;
        private bool direction = false;
        public Cloud()
        {


        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera, Player player)
        {
            this.camera = camera;
            this.player = player;
            base.Initialize(spriteBatch, contentManager);
        }

        public void Load(Texture2D texture, bool dir)
        {
            this.texture = texture;

            this.Size = new Vector2(this.camera.Size.X, texture.Height * 2);
            this.position = new Vector2(0, -this.Size.Y);

            this.direction = dir;
            if(dir)
            {
                this.moveX = this.texture.Width / 2f;
            }
            else
            {
                this.moveX = 0f;
            }
        }

        private void PlayerHit(Vector2 position, Vector2 size)
        {
            if (position.X < this.position.X + this.Size.X && position.X + size.X > this.position.X && position.Y < this.position.Y + this.Size.Y && position.Y + size.Y > this.position.Y)
            {
                this.camera.Shake(2);
            }
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            
            if(this.direction)
            {
                this.moveX -= deltatime * 10;
            }
            else
            {
                this.moveX += deltatime * 10;
            }

            if(this.moveX >= this.texture.Width / 2f)
            {
                this.direction = true;
                this.moveX = this.texture.Width / 2f;
            }
            else if(this.moveX <= 0f)
            {
                this.direction = false;
                this.moveX = 0;
            }

            this.position += new Vector2(0, deltatime * 120);
            this.PlayerHit(this.player.position, this.player.Size); 
        }

        public override void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);
            this.spriteBatch.Draw(this.texture,
                destinationRectangle: new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.Size.X, (int)this.Size.Y),
                sourceRectangle: new Rectangle((int)this.moveX, 0, this.texture.Width/2, this.texture.Height),
                Color.White);
            this.spriteBatch.End();
        }

        public override void Reset()
        {
        }
    }
}
