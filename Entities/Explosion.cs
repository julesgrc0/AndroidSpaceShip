using System;
using System.Collections.Generic;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Entities
{
    public class ExplosionManager : Entity
    {
        public List<Explosion> explosions = new List<Explosion>();
        private Camera camera;
        public ExplosionManager()
        {
            
        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera)
        {
            this.camera = camera;

            base.Initialize(spriteBatch, contentManager);
        }

        public void Add(Vector2 position)
        {
            Explosion explosion = new Explosion();
            explosion.Initialize(this.spriteBatch, this.contentManager, this.camera);
            explosion.Load(position, texture);
            this.explosions.Add(explosion);
        }

        public override void Load()
        {
            this.texture = this.contentManager.Load<Texture2D>("explosion");
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            List<Explosion> remove = new List<Explosion>();
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(deltatime, touchLocations);
                if (explosion.remove)
                {
                    remove.Add(explosion);
                }
            }

            foreach (Explosion explosion in remove)
            {
                explosions.Remove(explosion);
            }
            remove.Clear();
        }

        public override void Draw()
        {
            foreach (Explosion explosion in this.explosions)
            {
                explosion.Draw();
            }
        }

        public override void Reset()
        {
            this.explosions.Clear();
        }
    }


    public class Explosion : Entity
    {
        private Rectangle[] sourceRect;
        public Vector2 size { get; private set; }

        public float animation { get; private set; } = 0f;
        public bool remove { get; private set; } = false;
        private Camera camera;

        public Explosion()
        {

        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera)
        {
            this.camera = camera;
            base.Initialize(spriteBatch, contentManager);
        }

        public void Load(Vector2 position, Texture2D texture2d)
        {
            this.texture = texture2d;
            this.sourceRect = new Rectangle[4];

            int tileW = 16, tileH = 16;

            this.size = new Vector2(tileW * 3.5f, tileH * 3.5f);
            this.position = position;

            for (int x = 0; x < 4; x++)
            {
                this.sourceRect[x] = new Rectangle(x * tileW, 0 * tileH, tileW, tileH);
            }
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            this.animation += deltatime * 10;
            if (this.animation > this.sourceRect.Length)
            {
                this.animation = 0f;
                this.remove = true;
            }
            this.position += new Vector2(0, deltatime * 300);
        }

        public override void Draw()
        {
            if (!this.remove)
            {
                this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);

                this.spriteBatch.Draw(this.texture,
                    destinationRectangle: new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.size.X, (int)this.size.Y),
                    sourceRectangle: sourceRect[(int)this.animation],
                    Color.White);

                this.spriteBatch.End();

            }
        }
    }
}