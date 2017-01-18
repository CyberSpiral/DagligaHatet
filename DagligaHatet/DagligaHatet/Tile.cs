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
    public class Tile {
        public Vector2 Position { get; set; }
        public Vector2 MapPosition { get; set; }
        public Rectangle Collision { get { return new Rectangle((int)Position.X, (int)Position.Y, 40, 40); } }

        public bool Occupied { get; set; } = false;


        public Tile(Vector2 position, Vector2 mapPosition) {
            Position = position;
            MapPosition = mapPosition;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D tex) {
            spriteBatch.Draw(tex, Collision, Color.White);
        }
    }
}
