using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DagligaHatet {

    static class World {
        public static List<Tile> Map { get; } = new List<Tile>();
        public static List<Character> AllCharacters { get; set; } = new List<Character>();
        public static BillBoard Board { get; set; }
        public static states phase = states.ChoosePhase;
        public static MouseState oldMouse;
        private static Button moveButton;
        private static Button attackButton;
        private static Button skillButton;
        private static Button skipButton;
        /// <summary>
        /// 0. Move 1. Attack 2. Skill 3. Skip
        /// </summary>
        public static List<Button> Buttons { get; set; } = new List<Button>();

        public static List<AnimatedTextureMould> TheMould = new List<AnimatedTextureMould>();

        public static int OrderNumber { get; set; } = 0;



        public static void Prepare(ContentManager content) {
            for (int x = 0; x < 20; x++) {
                for (int y = 0; y < 16; y++) {
                    Map.Add(new Tile(new Vector2(x, y), content.Load<Texture2D>("Bushes and dirt_0"), content.Load<Texture2D>("Bushes and dirt_6")));
                }
            }
            Board = new BillBoard(content.Load<Texture2D>("Pinboard"), content.Load<Texture2D>("Art"), content.Load<Texture2D>("Dot"), new Vector2(100, 730));

            moveButton = new Button(new Rectangle(120, 10, 100, 60), content.Load<Texture2D>("Move"), "moveButton");
            attackButton = new Button(new Rectangle(320, 10, 100, 60), content.Load<Texture2D>("Attack"), "attackButton");
            skillButton = new Button(new Rectangle(520, 10, 100, 60), content.Load<Texture2D>("Skill"), "skillButton");
            skipButton = new Button(new Rectangle(720, 10, 100, 60), content.Load<Texture2D>("Skip"), "skipButton");
            Buttons.Add(moveButton);
            Buttons.Add(attackButton);
            Buttons.Add(skillButton);
            Buttons.Add(skipButton);

            TheMould.Add(new AnimatedTextureMould(content.Load<Texture2D>("MoveAni"), "Move", Vector2.Zero, 0, 0, 2, 0.4f));

            Tile tempTile;

            tempTile = GetTile(new Vector2(2, 3));
            tempTile.AddInhabitor(new Player(content.Load<Texture2D>("Knight1"), "Knight", new AttackMelee(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 3 /*Range*/, 3 /*Damage*/, new SkillKnightWhirlwind(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("WhirlwindAni")), 2 /*Skill Range*/, 2 /*Skill Damage*/, 4 /*Movement Speedu*/, 14 /*Health*/, 0/*Alignment*/));
            AllCharacters.Add(tempTile.Inhabitant);

            tempTile = GetTile(new Vector2(3, 3));
            tempTile.AddInhabitor(new Player(content.Load<Texture2D>("Wizard1"), "Wizard", new AttackRangeCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Fieer"),
                content.Load<Texture2D>("Cross")), 4 /*Range*/, 3 /*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                100 /*Skill Range*/, 3 /*Skill Damage*/, 5 /*Movement Speed*/, 10 /*Health*/, 0/*Alignment*/));
            AllCharacters.Add(tempTile.Inhabitant);

            tempTile = GetTile(new Vector2(2, 4));
            tempTile.AddInhabitor(new Player(content.Load<Texture2D>("Ranger1"), "Ranger", new AttackRangeXCross(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Botched Arrow"), content.Load<Texture2D>("Cross")), 5 /*Range*/, 2 /*Damage*/, new SkillRangerBomb(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Ex"), content.Load<Texture2D>("Botched Arrow"), content.Load<Texture2D>("Target")), 4 /*Skill Range*/, 3 /*Skill Damage*/, 6 /*Movement Speed*/, 13 /*Health*/, 0/*Alignment*/));
            AllCharacters.Add(tempTile.Inhabitant);


            tempTile = GetTile(new Vector2(9, 9));
            tempTile.AddInhabitor(new Evil(content.Load<Texture2D>("S"), "Skeleton", new AttackRangeCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Fieer"),
                content.Load<Texture2D>("Cross")), 5/*Range*/, 3/*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                3 /*Skill Range*/, 2/*Skill Damage*/, 4/*Movement Speed*/, 14/*Health*/, 1/*Alignment*/));
            tempTile.Inhabitant.Rotation = (float)Math.PI;
            AllCharacters.Add(tempTile.Inhabitant);

            tempTile = GetTile(new Vector2(13, 10));
            tempTile.AddInhabitor(new Evil(content.Load<Texture2D>("S2"), "Frost demon", new AttackMelee(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 3/*Range*/, 4/*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                3 /*Skill Range*/, 2/*Skill Damage*/, 4/*Movement Speed*/, 18/*Health*/, 1/*Alignment*/));
            tempTile.Inhabitant.Rotation = (float)Math.PI;
            AllCharacters.Add(tempTile.Inhabitant);

            tempTile = GetTile(new Vector2(12, 12));
            tempTile.AddInhabitor(new Evil(content.Load<Texture2D>("S3"), "Wraith", new AttackRangeXCross(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Botched Arrow"), content.Load<Texture2D>("Cross")), 4/*Range*/, 3/*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                3 /*Skill Range*/, 2/*Skill Damage*/, 5/*Movement Speed*/, 10/*Health*/, 1/*Alignment*/));
            tempTile.Inhabitant.Rotation = (float)Math.PI;
            AllCharacters.Add(tempTile.Inhabitant);

            for (int i = 0; i < 15; i++) {
                GetTile(new Vector2(7, i)).Cement(content.Load<Texture2D>("Bushes and dirt_1"));
            }

            var rnd = new Random();
            AllCharacters = AllCharacters.OrderBy(x => rnd.Next()).ToList();
        }

        /// <summary>
        /// Translate a mapcoordinate into it's actual position
        /// </summary>
        /// <param name="mapCoordinate">Your mapposition coordinate</param>
        /// <returns>Your position in pixels</returns>
        public static Vector2 TranslateMapCoordinate(Vector2 mapCoordinate) {
            return new Vector2(mapCoordinate.X * 40 + 100, mapCoordinate.Y * 40 + 80);
        }

        /// <summary>
        /// Translate the middle of a tile on the selected mapcoordinate into it's actual position
        /// </summary>
        /// <param name="mapCoordinate">Your mapcoordinate</param>
        /// <returns>Your position in pixels</returns>
        public static Vector2 TranslateMapMiddleMapCoordinate(Vector2 mapCoordinate) {
            return new Vector2(mapCoordinate.X * 40 + 120, mapCoordinate.Y * 40 + 100);
        }

        /// <summary>
        /// Translate a position into it's respective mapcoordinate
        /// </summary>
        /// <param name="Position">Your position in pixels</param>
        /// <returns>Your mapcoordinate</returns>
        public static Vector2 TranslatePosition(Vector2 Position) {
            return new Vector2((Position.X - 100) / 40, (Position.Y - 80) / 40);
        }

        public static float GetRotation(Vector2 pos1, Vector2 pos2) {
            return (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
        }

        public static Tile GetTile(Vector2 mapPosition) {
            return Map.Find(x => x.MapCoordinate == mapPosition);
        }

        public static float Distance(Vector2 v1, Vector2 v2) {
            return (Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y));
        }

        public static Tuple<List<Tile>, bool> Path2(List<Tile> acceptable, int distance, Tile start, Tile stop) {
            List<Flood> floodList = new List<Flood>();
            acceptable.Add(start);
            acceptable.Add(stop);
            Map.ForEach(x => {
                if (acceptable.Contains(x))
                    floodList.Add(new Flood(x.MapCoordinate, 0));
                else
                    floodList.Add(new Flood(x.MapCoordinate, -1));
            });
            floodList.ForEach(x => x.FillNeighbours(floodList));

            floodList.Find(x => x.MapPosition == start.MapCoordinate).Steps = distance;
            for (int i = distance; i > 0; i--) {
                floodList.Where(x => x.Steps == i).ToList().ForEach(x => {
                    if (x.NeighbourLeft != null && x.NeighbourLeft.Steps == 0) {
                        x.NeighbourLeft.Steps = i - 1;
                    }
                    if (x.NeighbourRight != null && x.NeighbourRight.Steps == 0) {
                        x.NeighbourRight.Steps = i - 1;
                    }
                    if (x.NeighbourTop != null && x.NeighbourTop.Steps == 0) {
                        x.NeighbourTop.Steps = i - 1;
                    }
                    if (x.NeighbourDown != null && x.NeighbourDown.Steps == 0) {
                        x.NeighbourDown.Steps = i - 1;
                    }
                });
            }

            for (int y = 0; y < 200; y++) {
                if (floodList.Find(x => x.MapPosition == start.MapCoordinate).Steps == -1) {
                    Map.ForEach(x => {
                        if (acceptable.Contains(x))
                            floodList.Add(new Flood(x.MapCoordinate, 0));
                        else
                            floodList.Add(new Flood(x.MapCoordinate, -1));
                    });
                    floodList.ForEach(x => x.FillNeighbours(floodList));

                    floodList.Find(x => x.MapPosition == start.MapCoordinate).Steps = distance;
                    for (int i = distance; i > 0; i--) {
                        floodList.Where(x => x.Steps == i).ToList().ForEach(x => {
                            if (x.NeighbourLeft != null && x.NeighbourLeft.Steps == 0) {
                                x.NeighbourLeft.Steps = i - 1;
                            }
                            if (x.NeighbourRight != null && x.NeighbourRight.Steps == 0) {
                                x.NeighbourRight.Steps = i - 1;
                            }
                            if (x.NeighbourTop != null && x.NeighbourTop.Steps == 0) {
                                x.NeighbourTop.Steps = i - 1;
                            }
                            if (x.NeighbourDown != null && x.NeighbourDown.Steps == 0) {
                                x.NeighbourDown.Steps = i - 1;
                            }
                        });
                    }
                }
                Random r = new Random();
                List<Flood> path = new List<Flood>();
                bool end = false;
                path.Add(floodList.Find(x => x.MapPosition == start.MapCoordinate));
                while (!end) {
                    float differenceX = path.Last().MapPosition.X - stop.MapCoordinate.X;
                    float differenceY = path.Last().MapPosition.Y - stop.MapCoordinate.Y;
                    List<Flood> eliglebleMoves = path.Last().Neighbours.Where(x => x != null && x.Steps == path.Last().Steps - 1).ToList();
                    if (eliglebleMoves.Count > 0) {
                        if (differenceX < 0 && eliglebleMoves.Contains(path.Last().NeighbourRight)) {
                            path.Add(path.Last().NeighbourRight);
                        }
                        else if (differenceX > 0 && eliglebleMoves.Contains(path.Last().NeighbourLeft)) {
                            path.Add(path.Last().NeighbourLeft);
                        }
                        else if (differenceY > 0 && eliglebleMoves.Contains(path.Last().NeighbourTop)) {
                            path.Add(path.Last().NeighbourTop);
                        }
                        else if (differenceY < 0 && eliglebleMoves.Contains(path.Last().NeighbourDown)) {
                            path.Add(path.Last().NeighbourDown);
                        }
                        else {
                            path.Add(eliglebleMoves[r.Next(eliglebleMoves.Count)]);
                        }
                        if (path.Last().MapPosition == stop.MapCoordinate) {
                            List<Tile> tilePath = new List<Tile>();
                            path.ForEach(x => tilePath.Add(World.GetTile(x.MapPosition)));
                            return new Tuple<List<Tile>, bool>(tilePath, true);
                        }
                    }
                    else {
                        if (path.Last().MapPosition == stop.MapCoordinate) {
                            List<Tile> tilePath = new List<Tile>();
                            path.ForEach(x => tilePath.Add(World.GetTile(x.MapPosition)));
                            return new Tuple<List<Tile>, bool>(tilePath, true);
                        }
                        else {
                            path.Last().Steps = -1;
                            floodList.ForEach(x => x.FillNeighbours(floodList));
                            end = true;
                        }
                    }
                }
            }

            return null;
        }

        public static List<Tile> FloodPath(int distance, Tile start) {
            List<Flood> floodList = new List<Flood>();
            Map.ForEach(x => {
                if (x.Inhabited || x.Occupied)
                    floodList.Add(new Flood(x.MapCoordinate, -1));
                else
                    floodList.Add(new Flood(x.MapCoordinate, 0));
            });
            floodList.Find(x => x.MapPosition == start.MapCoordinate).Steps = distance;
            for (int i = distance; i > 0; i--) {
                floodList.Where(x => x.Steps == i).ToList().ForEach(x => {
                    if (floodList.Exists(y => y.MapPosition == new Vector2(x.MapPosition.X - 1, x.MapPosition.Y) && y.Steps != -1 && y.Steps == 0)) {
                        floodList.Find(y => y.MapPosition == new Vector2(x.MapPosition.X - 1, x.MapPosition.Y)).Steps = i - 1;
                    }
                    if (floodList.Exists(y => y.MapPosition == new Vector2(x.MapPosition.X + 1, x.MapPosition.Y) && y.Steps != -1 && y.Steps == 0)) {
                        floodList.Find(y => y.MapPosition == new Vector2(x.MapPosition.X + 1, x.MapPosition.Y)).Steps = i - 1;
                    }
                    if (floodList.Exists(y => y.MapPosition == new Vector2(x.MapPosition.X, x.MapPosition.Y + 1) && y.Steps != -1 && y.Steps == 0)) {
                        floodList.Find(y => y.MapPosition == new Vector2(x.MapPosition.X, x.MapPosition.Y + 1)).Steps = i - 1;
                    }
                    if (floodList.Exists(y => y.MapPosition == new Vector2(x.MapPosition.X, x.MapPosition.Y - 1) && y.Steps != -1 && y.Steps == 0)) {
                        floodList.Find(y => y.MapPosition == new Vector2(x.MapPosition.X, x.MapPosition.Y - 1)).Steps = i - 1;
                    }
                });
            }
            List<Tile> tempList = new List<Tile>();
            for (int i = 0; i < floodList.Where(x => x.Steps > 0).Count(); i++) {
                Map.ForEach(x => {
                    if (x.MapCoordinate == floodList.Where(y => y.Steps > 0).ToList()[i].MapPosition)
                        tempList.Add(x);
                });
            }
            return tempList;
        }

        public static void DoDamage(int damage, Character turnMaster, Character reciver) {
            reciver.Health -= damage;
            Console.WriteLine("The {0} does {1} damage to the {2}, their health is now {3}", turnMaster.Name, damage, reciver.Name, reciver.Health);
            World.Board.Pin(string.Format("The {0} does {1} damage to the {2}, their health is now {3}", turnMaster.Name, damage, reciver.Name, reciver.Health));
            Vector2 translated = TranslateMapCoordinate(reciver.MapCoordinate);
            Vector2 origin = new Vector2(translated.X - 10, translated.Y - 10);
            Vector2 goal = new Vector2(translated.X - 10, translated.Y - 20);
            DrawEngine.AddDamageReport(string.Format("-{0}", damage), Color.Red, TranslatePosition(origin), 1f, false, TranslatePosition(goal));
        }
        public static void DoHealing(int heal, Character turnMaster, Character reciver) {
            int difference = reciver.MaxHealth - reciver.Health;
            if (difference > heal) {
                reciver.Health += heal;
                Console.WriteLine("The {0} heals the {1} for {2}, their health is now {3}", turnMaster.Name, reciver.Name, heal, reciver.Health);
                World.Board.Pin(string.Format("The {0} heals {1} for {2}, their health is now {3}", turnMaster.Name, reciver.Name, heal, reciver.Health));
                Vector2 translated = TranslateMapCoordinate(reciver.MapCoordinate);
                Vector2 origin = new Vector2(translated.X - 10, translated.Y - 10);
                Vector2 goal = new Vector2(translated.X - 10, translated.Y - 20);
                DrawEngine.AddDamageReport(string.Format("+{0}", heal), Color.ForestGreen, TranslatePosition(origin), 1f, false, TranslatePosition(goal));
            }
            else {
                reciver.Health += difference;
                Console.WriteLine("The {0} heals {1} for {2}, their health is now {3}", turnMaster.Name, reciver.Name, difference, reciver.Health);
                World.Board.Pin(string.Format("The {0} heals the {1} for {2}, their health is now {3}", turnMaster.Name, reciver.Name, difference, reciver.Health));
                Vector2 translated = TranslateMapCoordinate(reciver.MapCoordinate);
                Vector2 origin = new Vector2(translated.X - 10, translated.Y - 10);
                Vector2 goal = new Vector2(translated.X - 10, translated.Y - 20);
                DrawEngine.AddDamageReport(string.Format("+{0}", difference), Color.ForestGreen, TranslatePosition(origin), 1f, false, TranslatePosition(goal));
            }
        }

    }
    public class Flood {
        public Vector2 MapPosition { get; }

        public Flood NeighbourTop { get; set; }
        public Flood NeighbourDown { get; set; }
        public Flood NeighbourLeft { get; set; }
        public Flood NeighbourRight { get; set; }
        public List<Flood> Neighbours { get; } = new List<Flood>();

        public int Steps { get; set; }
        public Flood(Vector2 mapPosition, int steps) {
            Steps = steps;
            MapPosition = mapPosition;
        }
        public void FillNeighbours(List<Flood> list) {
            NeighbourRight = null;
            NeighbourLeft = null;
            NeighbourTop = null;
            NeighbourDown = null;
            list.ForEach(x => {
                if (x.MapPosition == new Vector2(MapPosition.X + 1, MapPosition.Y)) {
                    NeighbourRight = x;
                }
                else if (x.MapPosition == new Vector2(MapPosition.X - 1, MapPosition.Y)) {
                    NeighbourLeft = x;
                }
                else if (x.MapPosition == new Vector2(MapPosition.X, MapPosition.Y + 1)) {
                    NeighbourDown = x;
                }
                else if (x.MapPosition == new Vector2(MapPosition.X, MapPosition.Y - 1)) {
                    NeighbourTop = x;
                }
            });
            Neighbours.Add(NeighbourDown);
            Neighbours.Add(NeighbourTop);
            Neighbours.Add(NeighbourLeft);
            Neighbours.Add(NeighbourRight);
        }
    }
    public class Tile {
        public Vector2 MapCoordinate { get; set; }
        public Rectangle Collision { get { return new Rectangle((int)World.TranslateMapCoordinate(MapCoordinate).X, (int)World.TranslateMapCoordinate(MapCoordinate).Y, 40, 40); } }

        public bool Occupied { get; set; } = false;
        public bool Inhabited { get; set; } = false;
        public Character Inhabitant { get; set; } = null;
        private Texture2D Texture { get; set; }
        private Texture2D Border { get; }


        public void MoveInhabited(Tile target) {
            target.AddInhabitor(Inhabitant);
            Tuple<List<Tile>, bool> path = World.Path2(Inhabitant.SelectedTiles, Inhabitant.MoveSpeed, this, target);
            if (path.Item2) {
                for (int i = 0; i < path.Item1.Count - 1; i++) {
                    DrawEngine.AddQueued(Inhabitant.Name, Inhabitant.Texture, path.Item1[i].MapCoordinate, new Vector2(20, 20), 0.3f, false, path.Item1[i + 1].MapCoordinate);
                }
                Vector2 goal = World.TranslateMapCoordinate(target.MapCoordinate);
                Vector2 position = World.TranslateMapCoordinate(path.Item1[path.Item1.Count - 2].MapCoordinate);
                target.Inhabitant.Rotation = (float)Math.Atan2(goal.Y - position.Y, goal.X - position.X);
            }
            RemoveInhabitor();
        }

        public Tile(Vector2 mapPosition, Texture2D texture, Texture2D borderTexture) {
            MapCoordinate = mapPosition;
            Texture = texture;
            Border = borderTexture;
        }
        public void Cement(Texture2D texture) {
            Occupied = true;
            Texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Texture, Collision, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(Border, Collision, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.99f);
        }
        public void AddInhabitor(Character inhabitor) {
            inhabitor.Inhabited = this;
            Inhabitant = inhabitor;
            Inhabited = true;
        }
        public void RemoveInhabitor() {
            Inhabitant = null;
            Inhabited = false;
        }
    }
}
