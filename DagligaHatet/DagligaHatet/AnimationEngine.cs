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
        public static List<TextQueued> QueuedText { get; set; } = new List<TextQueued>();

        static public void Update(float elapsed) {


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
                    Vector2 direction = new Vector2((float)Math.Cos(x.Rotation), (float)Math.Sin(x.Rotation));
                    x.Position += direction * x.Speed;
                }
                x.ExpirationTime += elapsed;
                if (x.ExpirationTime > x.ExpirationDate) {
                    x.MarkForDeath = true;
                }

            });

            QueuedText.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedText[0].Priority).ToList().ForEach(x => {
                if (x.Moving && x.Name != "Pause") {
                    Vector2 direction = new Vector2((float)Math.Cos(x.Rotation), (float)Math.Sin(x.Rotation));

                    x.Position += direction * x.Speed;
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
            World.Board.Draw(spriteBatch, font);


            QueuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedAnimations[0].Priority).Where(x => !x.Hidden)
                .Where(x => x.Animated).Where(x => x.Name != "Pause").ToList().ForEach(x => {
                    if (!x.Reversed) {
                        int FrameWidth = x.Texture.Width / x.TotalFrames;
                        Rectangle sourcerect = new Rectangle(FrameWidth * x.Frame, 0,
                            FrameWidth, x.Texture.Height);
                        spriteBatch.Draw(x.Texture, x.Position, sourcerect, Color.White,
                            x.Rotation, x.Origin, 1, SpriteEffects.None, 0);
                    }
                    else if (x.Reversed) {
                        int FrameWidth = x.Texture.Width / x.TotalFrames;
                        Rectangle sourcerect = new Rectangle(FrameWidth * (x.TotalFrames - x.Frame), 0,
                            FrameWidth, x.Texture.Height);
                        spriteBatch.Draw(x.Texture, x.Position, sourcerect, Color.White,
                            x.Rotation, x.Origin, 1, SpriteEffects.None, 0);
                    }

                });
            QueuedAnimations.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedAnimations[0].Priority).ToList().Where(x => !x.Animated).ToList().Where(x => x.Name != "Pause").ToList()
                .Where(x => !x.Hidden).ToList().ForEach(x => spriteBatch.Draw(x.Texture, x.Position, null, Color.White,
                            x.Rotation, x.Origin, 1, SpriteEffects.None, 0));

            QueuedText.OrderBy(x => x.Priority).Where(x => x.Priority == QueuedText[0].Priority).ToList().Where(x => !x.Animated).ToList().Where(x => x.Name != "Pause").ToList()
                .Where(x => !x.Hidden).ToList().ForEach(x => spriteBatch.DrawString(font, x.Name, x.Position, x.Color));

            World.AllCharacters.ForEach(x => x.Draw(spriteBatch));

        }

        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float expirationDate, bool asPrevious) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapCoordinate(mapPosition), origin, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1)));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapCoordinate(mapPosition), origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority + 1), false));
        }
        static public void AddReverseQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapCoordinate(mapPosition), origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.Last().Priority + 1), true));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float expirationDate, bool asPrevious, Vector2 goal) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapCoordinate(mapPosition), origin, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1), World.TranslateMapCoordinate(goal)));
        }
        static public void AddQueued(string name, Texture2D texture, Vector2 mapPosition, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, bool asPrevious, Vector2 goal) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(name, texture, World.TranslateMapCoordinate(mapPosition), origin, timePerFrame, totalFrames, expirationDate, asPrevious ? (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority) : (QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1), World.TranslateMapCoordinate(goal)));
        }
        static public void AddPause(float expirationDate) {
            QueuedAnimations = QueuedAnimations.OrderBy(x => x.Priority).ToList();
            QueuedAnimations.Add(new AnimationClassQueued(expirationDate, QueuedAnimations.Count == 0 ? 0 : QueuedAnimations.OrderBy(x => x.Priority).Last().Priority + 1));
        }
        static public void AddDamageReport(string name, Color color, Vector2 mapPosition, float expirationDate, bool asPrevious, Vector2 goal) {
            QueuedText = QueuedText.OrderBy(x => x.Priority).ToList();
            QueuedText.Add(new TextQueued(name, World.TranslateMapCoordinate(mapPosition), expirationDate, asPrevious ? (QueuedText.Count == 0 ? 0 : QueuedText.OrderBy(x => x.Priority).Last().Priority) : (QueuedText.Count == 0 ? 0 : QueuedText.OrderBy(x => x.Priority).Last().Priority + 1), World.TranslateMapCoordinate(goal), color));
        }


        /*static public void AddQueued(string[] name, Texture2D texture, Vector2[] position, Vector2[] origin, float timePerFrame, int totalFrames) {
            for (int i = 0; i < name.Count(); i++) {
                queuedAnimations.Add(new AnimationClass(name[i], texture, position[i], origin[i], timePerFrame, totalFrames));
            }
        }*/

    }
    class AnimationClassQueued {
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

        public float ExpirationDate { get; }
        public float ExpirationTime { get; set; }
        public int Priority { get; }
        public bool MarkForDeath { get; set; } = false;

        public bool Moving { get; } = false;
        public Vector2 Goal { get; }
        public float Speed { get; }
        public bool Pause { get; }
        public bool Reversed { get; set; }
        public float Rotation { get; set; }


        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, int priority) {
            Texture = texture;
            Name = name;
            Origin = origin;
            Position = new Vector2(position.X + 20, position.Y + 20);
            Animated = false;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float expirationDate, int priority, Vector2 goal) {
            Texture = texture;
            Name = name;
            Position = new Vector2(position.X + 20, position.Y + 20);
            Animated = false;
            Moving = true;
            Origin = origin;
            Goal = goal;
            ExpirationDate = expirationDate;
            Priority = priority;
            Speed = World.Distance(position, goal) / (ExpirationDate * 60);
            Rotation = (float)Math.Atan2(goal.Y - position.Y, goal.X - position.X);

            /*Vector2 difference = Goal - Position;
            difference.Normalize();
            Rotation = (float)Math.Acos(Vector2.Dot(difference, new Vector2(1, 0)));*/

        }
        public AnimationClassQueued(string name, Texture2D texture, Rectangle position, Vector2 origin, float expirationDate, int priority) {
            Texture = texture;
            Name = name;
            Position = new Vector2(position.X + 20, position.Y + 20);
            Animated = false;
            Origin = origin;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, int priority, bool reversed) {
            Texture = texture;
            Name = name;
            Position = new Vector2(position.X + 20, position.Y + 20);
            Animated = true;
            Origin = origin;
            TimePerFrame = timePerFrame;
            TotalFrames = totalFrames;
            Moving = false;
            ExpirationDate = expirationDate;
            Priority = priority;
            Reversed = reversed;
        }
        public AnimationClassQueued(string name, Texture2D texture, Vector2 position, Vector2 origin, float timePerFrame, int totalFrames, float expirationDate, int priority, Vector2 goal) {
            Texture = texture;
            Name = name;
            Position = new Vector2(position.X + 20, position.Y + 20);
            Animated = true;
            Origin = origin;
            TimePerFrame = timePerFrame;
            TotalFrames = totalFrames;
            Moving = true;
            Goal = goal;
            ExpirationDate = expirationDate;
            Priority = priority;
            Speed = World.Distance(position, goal) / (ExpirationDate * 60);
            Rotation = (float)Math.Atan2(goal.Y - position.Y, goal.X - position.X);
        }
        public AnimationClassQueued(float expirationDate, int priority) {
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
    class BillBoard {

        private List<string> Pinned = new List<string>();
        public Vector2 Position { get; }
        public Vector2 TextPosition1 { get { return new Vector2(Position.X + 440, Position.Y + 15); } }
        public Vector2 TextPosition2 { get { return new Vector2(Position.X + 440, Position.Y + 30); } }
        private Texture2D texture;
        private Texture2D painting;
        private Texture2D dot;

        public BillBoard(Texture2D texture, Texture2D art, Texture2D dot, Vector2 position) {
            this.texture = texture;
            painting = art;
            this.dot = dot;
            Position = position;
            Pinned.Add("");
            Pinned.Add("");
        }

        public void Pin(string text) {
            Pinned.Add(text);
            Pinned.Remove(Pinned.First());
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font) {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, Pinned[0], TextPosition1, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, Pinned[1], TextPosition2, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            float spacing = 420 / World.AllCharacters.Count;
            for (int i = 0; i < World.AllCharacters.Count; i++) {
                spriteBatch.Draw(painting, new Vector2(Position.X + 15 + (spacing * i), Position.Y + 7), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                spriteBatch.Draw(World.AllCharacters[i].Texture, new Vector2(Position.X + 17 + (spacing * i), Position.Y + 9), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(dot, new Rectangle((int)(Position.X + 17 + (spacing * i)), (int)Position.Y + 52, 40, 4), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
                float percent = (float)World.AllCharacters[i].Health / (float)World.AllCharacters[i].MaxHealth;
                spriteBatch.Draw(dot, new Rectangle((int)(Position.X + 18 + (spacing * i)), (int)Position.Y + 53, (int)(38 * percent), 2), null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 0.8f);
            }

        }

    }
}
