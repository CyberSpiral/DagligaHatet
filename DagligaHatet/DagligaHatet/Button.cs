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
using System.Collections;

namespace DagligaHatet {
    public class Button {
        public Texture2D Texture { get; }
        public Rectangle Hitbox { get; }

        public Button(Rectangle hit, Texture2D tex) {
            Texture = tex;
            Hitbox = hit;
        }

        public bool Update(MouseState currentMouse, MouseState oldMouse) {
            Rectangle temp = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            if (temp.Intersects(Hitbox) && currentMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released) {
                return true;
            }
            return false;
        }
    }

    public class Selected {
        public Texture2D Shade { get; }
        public Vector2 Position { get; }
        public Vector2 MapPosition { get; }

        public Selected(Texture2D shade, Vector2 map, Vector2 pos) {
            Shade = shade;
            MapPosition = map;
            Position = pos;
        }
    }


}
