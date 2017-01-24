using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DagligaHatet {
    public class Object {
        public Texture2D Texture { get; }
        public Vector2 MapPosition { get; set; }
        public Vector2 Position { get; set; }

        public string Name { get; }

        public Object(Texture2D tex, Vector2 position, Vector2 mapPosition, string name) {
            Texture = tex;
            Position = position;
            MapPosition = mapPosition;
            Name = name;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Texture, new Vector2(Position.X, Position.Y), Color.White);
        }

    }
    public class PlayerCharacter : Object {
        public int MoveSpeed { get; }
        public int Health { get; set; }

        public Skill Attack { get; }
        public int Damage { get; }
        public int Range { get; }

        public Skill Skill { get; }
        public int SkillRange { get; set; }


        /// <summary>
        /// Creating a new player controlled character
        /// </summary>
        /// <param name="tex">Sprite texture</param>
        /// <param name="position">Position in pixels</param>
        /// <param name="mapPosition">Position in map-coordinates</param>
        /// <param name="name">Name to identify character</param>
        /// <param name="attack">Class of normal attack</param>
        /// <param name="range">Attack range</param>
        /// <param name="damage">Damage of normal attack</param>
        /// <param name="skill">Class of specific skill</param>
        /// <param name="skillRange">Range of skill</param>
        /// <param name="movementSpeed">Movement speed in tiles</param>
        /// <param name="health">Health points</param>
        public PlayerCharacter(Texture2D tex, Vector2 position, Vector2 mapPosition, string name, Skill attack, int range, int damage, Skill skill, int skillRange, int movementSpeed, int health) : base(tex, position, mapPosition, name) {
            MoveSpeed = movementSpeed;
            Range = range;
            Health = health;
            Damage = damage;
            Attack = attack;
            Skill = skill;
        }
    }


}
