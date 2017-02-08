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

            Order.Add("S1");
            GetTile(new Vector2(17, 15)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("S"), "S1", new AttackRangeCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 5, 3, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")), 3, 2, 4, 14));
            
            Order.Add("Knight");
            GetTile(new Vector2(1,4)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Knight1"), "Knight", new AttackMelee(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 3 /*Range*/, 3 /*Damage*/, new SkillKnightWhirlwind(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("WhirlwindAni")), 2 /*Skill Range*/, 2 /*Skill Damage*/, 3 /*Movement Speedu*/, 10 /*Health*/));
            
            Order.Add("Wizard");
            GetTile(new Vector2(5,4)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Wizard1"), "Wizard", new AttackRangeCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 4 /*Range*/, 3 /*Damage*/, new SkillWizardHeal(content.Load<Texture2D>("HealAnimation")), 100 /*Skill Range*/, 3 /*Skill Damage*/, 4 /*Movement Speed*/, 10 /*Health*/));
            
            Order.Add("Ranger");
            GetTile(new Vector2(10,3)).AddInhabitor(new PlayerCharacter(content.Load<Texture2D>("Ranger1"), "Ranger", new AttackRangeXCross(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Cross")), 5 /*Range*/, 2 /*Damage*/, new SkillRangerBomb(content.Load<Texture2D>("Sword"), content.Load<Texture2D>("Target")), 4 /*Skill Range*/, 3 /*Skill Damage*/, 5 /*Movement Speed*/, 13 /*Health*/));
            
        }

        public static void Update(GameTime gameTime, MouseState oldMouse) {
            
        }

        public static Vector2 TranslateMapPosition(Vector2 mapPosition) {
            return new Vector2(mapPosition.X * 40 + 100, mapPosition.Y * 40 + 100);
        }
        public static Vector2 UnTranslateMapPosition(Vector2 Position) {
            return new Vector2((Position.X - 100)/40, (Position.Y - 100)/40);
        }
        public static Tile GetTile (Vector2 mapPosition){
            return Map.Find(x => x.MapPosition == mapPosition);
        }
        public static float Distance(Vector2 v1, Vector2 v2) {
            return (Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y));
        }
    }
    public class Tile {
        public Vector2 MapPosition { get; set; }
        public Rectangle Collision { get { return new Rectangle((int)World.TranslateMapPosition(MapPosition).X, (int)World.TranslateMapPosition(MapPosition).Y, 40, 40); } }

        public bool Inhabited { get; set; } = false;
        public PlayerCharacter Inhabitant { get; set; } = null;


        public void MoveInhabited(Tile target) {
            target.AddInhabitor(Inhabitant);
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
