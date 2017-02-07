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
            queuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == queuedAnimations[0].Priority).ToList().ForEach(x => {
                if (x.Animated && x.Name != "Pause") {
                    x.TotalElapsed += elapsed;
                    if (x.TotalElapsed > x.TimePerFrame) {
                        x.Frame++;
                        x.Frame = x.Frame % x.TotalFrames;
                        x.TotalElapsed -= x.TimePerFrame;
                    }
                }
                if (x.Moving && x.Name != "Pause") {
                    x.Position = new Vector2(x.Position.X + x.SpeedX, x.Position.Y + x.SpeedY);
                }
                x.ExpirationTime += elapsed;
                if (x.ExpirationTime > x.ExpirationDate) {
                    x.MarkForDeath = true;
                }
            });
            queuedAnimations.RemoveAll(x => x.MarkForDeath);
        }

        static public void Draw(SpriteBatch spriteBatch) {
            permanentAnimations.Where(x => x.Animated).ToList().ForEach(x => {
                int FrameWidth = x.Texture.Width / x.TotalFrames;
                Rectangle sourcerect = new Rectangle(FrameWidth * x.Frame, 0,
                    FrameWidth, x.Texture.Height);
                spriteBatch.Draw(x.Texture, x.Position, sourcerect, Color.White,
                    0, x.Origin, 1, SpriteEffects.None, 0);

            });
            queuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == queuedAnimations[0].Priority).ToList().Where(x => x.Animated).ToList().Where(x => x.Name != "Pause").ToList().ForEach(x => {
                if (!x.Reversed) {
                    int FrameWidth = x.Texture.Width / x.TotalFrames;
                    Rectangle sourcerect = new Rectangle(FrameWidth * x.Frame, 0,
                        FrameWidth, x.Texture.Height);
                    spriteBatch.Draw(x.Texture, x.Position, sourcerect, Color.White,
                        0, x.Origin, 1, SpriteEffects.None, 0);
                }
                else if (x.Reversed) {
                    int FrameWidth = x.Texture.Width / x.TotalFrames;
                    Rectangle sourcerect = new Rectangle(FrameWidth * (x.TotalFrames - x.Frame), 0,
                        FrameWidth, x.Texture.Height);
                    spriteBatch.Draw(x.Texture, x.Position, sourcerect, Color.White,
                        0, x.Origin, 1, SpriteEffects.None, 0);
                }

            });
            queuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == queuedAnimations[0].Priority).ToList().Where(x => !x.Animated).ToList().Where(x => x.Name != "Pause").ToList().ForEach(x => spriteBatch.Draw(x.Texture, x.Position, Color.White));
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
        static public void AddQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, bool asPrevious) {
            queuedAnimations.OrderBy(x => x.Priority);
            queuedAnimations.Add(new AnimationClassQueued(name, texture, position, origin, expirationDate, asPrevious ? (queuedAnimations.Count == 0 ? 0 : queuedAnimations.Last().Priority) : (queuedAnimations.Count == 0 ? 0 : queuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1)));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious) {
            queuedAnimations.OrderBy(x => x.Priority);
            queuedAnimations.Add(new AnimationClassQueued(name, texture, position, origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (queuedAnimations.Count == 0 ? 0 : queuedAnimations.Last().Priority) : (queuedAnimations.Count == 0 ? 0 : queuedAnimations.Last().Priority + 1),false));
        }
        static public void AddReverseQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious) {
            queuedAnimations.OrderBy(x => x.Priority);
            queuedAnimations.Add(new AnimationClassQueued(name, texture, position, origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (queuedAnimations.Count == 0 ? 0 : queuedAnimations.Last().Priority) : (queuedAnimations.Count == 0 ? 0 : queuedAnimations.Last().Priority + 1),true));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, bool asPrevious, Vector2 goal) {
            queuedAnimations.OrderBy(x => x.Priority);
            queuedAnimations.Add(new AnimationClassQueued(name, texture, position, origin, expirationDate, asPrevious ? (queuedAnimations.Count == 0 ? 0 : queuedAnimations.OrderBy(x => x.Priority).Last().Priority) : (queuedAnimations.Count == 0 ? 0 : queuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1), goal));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious, Vector2 goal) {
            queuedAnimations.OrderBy(x => x.Priority);
            queuedAnimations.Add(new AnimationClassQueued(name, texture, position, origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (queuedAnimations.Count == 0 ? 0 : queuedAnimations.OrderBy(x => x.Priority).Last().Priority) : (queuedAnimations.Count == 0 ? 0 : queuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1), goal));
        }
        static public void AddPause(float expirationDate) {
            queuedAnimations.OrderBy(x => x.Priority);
            queuedAnimations.Add(new AnimationClassQueued(expirationDate, queuedAnimations.Count == 0 ? 0 : queuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1));
        }


        /*static public void AddQueued(string[] name, Texture2D texture, Vector2[] position, Vector2[] origin, float timePerFrame, int totalFrames) {
            for (int i = 0; i < name.Count(); i++) {
                queuedAnimations.Add(new AnimationClass(name[i], texture, position[i], origin[i], timePerFrame, totalFrames));
            }
        }*/

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
        public int Priority { get; }
        public bool MarkForDeath { get; set; } = false;

        public new bool Hidden { get; } = false;
        public new bool Animated { get; }
        public new string Name { get; }
        public new Texture2D Texture { get; }
        public new Rectangle TextureRectangle { get; }
        public new Vector2 Position { get; set; }
        public new Vector2 Origin { get; }
        public bool Moving { get; } = false;
        public Vector2 Goal { get; }
        public float SpeedX { get; }
        public float SpeedY { get; }
        public bool Pause { get; }
        public bool Reversed { get; set; }


        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, int priority) : base(name, texture, position) {
            Texture = texture;
            Name = name;
            Origin = origin;
            Position = position;
            Animated = false;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, int priority, Vector2 goal) : base(name, texture, position) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = false;
            Moving = true;
            Origin = origin;
            Goal = goal;
            ExpirationDate = expirationDate;
            Priority = priority;
            SpeedX = (Goal.X - Position.X) / (ExpirationDate * 60);
            SpeedY = (Goal.Y - Position.Y) / (ExpirationDate * 60);
        }
        public AnimationClassQueued(string name, Texture2D texture, Rectangle position, Vector2 origin, float expirationDate, int priority) : base(name, texture, position) {
            Texture = texture;
            Name = name;
            TextureRectangle = position;
            Animated = false;
            Origin = origin;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, int priority, bool reversed) : base(name, texture, position, origin, timePerFrame, totalFrames) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = true;
            Origin = origin;
            TimePerFrame = timePerFrame;
            TotalFrames = totalFrames;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
            Reversed = reversed;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, int priority, Vector2 goal) : base(name, texture, position) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = true;
            Origin = origin;
            TimePerFrame = timePerFrame;
            TotalFrames = totalFrames;
            Moving = true;
            Goal = goal;
            ExpirationDate = expirationDate;
            Priority = priority;
            SpeedX = (Goal.X - Position.X) / (ExpirationDate * 60);
            SpeedY = (Goal.Y - Position.Y) / (ExpirationDate * 60);
        }
        public AnimationClassQueued(float expirationDate, int priority) : base("Pause", null, Vector2.Zero) {
            ExpirationDate = expirationDate;
            Priority = priority;
            Name = "Pause";
        }
    }
}
