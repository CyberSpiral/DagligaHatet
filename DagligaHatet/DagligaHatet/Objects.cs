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
        /*public void GetLocal(Tile[,] T) {
            if (T[(int)Position.X + 1, (int)Position.Y].Contains) {
                T[(int)Position.X + 1, (int)Position.Y].Contained.Number++;
            }
        }*/

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Texture, new Vector2(Position.X, Position.Y), Color.White);
        }

    }
    public class PlayerCharacter : Object {
        public int MoveSpeed { get; }
        public attackStyle Style { get; }
        public int Range { get; }
        public int Health { get; set; }
        public int Damage { get; }

        public PlayerCharacter(Texture2D tex, Vector2 position, Vector2 mapPosition, string name, int range, int movementSpeed, attackStyle style, int health, int damage) : base(tex, position, mapPosition, name) {
            MoveSpeed = movementSpeed;
            Range = range;
            Style = style;
            Health = health;
            Damage = damage;
        }

        public void Move(Tile[,] t/*, List<Rectangle> selection*/) {
            /*for (int i = 0; i < MoveSpeed; i++) {
                selection.Add(new Rectangle((int)Position.X + MoveSpeed * Tile.HeightWidth, (int)Position.Y, Tile.HeightWidth, Tile.HeightWidth));
                selection.Add(new Rectangle((int)Position.X - MoveSpeed * Tile.HeightWidth, (int)Position.Y, Tile.HeightWidth, Tile.HeightWidth));
                selection.Add(new Rectangle((int)Position.X, (int)Position.Y + MoveSpeed * Tile.HeightWidth, Tile.HeightWidth, Tile.HeightWidth));
                selection.Add(new Rectangle((int)Position.X, (int)Position.Y - MoveSpeed * Tile.HeightWidth, Tile.HeightWidth, Tile.HeightWidth));
            }*/

        }
    }

    public enum attackStyle {
        around, linecross, lineXcross
    }

    public abstract class Skill {

        public abstract void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map);

        public abstract void InvokeSkill();
    }
    
}
