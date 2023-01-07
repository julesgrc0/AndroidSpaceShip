using System;
using System.Collections.Generic;
using AndroidSpaceShip.Core;
using AndroidSpaceShip.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace SpaceShip.Entities
{
    public enum ItemTypeName
    {
        LIFE_SAVER,
        BOOST_FIRE,
        CLEAR_ENEMIES,
        COIN,
        GIFT
    }

    public class ItemManager : Entity
    {
        public List<Item> items { get; private set; } = new List<Item>();
        public ItemManager()
        {

        }

        public void Initialise()
        {

        }
    }

    public class Item : Entity
    {
        public Item()
        {

        }

        /*
        public bool CheckCollision(Vector2 position, Vector2 size)
        {
            if (position.X < this.position.X + this.Size.X && position.X + size.X > this.position.X && position.Y < this.position.Y + this.Size.Y && position.Y + size.Y > this.position.Y)
            {
                return true;
            }

            return false;
        }
        */
    }
}
