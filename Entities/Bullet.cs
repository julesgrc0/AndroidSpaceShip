using System;
using System.Collections.Generic;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Entities
{
    public enum Direction
    {
        FromPlayer = 0,
        FromEnemy = 1,
    };

    public enum BulletTypeName
    {
        BULLET,
        MISSILE,
        BALLS
    }

    public class BulletManager : Entity
    {
        private List<Bullet> bullets = new List<Bullet>();
        private Camera camera;
        private EnemyManager enemyManager;
        private Player player;
        public BulletManager()
        {
           
        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera, EnemyManager enemyManager, Player player)
        {
            this.camera = camera;
            this.player = player;
            this.enemyManager = enemyManager;
            base.Initialize(spriteBatch, contentManager);
        }

        public void Add(Vector2 position, Direction from, BulletTypeName type)
        {
            if (type == BulletTypeName.BULLET || type == BulletTypeName.MISSILE)
            {
                Bullet bullet = new Bullet();
                bullet.Initialize(this.spriteBatch, this.contentManager, this.camera);
                bullet.Load(from, type, this.texture);

                bullet.SetPosition(new Vector2(position.X - bullet.Size.X / 2, position.Y + (from == Direction.FromEnemy ? bullet.Size.Y : 0)));
                this.bullets.Add(bullet);
            }
            else if(type == BulletTypeName.BALLS)
            {
                this.GenerateBalls(position, from);
            }
        }

        private void GenerateBalls(Vector2 position, Direction from)
        {
            Random random = new Random();
            int count = random.Next(5, 10);


            for (int i = 0; i < count; i++)
            {
                Bullet bullet = new Bullet();
                bullet.Initialize(this.spriteBatch, this.contentManager, this.camera);
                bullet.Load(from, BulletTypeName.BALLS, this.texture);

                Vector2 space = new Vector2(random.Next(0, 5), random.Next(0, 4)/10f);
                space *= bullet.Size;
                space -= new Vector2(bullet.Size.X, 0);

                Vector2 pos = new Vector2(position.X - bullet.Size.X / 2, position.Y);

                bullet.SetPosition(pos + space);

                this.bullets.Add(bullet);
            }
        }

        public override void Load()
        {
            this.texture = this.contentManager.Load<Texture2D>("laser-bolts");
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            List<Bullet> remove = new List<Bullet>();
            foreach (Bullet bullet in bullets)
            {
                bullet.Update(deltatime, touchLocations);
                if (bullet.Direction == Direction.FromEnemy)
                {
                    if (bullet.position.Y - bullet.Size.Y > this.camera.Size.Y)
                    {
                        remove.Add(bullet);
                    }
                    else if (bullet.CheckCollision(this.player.position, this.player.Size))
                    {
                        this.player.Damage(bullet.Damage);
                        remove.Add(bullet);
                    }

                }
                else
                {
                    if (bullet.position.Y + bullet.Size.Y < 0)
                    {
                        remove.Add(bullet);
                    }
                    else
                    {
                        foreach (Enemy enemy in this.enemyManager.enemies)
                        {
                            if (bullet.CheckCollision(enemy.position, enemy.size))
                            {
                                enemy.Damage(bullet.Damage);
                                remove.Add(bullet);
                                break;
                            }
                        }
                    }
                }

            }

            foreach (Bullet bullet in remove)
            {
                bullets.Remove(bullet);
            }
            remove.Clear();
        }

        public override void Draw()
        {
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw();
            }
        }

        public override void Reset()
        {
            this.bullets.Clear();
        }

    }

    public class Bullet : Entity
    {
        private Rectangle[] SourceRect;
        public Vector2 Size { get; private set; }
        public int Damage { get; private set; }
        private int Speed = 800;

        private float Animation = 0f;
        public Direction Direction { get; private set; }

        private BulletTypeName type = BulletTypeName.BULLET;

        private Random Random = new Random();
        private Camera camera;

        public Bullet() 
        {
            
        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera)
        {
            this.camera = camera;
            base.Initialize(spriteBatch, contentManager);
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public bool CheckCollision(Vector2 position, Vector2 size)
        {
            if (position.X < this.position.X + this.Size.X && position.X + size.X > this.position.X && position.Y < this.position.Y + this.Size.Y && position.Y + size.Y > this.position.Y)
            {
                return true;    
            }

            return false;
        }

        public void Load(Direction direction,BulletTypeName type, Texture2D texture2d)
        {
            this.type = type;

            this.Direction = direction;
            this.texture = texture2d;
            this.SourceRect = new Rectangle[2];
            int tileW = 16, tileH = 16;

            if (type == BulletTypeName.BALLS)
            {
                this.Damage = 5;
                this.SourceRect[0] = new Rectangle(0, 0, tileW, tileH);
                this.SourceRect[1] = new Rectangle(tileW, 0, tileH, tileW);
                this.Size = new Vector2(tileW * 2.5f, tileH * 2.5f);
            }
            else if(type == BulletTypeName.BULLET)
            {
                this.Damage = 10;
                this.SourceRect[0] = new Rectangle(0, tileH, tileW, tileH);
                this.SourceRect[1] = new Rectangle(tileW, tileH, tileH, tileW);
                this.Size = new Vector2(tileW * 1.5f, tileH * 4);
            }
            else if (type == BulletTypeName.MISSILE)
            {
                this.Damage = 20;
                this.SourceRect[0] = new Rectangle(0, tileH * 2, tileW, tileH);
                this.SourceRect[1] = new Rectangle(tileW, tileH * 2, tileH, tileW);
                this.Size = new Vector2(tileW * 2.5f, tileH * 3.5f);
            }
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            this.Animation += deltatime * 20;
            this.Animation %= 2;
        
            if(type == BulletTypeName.BULLET || type == BulletTypeName.MISSILE)
            {
                float move = deltatime * Speed;
                if (this.Direction == Direction.FromPlayer)
                {
                    this.position -= new Vector2(0, move);
                }
                else
                {
                    this.position += new Vector2(0, move);
                }

            }
            else if(type == BulletTypeName.BALLS)
            {
                float moveY = deltatime * (Speed/2);
                float moveX = this.Random.Next(-100, 100) * deltatime;

                if (this.Direction == Direction.FromPlayer)
                {
                    this.position -= new Vector2(moveX, moveY);
                }
                else
                {
                    this.position += new Vector2(moveX, moveY);
                }

            }
        }

        public override void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);

            this.spriteBatch.Draw(this.texture,
                destinationRectangle: new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.Size.X, (int)this.Size.Y),
                sourceRectangle: SourceRect[(int)this.Animation],
                Color.White,
                0,
                Vector2.Zero,
                this.Direction == Direction.FromEnemy ? SpriteEffects.FlipVertically : SpriteEffects.None,
                0f);
           
            this.spriteBatch.End();
        }
    }
}
