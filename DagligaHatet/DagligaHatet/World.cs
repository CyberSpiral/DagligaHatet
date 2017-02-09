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
            Order.Add("Knight");
            GetTile(new Vector2(1, 4)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Knight1"), "Knight", new AttackMelee(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 3 /*Range*/, 3 /*Damage*/, new SkillKnightWhirlwind(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("WhirlwindAni")), 2 /*Skill Range*/, 2 /*Skill Damage*/, 7 /*Movement Speedu*/, 10 /*Health*/, 0/*Alignment*/));
            DrawEngine.AddPermanent(GetTile(new Vector2(1, 4)).Inhabitant.Name, GetTile(new Vector2(1, 4)).Inhabitant.Texture, new Vector2(1, 4));

            Order.Add("S1");
            GetTile(new Vector2(17, 15)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("S"), "S1", new AttackRangeCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 5/*Range*/, 3/*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")), 3 /*Skill Range*/, 2/*Skill Damage*/, 4/*Movement Speed*/, 14/*Health*/, 1/*Alignment*/));
            DrawEngine.AddPermanent(GetTile(new Vector2(17, 15)).Inhabitant.Name, GetTile(new Vector2(17, 15)).Inhabitant.Texture, new Vector2(17, 15));

            Order.Add("Wizard");
            GetTile(new Vector2(5, 4)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Wizard1"), "Wizard", new AttackRangeCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 4 /*Range*/, 3 /*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")), 100 /*Skill Range*/, 3 /*Skill Damage*/, 4 /*Movement Speed*/, 10 /*Health*/, 0/*Alignment*/));
            DrawEngine.AddPermanent(GetTile(new Vector2(5, 4)).Inhabitant.Name, GetTile(new Vector2(5, 4)).Inhabitant.Texture, new Vector2(5, 4));

            Order.Add("Ranger");
            GetTile(new Vector2(10, 3)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Ranger1"), "Ranger", new AttackRangeXCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 5 /*Range*/, 2 /*Damage*/, new SkillRangerBomb(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Target")), 4 /*Skill Range*/, 3 /*Skill Damage*/, 5 /*Movement Speed*/, 13 /*Health*/, 0/*Alignment*/));
            DrawEngine.AddPermanent(GetTile(new Vector2(10, 3)).Inhabitant.Name, GetTile(new Vector2(10, 3)).Inhabitant.Texture, new Vector2(10, 3));
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

        public static Tuple<List<Tile>, bool> Path(List<Tile> acceptable, int max, Tile start, Tile stop) {
            Random r = new Random();
            List<List<Tile>> GreatPath = new List<List<Tile>>();
            for (int i = 0; i < 100; i++) {
                List<Tile> path = new List<Tile>();
                path.Add(start);
                Tile temp = GetTile(new Vector2(0, 0));
                while (true) {
                    float differenceX = path.Last().MapPosition.X - stop.MapPosition.X;
                    float differenceY = path.Last().MapPosition.Y - stop.MapPosition.Y;
                    bool added = false;
                    if (i % 2 == 0) {
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
                    }
                    else {
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
                    }
                    if (!added) {
                        switch (r.Next(1, 5)) {
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
                            case 3:
                                temp = GetTile(new Vector2(path.Last().MapPosition.X, path.Last().MapPosition.Y - 1));
                                if (temp != null && acceptable.Exists(x => x.MapPosition == temp.MapPosition)) {
                                    path.Add(temp);
                                }
                                break;
                            case 4:
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

    }
    public class Flood {
        public Vector2 MapPosition { get; }
        public int Steps { get; set; }
        public Flood(Vector2 mapPosition, int steps) {
            Steps = steps;
            MapPosition = mapPosition;
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
            Tuple<List<Tile>, bool> path = World.Path(World.SelectedTiles, Inhabitant.MoveSpeed, this, target);
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

        public void Draw(SpriteBatch spriteBatch, Texture2D tex) {
            spriteBatch.Draw(tex, Collision, Color.White);
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
