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
        public Tile Inhabited { get; set; }
        public Vector2 MapPosition { get { return Inhabited.MapPosition; } }

        public string Name { get; }

        public Object(Texture2D tex, string name) {
            Texture = tex;
            Name = name;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Texture, World.TranslateMapPosition(Inhabited.MapPosition), Color.White);
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
        public int SkillDamage { get; }


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
        public PlayerCharacter(Texture2D tex, string name, Skill attack, int range, int damage, Skill skill, int skillRange, int skillDamage, int movementSpeed, int health) : base(tex, name) {
            MoveSpeed = movementSpeed;
            Range = range;
            Health = health;
            Damage = damage;
            Attack = attack;
            Skill = skill;
            SkillRange = skillRange;
            SkillDamage = skillDamage;
        }
    }
}
