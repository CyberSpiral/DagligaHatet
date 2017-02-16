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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DagligaHatet {

    public class Object {
        public Texture2D Texture { get; }
        public Tile Inhabited { get; set; }
        public Vector2 MapCoordinate { get { return Inhabited.MapCoordinate; } }

        public string Name { get; }

        public Object(Texture2D tex, string name) {
            Texture = tex;
            Name = name;
        }


    }

    public class Character : Object {
        public int MoveSpeed { get; }
        public int MaxHealth { get; }
        public int Health { get; set; }
        public int Alignment { get; }
        public List<Tile> SelectedTiles { get; set; }

        public Attack Attack { get; }
        public int Damage { get; }
        public int Range { get; }

        public Skill Skill { get; }
        public int SkillRange { get; set; }
        public int SkillDamage { get; }
        public float Rotation { get; set; }

        protected List<Character> allFriendly { get; set; } = new List<Character>();
        public List<Character> allEnemies { get; set; } = new List<Character>();


        /// <summary>
        /// Creating a new character
        /// </summary>
        /// <param name="tex">Sprite texture</param>
        /// <param name="position">Position in pixels</param>
        /// <param name="mapPosition">Position in map-coordinates</param>
        /// <param name="name">Name to identify character</param>
        /// <param name="attack">Class of normal attack</param>
        /// <param name="range">Attack range</param>
        /// <param name="damage">Damage of normal attack</param>
        /// <param name="skill">Class of specific skill</param>
        /// <param name="skillRange">Range of skill</param>
        /// <param name="movementSpeed">Movement speed in tiles</param>
        /// <param name="health">Health points</param>
        public Character(Texture2D tex, string name, Attack attack, int range, int damage, Skill skill, int skillRange, int skillDamage, int movementSpeed, int health, int alignment) : base(tex, name) {
            Alignment = alignment;
            MoveSpeed = movementSpeed;
            Range = range;
            Health = health;
            MaxHealth = health;
            Damage = damage;
            Attack = attack;
            Skill = skill;
            SkillRange = skillRange;
            SkillDamage = skillDamage;
            Rotation = 0;
            SelectedTiles = new List<Tile>();
            allEnemies = World.AllCharacters.Where(x => x.Alignment != Alignment).ToList();
            allFriendly = World.AllCharacters.Where(x => x.Alignment == Alignment && x != this).ToList();
        }

        public void Draw(SpriteBatch sB) {
            if (!DrawEngine.QueuedAnimations.Exists(x => x.Name == Name)) {
                sB.Draw(Texture, World.TranslateMapMiddleMapCoordinate(MapCoordinate), null, Color.White, Rotation, new Vector2(20, 20), 1, SpriteEffects.None, 0.9f);
            }
        }

        public virtual void Update() {
            allEnemies = World.AllCharacters.Where(x => x.Alignment != Alignment).ToList();
            allFriendly = World.AllCharacters.Where(x => x.Alignment == Alignment && x != this).ToList();
        }

    }

    public class Player : Character {
        public Player(Texture2D tex, string name, Attack attack, int range, int damage, Skill skill, int skillRange, int skillDamage, int movementSpeed, int health, int alignment)
            : base(tex, name, attack, range, damage, skill, skillRange, skillDamage, movementSpeed, health, alignment) {
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttons">1. Move 2.Attack 3. Skill 4. Skip</param>
        /// 
        public override void Update() {
            base.Update();
            #region Players
            if (World.Buttons[0].Update(Mouse.GetState(), World.oldMouse)) {
                if (World.phase == states.ChoosePhase) {
                    SelectedTiles.Clear();
                    SelectedTiles.AddRange(World.FloodPath(MoveSpeed, Inhabited));
                    SelectedTiles.RemoveAll(x => x.MapCoordinate == MapCoordinate);
                    SelectedTiles.RemoveAll(x => x.Inhabited);
                    World.Buttons.Where(x => !x.Name.Contains("move")).ToList().ForEach(x => x.Hidden = true);
                    World.phase = states.MovePhase1;
                }
                else if (World.phase == states.MovePhase1) {
                    SelectedTiles.Clear();
                    World.Buttons.ForEach(x => x.Hidden = false);
                    World.phase = states.ChoosePhase;
                }
            }

            if (World.Buttons[1].Update(Mouse.GetState(), World.oldMouse)) {
                if (World.phase == states.ChoosePhase) {
                    SelectedTiles.Clear();
                    Attack.PrepareSkill(this, World.Map, SelectedTiles);
                    World.Buttons.Where(x => !x.Name.Contains("attack")).ToList().ForEach(x => x.Hidden = true);
                    World.phase = states.AttackPhase1;
                }
                else if (World.phase == states.AttackPhase1) {
                    SelectedTiles.Clear();
                    World.Buttons.ForEach(x => x.Hidden = false);
                    World.phase = states.ChoosePhase;
                }
            }

            if (World.Buttons[2].Update(Mouse.GetState(), World.oldMouse)) {
                if (World.phase == states.ChoosePhase) {
                    SelectedTiles.Clear();
                    Skill.PrepareSkill(this, World.Map, SelectedTiles);
                    World.Buttons.Where(x => !x.Name.Contains("skill")).ToList().ForEach(x => x.Hidden = true);
                    World.phase = states.SkillPhase1;
                }
                else if (World.phase == states.SkillPhase1) {
                    SelectedTiles.Clear();
                    World.Buttons.ForEach(x => x.Hidden = false);
                    World.phase = states.ChoosePhase;
                }
            }
            if (World.Buttons[3].Update(Mouse.GetState(), World.oldMouse)) {
                if (World.phase == states.ChoosePhase) {
                    SelectedTiles.Clear();
                    World.OrderNumber++;
                    World.phase = states.ChoosePhase;
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && World.oldMouse.LeftButton == ButtonState.Released) {
                MouseState mus = Mouse.GetState();
                Rectangle musRec = new Rectangle(mus.X, mus.Y, 1, 1);

                if (SelectedTiles.Exists(x => x.Collision.Intersects(musRec))) {
                    Tile clickedTile = SelectedTiles.Find(x => x.Collision.Intersects(musRec));
                    if (World.phase == states.MovePhase1) {
                        Inhabited.MoveInhabited(clickedTile);
                        World.Buttons.ForEach(x => x.Hidden = false);
                        SelectedTiles.Clear();

                        //Round over/Move over
                        World.phase = states.ChoosePhase;
                        World.OrderNumber++;
                    }
                    else if (World.phase == states.AttackPhase1 && clickedTile.Inhabited == true) {
                        Attack.InvokeSkill(this, World.Map, SelectedTiles, clickedTile);
                        //Round over/Attack over
                        World.phase = states.ChoosePhase;
                        World.Buttons.ForEach(x => x.Hidden = false);
                        World.OrderNumber++;
                    }
                    else if (World.phase == states.SkillPhase1) {
                        Skill.InvokeSkill(this, World.Map, SelectedTiles, clickedTile);
                        World.phase = states.ChoosePhase;
                        World.Buttons.ForEach(x => x.Hidden = false);
                        World.OrderNumber++;
                    }
                }
            }


            #endregion

        }
    }

    public class Evil : Character {

        private Tile target;
        private List<Character> ListCanHitMe { get; set; } = new List<Character>();
        private List<Character> ListCanHit { get; set; } = new List<Character>();
        private List<Tile> CanMoveTo { get; set; } = new List<Tile>();
        private bool Step { get; set; } = false;


        public Evil(Texture2D tex, string name, Attack attack, int range, int damage, Skill skill, int skillRange, int skillDamage, int movementSpeed, int health, int alignment)
            : base(tex, name, attack, range, damage, skill, skillRange, skillDamage, movementSpeed, health, alignment) {

        }

        public override void Update() {
            base.Update();
            #region Enmenies 

            Random r = new Random();
            #region reset

            SelectedTiles.Clear();
            ListCanHitMe = new List<Character>();
            allEnemies.ForEach(x => {
                if (x.Attack.WouldHit(x, World.Map, SelectedTiles).Item1.Contains(this)) {
                    ListCanHitMe.Add(x);
                }
            });
            ListCanHit = Attack.WouldHit(this, World.Map, SelectedTiles).Item1;
            CanMoveTo = World.FloodPath(MoveSpeed, Inhabited);
            CanMoveTo.Remove(Inhabited);
            #endregion


            //Not Stepd
            if (!Step) {
                if (Health > MaxHealth / 3) {
                    #region Health over 1/3
                    if (ListCanHitMe.Count > 2) {
                        #region 3 Can hit me
                        if (CanMoveTo.Count > 0) {
                            #region Can move
                            for (int i = 0; i < CanMoveTo.Count; i++) {
                                ListCanHitMe.Clear();
                                allEnemies.ForEach(x => {

                                    SelectedTiles.Clear();
                                    x.Attack.PrepareSkill(x, World.Map, SelectedTiles);
                                    if (SelectedTiles.Exists(y => y == CanMoveTo[i]))
                                        ListCanHitMe.Add(x);

                                    SelectedTiles.Clear();
                                });
                                if (ListCanHitMe.Count <= 2) {
                                    //Moving
                                    SelectedTiles = CanMoveTo;
                                    target = CanMoveTo[i];
                                    World.phase = states.MovePhase1;
                                    DrawEngine.AddPause(2f);
                                    Step = true;
                                    break;
                                }
                            }
                            if (!Step) {
                                if (ListCanHit.Count > 0) {
                                    Attack.PrepareSkill(this, World.Map, SelectedTiles);
                                    target = ListCanHit.OrderBy(x => x.Health).First().Inhabited;
                                    World.phase = states.AttackPhase1;
                                    DrawEngine.AddPause(2f);
                                    Step = true;
                                }
                                else {
                                    //Moving
                                    target = CanMoveTo[r.Next(CanMoveTo.Count)];
                                    SelectedTiles = CanMoveTo;
                                    World.phase = states.MovePhase1;
                                    DrawEngine.AddPause(2f);
                                    Step = true;
                                }
                            }
                            #endregion
                        }
                        else {
                            #region Can't move
                            if (ListCanHit.Count > 0) {
                                Attack.PrepareSkill(this, World.Map, SelectedTiles);
                                target = ListCanHit.OrderBy(x => x.Health).ToList().First().Inhabited;
                                World.phase = states.AttackPhase1;
                                DrawEngine.AddPause(2f);
                                Step = true;
                            }
                            else {
                                //Skill
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else {
                        #region Less than 3 can hit me
                        if (ListCanHit.Count > 0) {
                            #region Can hit someone
                            Attack.PrepareSkill(this, World.Map, SelectedTiles);
                            target = ListCanHit.OrderBy(x => x.Health).ToList().First().Inhabited;
                            World.phase = states.AttackPhase1;
                            DrawEngine.AddPause(2f);
                            Step = true;
                            #endregion
                        }
                        else if (CanMoveTo.Count > 0) {

                            Tuple<List<Tile>, bool> pathing = World.Path2(World.Map.Where(x => !x.Inhabited || (allEnemies.OrderByDescending(h => World.Distance(h.MapCoordinate, MapCoordinate)).Last().Inhabited == x)).ToList(), 100, Inhabited, allEnemies.OrderByDescending(x => World.Distance(x.MapCoordinate, MapCoordinate)).Last().Inhabited);
                            Console.WriteLine(pathing.Item2);
                            pathing.Item1.RemoveAt(0);
                            pathing.Item1.Remove(pathing.Item1.Last());
                            int farthest = 0;
                            for (int i = 0; i < pathing.Item1.Count; i++) {
                                if (CanMoveTo.Exists(x => x.MapCoordinate == pathing.Item1[i].MapCoordinate)) {
                                    farthest = i;
                                }
                                else
                                    break;
                            }
                            target = pathing.Item1[farthest];
                            //Moving
                            World.phase = states.MovePhase1;
                            DrawEngine.AddPause(2f);
                            Step = true;


                        }
                        #endregion
                    }
                    #endregion
                }
                else {
                    #region Health under 1/3
                    if (ListCanHitMe.Count > 2) {
                        for (int i = 0; i < CanMoveTo.Count; i++) {
                            ListCanHitMe.Clear();
                            allEnemies.ForEach(x => {

                                SelectedTiles.Clear();
                                x.Attack.PrepareSkill(x, World.Map, SelectedTiles);
                                if (SelectedTiles.Exists(y => y == CanMoveTo[i]))
                                    ListCanHitMe.Add(x);

                                SelectedTiles.Clear();
                            });
                            if (ListCanHitMe.Count <= 1) {
                                //Moving
                                SelectedTiles = CanMoveTo;
                                target = CanMoveTo[i];
                                World.phase = states.MovePhase1;
                                DrawEngine.AddPause(2f);
                                Step = true;
                                break;
                            }
                        }
                    }
                    else if (ListCanHit.Count > 0) {
                        Attack.PrepareSkill(this, World.Map, SelectedTiles);
                        target = ListCanHit.OrderBy(x => x.Health).First().Inhabited;
                        World.phase = states.AttackPhase1;
                        DrawEngine.AddPause(2f);
                        Step = true;
                    }
                    else if (CanMoveTo.Count > 0 && allFriendly.Count > 0) {

                        Tuple<List<Tile>, bool> pathing = World.Path2(World.Map.Where(x => !x.Inhabited).ToList(), 100, Inhabited, 
                            allFriendly.OrderByDescending(x => World.Distance(x.MapCoordinate, MapCoordinate)).Last().Inhabited);
                        Console.WriteLine(pathing.Item2);
                        pathing.Item1.RemoveAt(0);
                        pathing.Item1.Remove(pathing.Item1.Last());
                        if (pathing.Item1.Count > 0) {
                            int farthest = 0;
                            for (int i = 0; i < pathing.Item1.Count; i++) {
                                if (CanMoveTo.Exists(x => x.MapCoordinate == pathing.Item1[i].MapCoordinate)) {
                                    farthest = i;
                                }
                                else
                                    break;
                            }
                            target = pathing.Item1[farthest];
                            //Moving
                            World.phase = states.MovePhase1;
                            DrawEngine.AddPause(2f);
                            Step = true;
                        }

                    }
                    #endregion
                }
            }
            //Stepd
            else if (Step) {
                switch (World.phase) {
                    case states.MovePhase1:
                        SelectedTiles = CanMoveTo;
                        Inhabited.MoveInhabited(target);
                        World.phase = states.ChoosePhase;
                        Step = false;
                        break;
                    case states.SkillPhase1:
                        break;
                    case states.AttackPhase1:
                        Attack.InvokeSkill(this, World.Map, SelectedTiles, target);
                        World.phase = states.ChoosePhase;
                        Step = false;
                        break;
                    default:
                        break;
                }
            }
            if (!Step) {
                SelectedTiles.Clear();
                World.OrderNumber++;
            }

            #endregion
            
        }
    }

}
