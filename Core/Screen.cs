using Microsoft.Xna.Framework.Input.Touch;


namespace AndroidSpaceShip.Core
{
    public class Screen
    {
        protected GameRoot game;
        public int Id { get; set; }
        public Screen(GameRoot root)
        {
            this.game = root;
        }

        public virtual void Initialize()
        {

        }
        public virtual void Load()
        {

        }

        public virtual int Update(float deltatime, TouchCollection touchLocations)
        {
            return 0;
        }

        public virtual void Draw()
        {

        }

        public virtual void OnPause()
        {

        }

        public virtual void OnResume()
        {

        }

        public virtual void Reset()
        {

        }
    }
}