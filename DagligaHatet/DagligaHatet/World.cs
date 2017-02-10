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
        public static List<Tile> SelectedTiles { get; set; } = new List<Tile>();

        public static List<string> Order { get; } = new List<string>();
        public static int OrderNumber { get; set; } = 0;



        public static void Prepare(ContentManager content) {
            for (int x = 0; x < 20; x++) {
                for (int y = 0; y < 16; y++) {
                    Map.Add(new Tile(new Vector2(x, y)));
                }
            }
            Tile tempTile;

            tempTile = GetTile(new Vector2(1, 3));
            Order.Add("Knight");
            tempTile.AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Knight1"), "Knight", new AttackMelee(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 3 /*Range*/, 3 /*Damage*/, new SkillKnightWhirlwind(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("WhirlwindAni")), 2 /*Skill Range*/, 2 /*Skill Damage*/, 4 /*Movement Speedu*/, 10 /*Health*/, 0/*Alignment*/));
            DrawEngine.AddPermanent(tempTile.Inhabitant.Name, tempTile.Inhabitant.Texture, tempTile.MapPosition, 0.5f);

            tempTile = GetTile(new Vector2(2, 3));
            Order.Add("S1");
            tempTile.AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("S"), "S1", new AttackRangeCross(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 5/*Range*/, 3/*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                3 /*Skill Range*/, 2/*Skill Damage*/, 4/*Movement Speed*/, 14/*Health*/, 1/*Alignment*/));
            DrawEngine.AddPermanent(tempTile.Inhabitant.Name, tempTile.Inhabitant.Texture, tempTile.MapPosition, 0.5f);

            tempTile = GetTile(new Vector2(3, 3));
            Order.Add("Wizard");
            tempTile.AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Wizard1"), "Wizard", new AttackRangeCross(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 4 /*Range*/, 3 /*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                100 /*Skill Range*/, 3 /*Skill Damage*/, 5 /*Movement Speed*/, 10 /*Health*/, 0/*Alignment*/));
            DrawEngine.AddPermanent(tempTile.Inhabitant.Name, tempTile.Inhabitant.Texture, tempTile.MapPosition, 0.5f);

            tempTile = GetTile(new Vector2(13, 10));
            Order.Add("S2");
            tempTile.AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("S"), "S2", new AttackMelee(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 3/*Range*/, 4/*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                3 /*Skill Range*/, 2/*Skill Damage*/, 3/*Movement Speed*/, 20/*Health*/, 1/*Alignment*/));
            DrawEngine.AddPermanent(tempTile.Inhabitant.Name, tempTile.Inhabitant.Texture, tempTile.MapPosition, 0.5f);

            tempTile = GetTile(new Vector2(2, 4));
            Order.Add("Ranger");
            tempTile.AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Ranger1"), "Ranger", new AttackRangeXCross(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 5 /*Range*/, 2 /*Damage*/, new SkillRangerBomb(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Target")), 4 /*Skill Range*/, 3 /*Skill Damage*/, 6 /*Movement Speed*/, 13 /*Health*/, 0/*Alignment*/));
            DrawEngine.AddPermanent(tempTile.Inhabitant.Name, tempTile.Inhabitant.Texture, tempTile.MapPosition, 0.5f);

            tempTile = GetTile(new Vector2(10, 14));
            Order.Add("S3");
            tempTile.AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("S"), "S3", new AttackRangeXCross(content.Load<Texture2D>("Sword"),
                content.Load<Texture2D>("Cross")), 4/*Range*/, 4/*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")),
                3 /*Skill Range*/, 2/*Skill Damage*/, 5/*Movement Speed*/, 8/*Health*/, 1/*Alignment*/));
            DrawEngine.AddPermanent(tempTile.Inhabitant.Name, tempTile.Inhabitant.Texture, tempTile.MapPosition, 0.5f);

        }

        public static Vector2 TranslateMapPosition(Vector2 mapPosition) {
            return new Vector2(mapPosition.X * 40 + 100, mapPosition.Y * 40 + 100);
        }
        public static Vector2 UnTranslateMapPosition(Vector2 Position) {
            return new Vector2((Position.X - 100) / 40, (Position.Y - 100) / 40);
        }

        public static Tile GetTile(Vector2 mapPosition) {
            return Map.Find(x => x.MapPosition == mapPosition);
        }

        public static float Distance(Vector2 v1, Vector2 v2) {
            return (Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y));
        }

        /*public static Tuple<List<Tile>, bool> Path(List<Tile> acceptable, int max, Tile start, Tile stop) {
            Random r = new Random();
            List<List<Tile>> GreatPath = new List<List<Tile>>();
            for (int i = 0; i < 30; i++) {
                List<Tile> path = new List<Tile>();
                path.Add(start);
                Tile temp = GetTile(new Vector2(0, 0));
                for (int q = 0; q < 50; q++) {
                    float differenceX = path.Last().MapPosition.X - stop.MapPosition.X;
                    float differenceY = path.Last().MapPosition.Y - stop.MapPosition.Y;
                    bool added = false;

                    if (differenceX > 0) {
                        temp = GetTile(new Vector2(path.Last().MapPosition.X - 1, path.Last().MapPosition.Y));
                        if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                            path.Add(temp);
                            added = true;
                        }
                    }
                    if (differenceX < 0) {
                        temp = GetTile(new Vector2(path.Last().MapPosition.X + 1, path.Last().MapPosition.Y));
                        if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                            path.Add(temp);
                            added = true;
                        }
                    }
                    if (differenceY > 0) {
                        temp = GetTile(new Vector2(path.Last().MapPosition.X, path.Last().MapPosition.Y - 1));
                        if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                            path.Add(temp);
                            added = true;
                        }
                    }
                    if (differenceY < 0) {
                        temp = GetTile(new Vector2(path.Last().MapPosition.X, path.Last().MapPosition.Y + 1));
                        if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                            path.Add(temp);
                            added = true;
                        }
                    }

                    if (!added) {
                        switch (r.Next(1, 3)) {
                            case 1:
                                temp = GetTile(new Vector2(path.Last().MapPosition.X - 1, path.Last().MapPosition.Y));
                                if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                                    path.Add(temp);
                                }
                                break;
                            case 2:
                                temp = GetTile(new Vector2(path.Last().MapPosition.X + 1, path.Last().MapPosition.Y));
                                if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                                    path.Add(temp);
                                }
                                break;
                        }
                        switch (r.Next(1, 3)) {
                            case 1:
                                temp = GetTile(new Vector2(path.Last().MapPosition.X, path.Last().MapPosition.Y - 1));
                                if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                                    path.Add(temp);
                                }
                                break;
                            case 2:
                                temp = GetTile(new Vector2(path.Last().MapPosition.X, path.Last().MapPosition.Y + 1));
                                if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                                    path.Add(temp);
                                }
                                break;
                        }
                    }
                    if (path.Last() == stop) {
                        GreatPath.Add(path);
                        break;
                    }
                    if (path.Count > max) {
                        break;
                    }
                }
            }
            GreatPath = GreatPath.OrderBy(x => x.Count).ToList();
            if (GreatPath.Count > 0)
                return new Tuple<List<Tile>, bool>(GreatPath.Last(), true);
            else
                return new Tuple<List<Tile>, bool>(null, false);
        } */

        public static Tuple<List<Tile>, bool> Path2(List<Tile> acceptable, int distance, Tile start, Tile stop) {
            List<Flood> floodList = new List<Flood>();
            Map.ForEach(x => {
                if (acceptable.Contains(x))
                    floodList.Add(new Flood(x.MapPosition, 0));
                else
                    floodList.Add(new Flood(x.MapPosition, -1));
            });
            floodList.ForEach(x => x.FillNeighbours(floodList));

            floodList.Find(x => x.MapPosition == start.MapPosition).Steps = distance;
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
            while (true) {
                Random r = new Random();
                List<Flood> path = new List<Flood>();
                bool end = false;
                path.Add(floodList.Find(x => x.MapPosition == start.MapPosition));
                while (!end) {
                    float differenceX = path.Last().MapPosition.X - stop.MapPosition.X;
                    float differenceY = path.Last().MapPosition.Y - stop.MapPosition.Y;
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
                        if (path.Last().MapPosition == stop.MapPosition) {
                            List<Tile> tilePath = new List<Tile>();
                            path.ForEach(x => tilePath.Add(World.GetTile(x.MapPosition)));
                            return new Tuple<List<Tile>, bool>(tilePath, true);
                        }
                    }
                    else {
                        if (path.Last().MapPosition == stop.MapPosition) {
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
        }

        public static List<Tile> FloodPath(int distance, Tile start) {
            List<Flood> floodList = new List<Flood>();
            Map.ForEach(x => {
                if (x.Inhabited)
                    floodList.Add(new Flood(x.MapPosition, -1));
                else
                    floodList.Add(new Flood(x.MapPosition, 0));
            });
            floodList.Find(x => x.MapPosition == start.MapPosition).Steps = distance;
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
                    if (x.MapPosition == floodList.Where(y => y.Steps > 0).ToList()[i].MapPosition)
                        tempList.Add(x);
                });
            }
            return tempList;
        }

        public static void DoDamage(int damage, PlayerCharacter turnMaster, PlayerCharacter reciver) {
            reciver.Health -= damage;
            Console.WriteLine("The {0} does {1} damage to the {2}, their health is now {3}", turnMaster.Name, damage, reciver.Name, reciver.Health);
            Vector2 translated = TranslateMapPosition(reciver.MapPosition);
            Vector2 origin = new Vector2(translated.X + 10, translated.Y + 10);
            Vector2 goal = new Vector2(translated.X + 10, translated.Y - 20);
            DrawEngine.AddDamageReport(string.Format("-{0}", damage), Color.Red, UnTranslateMapPosition(origin), 1f, false, UnTranslateMapPosition(goal));
        }

        public static void DoHealing(int heal, PlayerCharacter turnMaster, PlayerCharacter reciver) {
            reciver.Health += heal;
            Console.WriteLine("The {0} heals {1} for {2}, their health is now {3}", turnMaster.Name, heal, reciver.Name, reciver.Health);
            Vector2 translated = TranslateMapPosition(reciver.MapPosition);
            Vector2 origin = new Vector2(translated.X + 10, translated.Y + 10);
            Vector2 goal = new Vector2(translated.X + 10, translated.Y - 20);
            DrawEngine.AddDamageReport(string.Format("+{0}", heal), Color.ForestGreen, UnTranslateMapPosition(origin), 1f, false, UnTranslateMapPosition(goal));
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
        public Vector2 MapPosition { get; set; }
        public Rectangle Collision { get { return new Rectangle((int)World.TranslateMapPosition(MapPosition).X, (int)World.TranslateMapPosition(MapPosition).Y, 40, 40); } }

        public bool Inhabited { get; set; } = false;
        public PlayerCharacter Inhabitant { get; set; } = null;


        public void MoveInhabited(Tile target) {
            target.AddInhabitor(Inhabitant);
            DrawEngine.PermanentAnimations.Find(x => x.Name == Inhabitant.Name).Position = World.TranslateMapPosition(target.MapPosition);
            Tuple<List<Tile>, bool> path = World.Path2(World.SelectedTiles, Inhabitant.MoveSpeed, this, target);
            if (path.Item2) {
                for (int i = 0; i < path.Item1.Count - 1; i++) {
                    DrawEngine.AddQueued(Inhabitant.Name, Inhabitant.Texture, path.Item1[i].MapPosition, Vector2.Zero, 0.3f, false, path.Item1[i + 1].MapPosition);
                }
            }
            RemoveInhabitor();
        }

        public Tile(Vector2 mapPosition) {
            MapPosition = mapPosition;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D tex, float layer) {
            spriteBatch.Draw(tex, Collision, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, layer);
        }
        public void AddInhabitor(PlayerCharacter inhabitor) {
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
