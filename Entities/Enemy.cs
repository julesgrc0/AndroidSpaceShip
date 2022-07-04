using System;
using System.Collections.Generic;
using AndroidSpaceShip.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Entities
{
    public enum EnemyTypeName
    {
        BIG_DOUBLE = 0,
        SLOW_BALL = 1,
        SPEED = 2,
        JOIN_SPEED = 3,
        BOSS_BIG = 4,
    };

    public class EnemyType
    {
        public EnemyType(int tileW, int tileH, int Speed, int Life, int FireInterval, EnemyTypeName type)
        {
            this.tileW = tileW;
            this.tileH = tileH;
            this.Speed = Speed;
            this.Life = Life;
            this.FireInterval = FireInterval;
            this.type = type;
        }

        public BulletTypeName bulletType = BulletTypeName.BULLET;
        public int tileW;
        public int tileH;
        public int Speed;
        public int Life;
        public int FireInterval;
        public EnemyTypeName type;
    }

    public class EnemyManager : Entity
    {
        public List<Enemy> enemies { get; private set; } = new List<Enemy>();
        private Texture2D[] textures;
        private Texture2D textureLife;

        private EnemyType[] enemyTypes;

        public float addTime { get; private set; } = 50f;
        private Tuple<EnemyTypeName, float, float>[] spawnLevel;
        private const float MAX_LEVEL = 100f;
        private const int MAX_ENEMY_ALIVE = 4;

        public Random random = new Random();
        private Camera camera;
        
        private ExplosionManager explosionManager;
        private BulletManager bulletManager;
        private Player player;


        public float Speed = 1f;
        public EnemyManager()
        {
           
        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera, ExplosionManager explosionManager, BulletManager bulletManager, Player player)
        {
            this.camera = camera;
            this.bulletManager = bulletManager;
            this.explosionManager = explosionManager;
            this.player = player;

            base.Initialize(spriteBatch, contentManager);
        }

        public override void Load()
        {
            this.enemyTypes = new EnemyType[5];

            this.enemyTypes[0] = new EnemyType(32, 32,Speed: 120, Life: 50, FireInterval: 100, EnemyTypeName.BIG_DOUBLE);
            
            this.enemyTypes[1] = new EnemyType(32, 16, 80, 40, 120, EnemyTypeName.SLOW_BALL);
            this.enemyTypes[1].bulletType = BulletTypeName.BALLS;

            this.enemyTypes[2] = new EnemyType(16, 16, 200, 20, 40, EnemyTypeName.SPEED);
            
            this.enemyTypes[3] = new EnemyType(29, 16, 150, 40, 70, EnemyTypeName.JOIN_SPEED);
            
            this.enemyTypes[4] = new EnemyType(26, 25, 120, 50, 100, EnemyTypeName.BOSS_BIG);

            this.spawnLevel = new Tuple<EnemyTypeName, float, float>[5];
            this.spawnLevel[0] = new Tuple<EnemyTypeName, float, float>(EnemyTypeName.SLOW_BALL, 0f, 3f);
            this.spawnLevel[1] = new Tuple<EnemyTypeName, float, float>(EnemyTypeName.SPEED, 0f, 1.5f);
            this.spawnLevel[2] = new Tuple<EnemyTypeName, float, float>(EnemyTypeName.BIG_DOUBLE, 1.5f, 2.5f);
            this.spawnLevel[3] = new Tuple<EnemyTypeName, float, float>(EnemyTypeName.JOIN_SPEED, 2f, MAX_LEVEL);
            this.spawnLevel[4] = new Tuple<EnemyTypeName, float, float>(EnemyTypeName.BOSS_BIG, 2.5f, MAX_LEVEL);

            this.textures = new Texture2D[5];
            this.textures[0] = this.contentManager.Load<Texture2D>("enemy-big");
            this.textures[1] = this.contentManager.Load<Texture2D>("enemy-medium");
            this.textures[2] = this.contentManager.Load<Texture2D>("enemy-small");
            this.textures[3] = this.contentManager.Load<Texture2D>("enemy-double");
            this.textures[4] = this.contentManager.Load<Texture2D>("enemy-boss");

            this.textureLife = this.contentManager.Load<Texture2D>("enemy-life");
        }

        private int GetSpawnIndex()
        {
            List<EnemyTypeName> types = new List<EnemyTypeName>();
            foreach(var lv in this.spawnLevel)
            {
                if(this.Speed >= lv.Item2 && this.Speed < lv.Item3)
                {
                    types.Add(lv.Item1);
                }
            }

            if(types.Count <= 0)
            {
                return -1;
            }


            int i = this.random.Next(0, types.Count);
            return (int)types[i];
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            this.addTime -= deltatime * 10 * Speed;
            if(this.addTime <= 0 && this.enemies.Count <= MAX_ENEMY_ALIVE)
            {
                int index = this.GetSpawnIndex();

                if(index >= 0)
                {
                    this.addTime = 50f + (index + 1) * 10f;

                    Enemy enemy = new Enemy();
                    enemy.Initialize(this.spriteBatch, this.contentManager, this.camera, this.bulletManager);
                    enemy.Load(this.enemyTypes[index], this.textures[index], this.textureLife);

                    this.enemies.Add(enemy);
                }
                else
                {
                    this.addTime = 50f;
                }
                
            }

            List<Enemy> remove = new List<Enemy>();
            foreach(Enemy enemy in enemies)
            {
                enemy.Update(deltatime, touchLocations);
                if(enemy.position.Y - enemy.size.Y > this.camera.Size.Y || enemy.Life <= 0)
                {
                    remove.Add(enemy);
                }
            }

            foreach(Enemy enemy in remove)
            {
                if(enemy.Life <= 0)
                {
                    this.explosionManager.Add(enemy.position);
                    this.player.AddKill();
                }
                enemies.Remove(enemy);
            }
            remove.Clear();
        }

        public override void Draw()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw();
            }
        }

        public override void Reset()
        {
            this.addTime = 50f;
            this.Speed = 0;
            this.enemies.Clear();
        }

    }

    public class Enemy : Entity
    {
        private Rectangle[] sourceRect;
        public Vector2 size { get; private set; }
        public Vector2 moveRange {  get; private set; }
        public bool LeftDirection { get; private set; } = false;
        private BulletTypeName bulletType = BulletTypeName.BULLET;
        public int Speed { get; private set; } = 0;
        public int Life { get; private set; } = 0;
        public int FireInterval { get; private set; } = 0;
        public float Animation { get; private set; } = 0f;
        public float FireTime { get; private set; } = 0f;

        private Camera camera;
        private BulletManager bulletManager;
        private EnemyTypeName type;
        private Texture2D lifeTexture;
        public Enemy()
        {
            
        }

        public void Initialize(SpriteBatch spriteBatch, ContentManager contentManager, Camera camera, BulletManager bulletManager)
        {
            this.camera = camera;
            this.bulletManager = bulletManager;
            base.Initialize(spriteBatch, contentManager);
        }

        public void Damage(int damage)
        {
            this.Life -= damage;
        }
        public void Load(EnemyType enemy, Texture2D enemyTexture, Texture2D lifeTexture)
        {
            this.sourceRect = new Rectangle[2];
           
            this.texture = enemyTexture;
            this.lifeTexture = lifeTexture;
            
            this.type = enemy.type;
            this.Life = enemy.Life;
            
            this.Speed = enemy.Speed;
            this.FireInterval = enemy.FireInterval;
            
            this.bulletType = enemy.bulletType;

            this.FireTime = this.FireInterval + 1f;

            this.size = new Vector2(enemy.tileW * 3.5f, enemy.tileH * 3.5f);

            Random random = new Random();

            int minX = 0;
            int maxX = (int)(this.camera.Size.X - this.size.X);
            

            this.moveRange = new Vector2(minX, maxX);

            this.position = new Vector2(random.Next(minX, maxX), -this.size.Y);

            this.sourceRect[0] = new Rectangle(0, 0, enemy.tileW, enemy.tileH);
            this.sourceRect[1] = new Rectangle(enemy.tileW, 0, enemy.tileW, enemy.tileH);
        }

        public override void Update(float deltatime, TouchCollection touchLocations)
        {
            this.FireTime += deltatime * 100;
            float move = deltatime * Speed;

            this.Animation += deltatime * 10;
            this.Animation %= this.sourceRect.Length;

            if (this.LeftDirection)
            {
                this.position = new Vector2(position.X - move, position.Y + move);

                if (this.position.X <= this.moveRange.X)
                {
                    this.LeftDirection = false;
                }
            }
            else
            {
                this.position = new Vector2(position.X + move, position.Y + move);

                if (this.position.X >= this.moveRange.Y)
                {
                    this.LeftDirection = true;
                }
            }

            if (this.FireTime > FireInterval)
            {
                this.FireTime = 0f;
                if(this.type == EnemyTypeName.BIG_DOUBLE)
                {
                    this.bulletManager.Add(new Vector2(position.X + size.X * (1/4f), position.Y), Direction.FromEnemy, this.bulletType);
                    this.bulletManager.Add(new Vector2(position.X + size.X * (3/4f), position.Y), Direction.FromEnemy, this.bulletType);
                }else if(this.type == EnemyTypeName.BOSS_BIG)
                {
                    this.bulletManager.Add(new Vector2(position.X + size.X * (1 / 4f), position.Y), Direction.FromEnemy, this.bulletType);
                    this.bulletManager.Add(new Vector2(position.X + size.X * (3 / 4f), position.Y), Direction.FromEnemy, this.bulletType);

                    this.bulletManager.Add(new Vector2(position.X + size.X / 2, position.Y), Direction.FromEnemy, BulletTypeName.MISSILE);
                }else if(this.type == EnemyTypeName.JOIN_SPEED)
                {
                    float margin = 2 * 3.5f;
                    this.bulletManager.Add(new Vector2(position.X - margin, position.Y), Direction.FromEnemy, this.bulletType);
                    this.bulletManager.Add(new Vector2(position.X + size.X + margin, position.Y), Direction.FromEnemy, this.bulletType);
                }
                else
                {
                    this.bulletManager.Add(new Vector2(position.X + size.X / 2, position.Y), Direction.FromEnemy, this.bulletType);
                }
            }
        }

        public override void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, this.camera.Transform);

            this.spriteBatch.Draw(this.texture,
                destinationRectangle: new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.size.X, (int)this.size.Y),
                sourceRectangle: sourceRect[(int)this.Animation],
                Color.White);
            
            for (int i = 0; i < this.Life; i += 10)
            {
                this.spriteBatch.Draw(this.lifeTexture, new Rectangle((int)this.position.X + i, (int)this.position.Y, 8, 8), Color.White);
            }

            this.spriteBatch.End();
        }
    }
}
