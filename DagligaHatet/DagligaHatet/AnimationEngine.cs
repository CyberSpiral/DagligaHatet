using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DagligaHatet {
    static class AnimationEngine {
        private static List<AnimationClassQueued> queuedAnimations = new List<AnimationClassQueued>();
        private static List<AnimationClass> permanentAnimations = new List<AnimationClass>();

        static public void Update(float elapsed) {

            permanentAnimations.Where(x => x.Animated).ToList().ForEach(x => {
                x.TotalElapsed += elapsed;
                if (x.TotalElapsed > x.TimePerFrame) {
                    x.Frame++;
                    x.Frame = x.Frame % x.TotalFrames;
                    x.TotalElapsed -= x.TimePerFrame;
                }
            });
            queuedAnimations.Where(x => x.Animated).ToList().ForEach(x => {
                x.TotalElapsed += elapsed;
                if (x.TotalElapsed > x.TimePerFrame) {
                    x.Frame++;
                    x.Frame = x.Frame % x.TotalFrames;
                    x.TotalElapsed -= x.TimePerFrame;
                }
            });
            queuedAnimations.Where(x => x.Animated).ToList().ForEach(x => {
                x.ExpirationTime += elapsed;
                if (x.ExpirationDate > x.ExpirationTime) {
                    
                }
            });
        }

        static public void Draw(SpriteBatch spriteBatch) {
            permanentAnimations.Where(x => x.Animated).ToList().ForEach(x => {
                int FrameWidth = x.Texture.Width / x.TotalFrames;
                Rectangle sourcerect = new Rectangle(FrameWidth * x.Frame, 0,
                    FrameWidth, x.Texture.Height);
                spriteBatch.Draw(x.Texture, x.Position, sourcerect, Color.White,
                    0, x.Origin, 1, SpriteEffects.None, 0);

            });
            permanentAnimations.Where(x => !x.Animated).ToList().ForEach(x => spriteBatch.Draw(x.Texture, x.Position, Color.White));
        }

        static public void AddPermanent(string name, Texture2D texture, Vector2 position) {
            permanentAnimations.Add(new DagligaHatet.AnimationClass(name, texture, position));
        }
        static public void AddPermanent(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames) {
            permanentAnimations.Add(new AnimationClass(name, texture, position, origin, timePerFrame, totalFrames));
        }
        static public void AddPermanent(string[] name, Texture2D texture, Vector2[] position, Vector2[] origin, float timePerFrame, int totalFrames) {
            for (int i = 0; i < name.Count(); i++) {
                permanentAnimations.Add(new AnimationClass(name[i], texture, position[i], origin[i], timePerFrame, totalFrames));
            }
        }

        static public void ClearPermanent(string name) {
            permanentAnimations.RemoveAll(x => x.Name == name);
        }
    }
    public class AnimationClass {
        public float TotalElapsed { get; set; } = 0;
        public int Frame { get; set; } = 0;
        public float TimePerFrame { get; }
        public int TotalFrames { get; }

        public bool Hidden { get; } = false;
        public bool Animated { get; }
        public string Name { get; }
        public Texture2D Texture { get; }
        public Rectangle TextureRectangle { get; }
        public Vector2 Position { get; }
        public Vector2 Origin { get; }

        public AnimationClass(string name, Texture2D texture, Vector2 position) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = false;
        }
        public AnimationClass(string name, Texture2D texture, Rectangle position) {
            Texture = texture;
            Name = name;
            TextureRectangle = position;
            Animated = false;
        }
        public AnimationClass(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = true;
            TimePerFrame = timePerFrame;
            TotalFrames = totalFrames;
            Origin = origin;
        }

    }
    class AnimationClassQueued : AnimationClass {
        public new float TotalElapsed { get; set; } = 0;
        public float ExpirationTime { get; set; } = 0;
        public new int Frame { get; set; } = 0;
        public new float TimePerFrame { get; }
        public new int TotalFrames { get; }
        public float ExpirationDate { get; }

        public new bool Hidden { get; } = false;
        public new bool Animated { get; }
        public new string Name { get; }
        public new Texture2D Texture { get; }
        public new Rectangle TextureRectangle { get; }
        public new Vector2 Position { get; }
        public new Vector2 Origin { get; }

        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, float expirationDate) : base(name, texture, position) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = false;
        }
        public AnimationClassQueued(string name, Texture2D texture, Rectangle position, float expirationDate) : base(name, texture, position) {
            Texture = texture;
            Name = name;
            TextureRectangle = position;
            Animated = false;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate) : base(name, texture, position, origin, timePerFrame, totalFrames) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = true;
            TimePerFrame = timePerFrame;
            TotalFrames = totalFrames;
        }
    }
}
