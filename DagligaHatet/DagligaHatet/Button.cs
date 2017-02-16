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
        public string Name { get; }
        public bool Hidden { get; set; } = false;

        public Button(Rectangle hit, Texture2D tex, string name) {
            Texture = tex;
            Hitbox = hit;
            Name = name;
        }

        public bool Update(MouseState currentMouse, MouseState oldMouse) {
            Rectangle temp = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            if (temp.Intersects(Hitbox) && currentMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released) {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch sB) {
            if (!Hidden) {
                sB.Draw(Texture, Hitbox, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
        }
    }
}