using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AndroidSpaceShip.Core
{


    public class Entity
    {
        protected SpriteBatch spriteBatch;
        protected ContentManager contentManager;

        protected Texture2D texture;
        public Vector2 position { get; protected set; } = Vector2.Zero;


        public Entity()
        {
           
        }

        public virtual void Initialize(SpriteBatch spriteBatch, ContentManager contentManager)
        {
            this.spriteBatch = spriteBatch;
            this.contentManager = contentManager;
        }

        public virtual void Load()
        {

        }

        public virtual void Update(float deltatime, TouchCollection touchLocations)
        {

        }

        public virtual void Draw()
        {

        }

        public virtual void Reset()
        {

        }
    }
}
