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
    static class DrawEngine {
        public static List<AnimationClassQueued> QueuedAnimations { get; set; } = new List<AnimationClassQueued>();
        public static List<AnimationClass> PermanentAnimations { get; } = new List<AnimationClass>();
        public static List<TextQueued> QueuedText { get; set; } = new List<TextQueued>();

        static public void Update(float elapsed) {

            PermanentAnimations.Where(x => x.Animated).ToList().ForEach(x => {
                x.TotalElapsed += elapsed;
                if (x.TotalElapsed > x.TimePerFrame) {
                    x.Frame++;
                    x.Frame = x.Frame % x.TotalFrames;
                    x.TotalElapsed -= x.TimePerFrame;
                }
            });

            QueuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedAnimations[0].Priority).ToList().ForEach(x => {

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

            QueuedText.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedText[0].Priority).ToList().ForEach(x => {
                if (x.Moving && x.Name != "Pause") {
                    x.Position = new Vector2(x.Position.X + x.SpeedX, x.Position.Y + x.SpeedY);
                }
                x.ExpirationTime += elapsed;
                if (x.ExpirationTime > x.ExpirationDate) {
                    x.MarkForDeath = true;
                }
            });

            QueuedAnimations.RemoveAll(x => x.MarkForDeath);

            QueuedText.RemoveAll(x => x.MarkForDeath);
        }

        static public void Draw(SpriteBatch spriteBatch, SpriteFont font) {
            PermanentAnimations.Where(x => x.Animated).ToList().Where(x => !x.Hidden).ToList().ForEach(x => {
                if (!QueuedAnimations.Exists(y => y.Name == x.Name)) {
                    int FrameWidth = x.Texture.Width / x.TotalFrames;
                    Rectangle sourcerect = new Rectangle(FrameWidth * x.Frame, 0,
                        FrameWidth, x.Texture.Height);
                    spriteBatch.Draw(x.Texture, x.Position, sourcerect, Color.White,
                        0, x.Origin, 1, SpriteEffects.None, x.Layer);
                }
            });
            PermanentAnimations.Where(x => !x.Animated).ToList().Where(x => !x.Hidden).ToList().ForEach(x => {
                if (!QueuedAnimations.Exists(y => y.Name == x.Name)) {
                    spriteBatch.Draw(x.Texture, x.Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, x.Layer);
                }
            });

            QueuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedAnimations[0].Priority).Where(x => !x.Hidden)
                .Where(x => x.Animated).Where(x => x.Name != "Pause").ToList().ForEach(x => {
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
            QueuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedAnimations[0].Priority).ToList().Where(x => !x.Animated).ToList().Where(x => x.Name != "Pause").ToList()
                .Where(x => !x.Hidden).ToList().ForEach(x => spriteBatch.Draw(x.Texture, x.Position, Color.White));

            QueuedText.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedText[0].Priority).ToList().Where(x => !x.Animated).ToList().Where(x => x.Name != "Pause").ToList()
                .Where(x => !x.Hidden).ToList().ForEach(x => spriteBatch.DrawString(font, x.Name, x.Position, x.Color));

        }

        static public void AddPermanent(string name, Texture2D texture, Vector2 mapPosition, float layer) {
            PermanentAnimations.Add(new AnimationClass(name, texture, World.TranslateMapPosition(mapPosition), layer));
        }
        static public void AddPermanent(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float timePerFrame, int totalFrames, float layer) {
            PermanentAnimations.Add(new AnimationClass(name, texture, World.TranslateMapPosition(mapPosition), origin, timePerFrame, totalFrames, layer));
        }
        static public void AddPermanent(string[] name, Texture2D texture, Vector2[] mapPosition, Vector2[] origin, float timePerFrame, int totalFrames, float layer) {
            for (int i = 0; i < name.Count(); i++) {
                PermanentAnimations.Add(new AnimationClass(name[i], texture, World.TranslateMapPosition(mapPosition[i]), origin[i], timePerFrame, totalFrames, layer));
            }
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float expirationDate, bool asPrevious) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapPosition(mapPosition), origin, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1)));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapPosition(mapPosition), origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority + 1), false));
        }
        static public void AddReverseQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapPosition(mapPosition), origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority + 1), true));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float expirationDate, bool asPrevious, Vector2 goal) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapPosition(mapPosition), origin, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1), World.TranslateMapPosition(goal)));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious, Vector2 goal) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapPosition(mapPosition), origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1), World.TranslateMapPosition(goal)));
        }
        static public void AddPause(float expirationDate) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(expirationDate, QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1));
        }
        static public void AddDamageReport(string name, Color color, Vector2 mapPosition, float expirationDate, bool asPrevious, Vector2 goal) {
            QueuedText = QueuedText.OrderBy(x => x.Priority).ToList();
            QueuedText.Add(new TextQueued(name, World.TranslateMapPosition(mapPosition), expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1), World.TranslateMapPosition(goal), color));
        }


        /*static public void AddQueued(string[] name, Texture2D texture, Vector2[] position, Vector2[] origin, float timePerFrame, int totalFrames) {
            for (int i = 0; i < name.Count(); i++) {
                queuedAnimations.Add(new AnimationClass(name[i], texture, position[i], origin[i], timePerFrame, totalFrames));
            }
        }*/

        static public void ClearPermanent(string name) {
            PermanentAnimations.RemoveAll(x => x.Name == name);
        }
    }
    class AnimationClass {
        public float TotalElapsed { get; set; } = 0;
        public int Frame { get; set; } = 0;
        public float TimePerFrame { get; }
        public int TotalFrames { get; }
        public float Layer { get; }

        public bool Hidden { get; set; } = false;
        public bool Animated { get; }
        public string Name { get; }
        public Texture2D Texture { get; }
        public Rectangle TextureRectangle { get; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; }

        public AnimationClass(string name, Texture2D texture, Vector2 position, float layer) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = false;
            Layer = layer;
        }
        public AnimationClass(string name, Texture2D texture, Rectangle position, float layer) {
            Texture = texture;
            Name = name;
            TextureRectangle = position;
            Animated = false;
            Layer = layer;
        }
        public AnimationClass(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float layer) {
            Texture = texture;
            Name = name;
            Position = position;
            Animated = true;
            TimePerFrame = timePerFrame;
            TotalFrames = totalFrames;
            Origin = origin;
            Layer = layer;
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

        public new bool Hidden { get; set; } = false;
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


        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, int priority) : base(name, texture, position, 0) {
            Texture = texture;
            Name = name;
            Origin = origin;
            Position = position;
            Animated = false;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, int priority, Vector2 goal) : base(name, texture, position, 0) {
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
        public AnimationClassQueued(string name, Texture2D texture, Rectangle position, Vector2 origin, float expirationDate, int priority) : base(name, texture, position, 0) {
            Texture = texture;
            Name = name;
            TextureRectangle = position;
            Animated = false;
            Origin = origin;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, int priority, bool reversed) : base(name, texture, position, origin, timePerFrame, totalFrames, 0) {
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
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, int priority, Vector2 goal) : base(name, texture, position, 0) {
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
        public AnimationClassQueued(float expirationDate, int priority) : base("Pause", null, Vector2.Zero, 0) {
            ExpirationDate = expirationDate;
            Priority = priority;
            Name = "Pause";
        }
    }
    class TextQueued : AnimationClassQueued {
        public Color Color { get; }
        public TextQueued(string name, Vector2 position, float expirationDate, int priority, Vector2 goal, Color color) : base(name, null, position, Vector2.Zero, expirationDate, priority, goal) {
            Color = color;
        }
    }
}
