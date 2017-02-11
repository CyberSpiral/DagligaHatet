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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    //public delegate void ClickedEventHandler(object sender, EventArgs e);


    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Button moveButton;
        Button attackButton;
        Button skillButton;
        Button skipButton;
        SpriteFont GothicFont;
        bool pause;
        public static Texture2D Cross;

        //Enemies
        Tile target;
        List<PlayerCharacter> listCanHitMe = new List<PlayerCharacter>();
        List<Tile> canMoveTo = new List<Tile>();
        List<PlayerCharacter> listCanHit = new List<PlayerCharacter>();

        Texture2D move;
        states phase = states.ChoosePhase;
        MouseState oldMouse;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1000;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            moveButton = new Button(new Rectangle(30, 30, 100, 60), Content.Load<Texture2D>("Move"));
            attackButton = new Button(new Rectangle(200, 30, 100, 60), Content.Load<Texture2D>("Attack"));
            skillButton = new Button(new Rectangle(400, 30, 100, 60), Content.Load<Texture2D>("Skill"));
            skipButton = new Button(new Rectangle(600, 30, 100, 60), Content.Load<Texture2D>("Skip"));
            pause = false;
            Cross = Content.Load<Texture2D>("Cross");

            DrawEngine.AddPermanent("moveButton", moveButton.Texture, World.UnTranslateMapPosition(new Vector2(moveButton.Hitbox.X, moveButton.Hitbox.Y)), 1);
            DrawEngine.AddPermanent("attackButton", attackButton.Texture, World.UnTranslateMapPosition(new Vector2(attackButton.Hitbox.X, attackButton.Hitbox.Y)), 1);
            DrawEngine.AddPermanent("skillButton", skillButton.Texture, World.UnTranslateMapPosition(new Vector2(skillButton.Hitbox.X, skillButton.Hitbox.Y)), 1);
            DrawEngine.AddPermanent("skipButton", skipButton.Texture, World.UnTranslateMapPosition(new Vector2(skipButton.Hitbox.X, skipButton.Hitbox.Y)), 1);
            GothicFont = Content.Load<SpriteFont>("Gothic");
            move = Content.Load<Texture2D>("MoveAni");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Clicked += MouseClick;


            World.Prepare(Content);
            // TODO: use this.Content to load your game content here

        }


        /*Event test
        public event ClickedEventHandler Clicked;
        protected virtual void OnClicked(EventArgs e) {
            if (Clicked != null)
                Clicked(this, e);
        }
        
        private void MouseClick(object sender, EventArgs e) {

            int indexNumber = playerCharacters.FindIndex(x => x.Name == order[orderNumber]);
            MouseState mus = Mouse.GetState();
            Rectangle musRec = new Rectangle(mus.X, mus.Y, 1, 1);

            if (World.selectedTiles.Exists(x => x.Collision.Intersects(musRec))) {
                int tileNumber = World.selectedTiles.FindIndex(x => x.Collision.Intersects(musRec));
                if (phase == states.MovePhase1) {
                    List<List<Vector2>> Path = new List<List<Vector2>>();

                    map.Find(x => x.MapPosition == playerCharacters[indexNumber].MapPosition).Occupied = false;
                    int mapNumber = map.FindIndex(x => x.Collision.Intersects(musRec));
                    playerCharacters[indexNumber].Position = map[mapNumber].Position;
                    playerCharacters[indexNumber].MapPosition = map[mapNumber].MapPosition;
                    map[mapNumber].Occupied = true;
                    World.selectedTiles.Clear();

                    //Round over/Move over
                    phase = states.ChoosePhase;
                    orderNumber++;
                }
                else if (phase == states.AttackPhase1 && World.selectedTiles[tileNumber].Occupied == true) {
                    //Damage
                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == World.selectedTiles[tileNumber].MapPosition).Health);
                    playerCharacters.Where(x => x.MapPosition == World.selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].Damage);

                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == World.selectedTiles[tileNumber].MapPosition).Health);
                    World.selectedTiles.Clear();


                    //Round over/Attack over
                    phase = states.ChoosePhase;
                    orderNumber++;
                }
            }
            if (orderNumber >= order.Count) {
                orderNumber = 0;
            }
        }
        */

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here 
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Event test
            /*if (oldMouse.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed) {
                OnClicked(EventArgs.Empty);
            }*/

            if (DrawEngine.QueuedAnimations.Count == 0 && DrawEngine.QueuedText.Count == 0) {

                PlayerCharacter turnMaster = World.Map.Find(x => x.Inhabited && x.Inhabitant.Name == World.Order[World.OrderNumber]).Inhabitant;
                List<PlayerCharacter> allEnemies = new List<PlayerCharacter>();
                World.Map.Where(x => x.Inhabited && x.Inhabitant.Alignment != turnMaster.Alignment).ToList().ForEach(x => allEnemies.Add(x.Inhabitant));

                #region Players
                if (turnMaster.Alignment == 0) {
                    if (moveButton.Update(Mouse.GetState(), oldMouse)) {
                        if (phase == states.ChoosePhase) {
                            World.SelectedTiles.Clear();
                            World.SelectedTiles.AddRange(World.FloodPath(turnMaster.MoveSpeed, turnMaster.Inhabited));
                            World.SelectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
                            World.SelectedTiles.RemoveAll(x => x.Inhabited);
                            World.SelectedTiles.ForEach(x => {
                                DrawEngine.AddPermanent("Move", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0);
                            });
                            DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button") && !x.Name.Contains("move")).ToList().ForEach(x => x.Hidden = true);
                            phase = states.MovePhase1;
                        }
                        else if (phase == states.MovePhase1) {
                            World.SelectedTiles.Clear();
                            DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button")).ToList().ForEach(x => x.Hidden = false);
                            DrawEngine.ClearPermanent("Move");
                            phase = states.ChoosePhase;
                        }
                    }

                    if (attackButton.Update(Mouse.GetState(), oldMouse)) {
                        if (phase == states.ChoosePhase) {
                            World.SelectedTiles.Clear();
                            turnMaster.Attack.PrepareSkill(turnMaster, World.Map, World.SelectedTiles);
                            DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button") && !x.Name.Contains("attack")).ToList().ForEach(x => x.Hidden = true);
                            phase = states.AttackPhase1;
                        }
                        else if (phase == states.AttackPhase1) {
                            World.SelectedTiles.Clear();
                            DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button")).ToList().ForEach(x => x.Hidden = false);
                            DrawEngine.ClearPermanent("selectedTiles");
                            phase = states.ChoosePhase;
                        }
                    }

                    if (skillButton.Update(Mouse.GetState(), oldMouse)) {
                        if (phase == states.ChoosePhase) {
                            DrawEngine.ClearPermanent("selectedTiles");
                            World.SelectedTiles.Clear();
                            turnMaster.Skill.PrepareSkill(turnMaster, World.Map, World.SelectedTiles);
                            DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button") && !x.Name.Contains("skill")).ToList().ForEach(x => x.Hidden = true);
                            phase = states.SkillPhase1;
                        }
                        else if (phase == states.SkillPhase1) {
                            World.SelectedTiles.Clear();
                            DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button")).ToList().ForEach(x => x.Hidden = false);
                            DrawEngine.ClearPermanent("selectedTiles");
                            phase = states.ChoosePhase;
                        }
                    }
                    if (skipButton.Update(Mouse.GetState(), oldMouse)) {
                        if (phase == states.ChoosePhase) {
                            World.SelectedTiles.Clear();
                            World.OrderNumber++;
                            phase = states.ChoosePhase;
                        }
                    }

                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released) {
                        MouseState mus = Mouse.GetState();
                        Rectangle musRec = new Rectangle(mus.X, mus.Y, 1, 1);

                        if (World.SelectedTiles.Exists(x => x.Collision.Intersects(musRec))) {
                            Tile clickedTile = World.SelectedTiles.Find(x => x.Collision.Intersects(musRec));
                            if (phase == states.MovePhase1) {
                                turnMaster.Inhabited.MoveInhabited(clickedTile);
                                DrawEngine.ClearPermanent("Move");
                                DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button")).ToList().ForEach(x => x.Hidden = false);
                                World.SelectedTiles.Clear();

                                //Round over/Move over
                                phase = states.ChoosePhase;
                                World.OrderNumber++;
                            }
                            else if (phase == states.AttackPhase1 && clickedTile.Inhabited == true) {
                                turnMaster.Attack.InvokeSkill(turnMaster, World.Map, World.SelectedTiles, clickedTile);
                                //Round over/Attack over
                                phase = states.ChoosePhase;
                                DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button")).ToList().ForEach(x => x.Hidden = false);
                                World.OrderNumber++;
                            }
                            else if (phase == states.SkillPhase1) {
                                turnMaster.Skill.InvokeSkill(turnMaster, World.Map, World.SelectedTiles, clickedTile);
                                phase = states.ChoosePhase;
                                DrawEngine.PermanentAnimations.Where(x => x.Name.Contains("Button")).ToList().ForEach(x => x.Hidden = false);
                                World.OrderNumber++;
                            }
                        }
                    }

                }
                #endregion

                #region Emenies
                /*else {
                    while (true) {
                        DrawEngine.ClearPermanent("selectedTiles");
                        World.SelectedTiles.Clear();
                        if (turnMaster.Health > (turnMaster.MaxHealth / 3)) {
                            turnMaster.Attack.PrepareSkill(turnMaster, World.Map, World.SelectedTiles);
                            Tuple<PlayerCharacter, bool> toHit = turnMaster.Attack.WouldHit(turnMaster, World.Map, World.SelectedTiles);
                            if (toHit.Item2) {
                                if (!pause) {
                                    DrawEngine.AddPause(1f);
                                    pause = true;
                                    break;
                                }
                                else if (pause) {
                                    turnMaster.Attack.InvokeSkill(turnMaster, World.Map, World.SelectedTiles, toHit.Item1.Inhabited);
                                    DrawEngine.ClearPermanent("selectedTiles");
                                    World.SelectedTiles.Clear();
                                    pause = false;
                                    break;
                                }
                            }
                            else {
                                if (!pause) {
                                    DrawEngine.ClearPermanent("selectedTiles");
                                    World.SelectedTiles.Clear();
                                    World.SelectedTiles.AddRange(World.FloodPath(turnMaster.MoveSpeed, turnMaster.Inhabited));
                                    World.SelectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
                                    World.SelectedTiles.RemoveAll(x => x.Inhabited);
                                    World.SelectedTiles.ForEach(x => {
                                        DrawEngine.AddPermanent("selectedTiles", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0);
                                    });
                                    DrawEngine.AddPause(1f);
                                    pause = true;
                                    break;
                                }
                                else if (pause) {
                                    DrawEngine.ClearPermanent("selectedTiles");
                                    World.SelectedTiles.Clear();
                                    allEnemies = allEnemies.OrderBy(x => World.Distance(x.MapPosition, turnMaster.MapPosition)).ToList();
                                    Tuple<List<Tile>, bool> pathing = World.Path(World.Map.Where(x => !x.Inhabited || (allEnemies.Last().Inhabited == x)).ToList(), 100, turnMaster.Inhabited, allEnemies.Last().Inhabited);
                                    Console.WriteLine(pathing.Item2);
                                    World.SelectedTiles.AddRange(World.FloodPath(turnMaster.MoveSpeed, turnMaster.Inhabited));
                                    World.SelectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
                                    World.SelectedTiles.RemoveAll(x => x.Inhabited);
                                    int farthest = 0;
                                    for (int i = 0; i < pathing.Item1.Count; i++) {
                                        if (World.SelectedTiles.Exists(x => x.MapPosition == pathing.Item1[i].MapPosition)) {
                                            farthest = i;
                                        }
                                    }
                                    turnMaster.Inhabited.MoveInhabited(pathing.Item1[farthest]);
                                    DrawEngine.ClearPermanent("selectedTiles");
                                    World.SelectedTiles.Clear();
                                }
                                pause = false;
                                break;
                            }
                        }
                        else if (turnMaster.Health < (turnMaster.MaxHealth / 3)) {
                            if (!pause) {
                                World.SelectedTiles.Clear();
                                DrawEngine.ClearPermanent("selectedTiles");
                                World.SelectedTiles.AddRange(World.FloodPath(turnMaster.MoveSpeed, turnMaster.Inhabited));
                                World.SelectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
                                World.SelectedTiles.RemoveAll(x => x.Inhabited);
                                World.SelectedTiles.ForEach(x => {
                                    DrawEngine.AddPermanent("Move", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0);
                                });
                                DrawEngine.AddPause(1f);
                                pause = true;
                                break;
                            }
                            else if (pause) {
                                List<int> distances = new List<int>();
                                bool scared = false;
                                allEnemies.ForEach(x => {
                                    x.Attack.PrepareSkill(x, World.Map, World.SelectedTiles);
                                    if (x.Attack.WouldHit(x, World.Map, World.SelectedTiles).Item2 == true) {
                                        scared = true;
                                    }
                                    DrawEngine.ClearPermanent("selectedTiles");
                                    World.SelectedTiles.Clear();
                                });
                                if (scared) {
                                    Tile testTile = null;
                                    Random r = new Random();
                                    int c = 0;
                                    while (scared) {
                                        c++;
                                        if (c >= 20)
                                            throw new NotImplementedException();
                                            //Scared
                                        World.SelectedTiles.AddRange(World.FloodPath(turnMaster.MoveSpeed, turnMaster.Inhabited));
                                        World.SelectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
                                        World.SelectedTiles.RemoveAll(x => x.Inhabited);
                                        scared = false;
                                        Tile oldTile = turnMaster.Inhabited;
                                        testTile = World.SelectedTiles[r.Next(World.SelectedTiles.Count)];

                                        allEnemies.ForEach(x => {
                                            DrawEngine.ClearPermanent("selectedTiles");
                                            World.SelectedTiles.Clear();
                                            x.Attack.PrepareSkill(x, World.Map, World.SelectedTiles);
                                            if (World.SelectedTiles.Exists(y => y == testTile))
                                                scared = true;
                                            DrawEngine.ClearPermanent("selectedTiles");
                                            World.SelectedTiles.Clear();
                                        });
                                    }
                                    World.SelectedTiles.AddRange(World.FloodPath(turnMaster.MoveSpeed, turnMaster.Inhabited));
                                    turnMaster.Inhabited.MoveInhabited(testTile);
                                    World.SelectedTiles.Clear();
                                    DrawEngine.ClearPermanent("selectedTiles");
                                    pause = false;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    if (!pause) {
                        World.OrderNumber++;
                    }
                }*/

                #endregion

                #region New 

                else {
                    Random r = new Random();
                    #region reset
                    DrawEngine.ClearPermanent("selectedTiles");
                    World.SelectedTiles.Clear();
                    listCanHitMe = new List<PlayerCharacter>();
                    allEnemies.ForEach(x => {
                        if (x.Attack.WouldHit(x, World.Map, World.SelectedTiles).Item1.Contains(turnMaster)) {
                            listCanHitMe.Add(x);
                        }
                    });
                    listCanHit = turnMaster.Attack.WouldHit(turnMaster, World.Map, World.SelectedTiles).Item1;
                    canMoveTo = World.FloodPath(turnMaster.MoveSpeed, turnMaster.Inhabited);
                    canMoveTo.Remove(turnMaster.Inhabited);
                    #endregion


                    //Not Paused
                    if (!pause) {
                        if (turnMaster.Health > turnMaster.MaxHealth / 3) {
                            #region Health over 1/3
                            if (listCanHitMe.Count > 2) {
                                #region 3 Can hit me
                                if (canMoveTo.Count > 0) {
                                    #region Can move
                                    for (int i = 0; i < canMoveTo.Count; i++) {
                                        listCanHitMe.Clear();
                                        allEnemies.ForEach(x => {
                                            DrawEngine.ClearPermanent("selectedTiles");
                                            World.SelectedTiles.Clear();
                                            x.Attack.PrepareSkill(x, World.Map, World.SelectedTiles);
                                            if (World.SelectedTiles.Exists(y => y == canMoveTo[i]))
                                                listCanHitMe.Add(x);
                                            DrawEngine.ClearPermanent("selectedTiles");
                                            World.SelectedTiles.Clear();
                                        });
                                        if (listCanHitMe.Count <= 2) {
                                            canMoveTo.ForEach(x => DrawEngine.AddPermanent("selectedTiles", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0));
                                            World.SelectedTiles = canMoveTo;
                                            target = canMoveTo[i];
                                            phase = states.MovePhase1;
                                            DrawEngine.AddPause(2f);
                                            pause = true;
                                            break;
                                        }
                                    }
                                    if (!pause) {
                                        if (listCanHit.Count > 0) {
                                            turnMaster.Attack.PrepareSkill(turnMaster, World.Map, World.SelectedTiles);
                                            target = listCanHit.OrderBy(x => x.Health).Last().Inhabited;
                                            phase = states.AttackPhase1;
                                            DrawEngine.AddPause(2f);
                                            pause = true;
                                        }
                                        else {
                                            canMoveTo.ForEach(x => DrawEngine.AddPermanent("selectedTiles", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0));
                                            target = canMoveTo[r.Next(canMoveTo.Count)];
                                            World.SelectedTiles = canMoveTo;
                                            phase = states.MovePhase1;
                                            DrawEngine.AddPause(2f);
                                            pause = true;
                                        }
                                    }
                                    #endregion
                                }
                                else {
                                    #region Can't move
                                    if (listCanHit.Count > 0) {
                                        turnMaster.Attack.PrepareSkill(turnMaster, World.Map, World.SelectedTiles);
                                        target = listCanHit.OrderBy(x => x.Health).ToList().Last().Inhabited;
                                        phase = states.AttackPhase1;
                                        DrawEngine.AddPause(2f);
                                        pause = true;
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
                                if (listCanHit.Count > 0) {
                                    #region Can hit someone
                                    turnMaster.Attack.PrepareSkill(turnMaster, World.Map, World.SelectedTiles);
                                    target = listCanHit.OrderBy(x => x.Health).ToList().Last().Inhabited;
                                    phase = states.AttackPhase1;
                                    DrawEngine.AddPause(2f);
                                    pause = true;
                                    #endregion
                                }
                                else if (canMoveTo.Count > 0) {
                                    
                                    Tuple <List<Tile>, bool> pathing = World.Path2(World.Map.Where(x => !x.Inhabited || (allEnemies.OrderByDescending(h => World.Distance(h.MapPosition,  turnMaster.MapPosition)).Last().Inhabited == x)).ToList(), 100, turnMaster.Inhabited, allEnemies.OrderByDescending(x => World.Distance(x.MapPosition, turnMaster.MapPosition)).Last().Inhabited);
                                    Console.WriteLine(pathing.Item2);
                                    pathing.Item1.RemoveAt(0);
                                    pathing.Item1.Remove(pathing.Item1.Last());
                                    int farthest = 0;
                                    for (int i = 0; i < pathing.Item1.Count; i++) {
                                        if (canMoveTo.Exists(x => x.MapPosition == pathing.Item1[i].MapPosition)) {
                                            farthest = i;
                                        }
                                        else
                                            break;
                                    }
                                    target = pathing.Item1[farthest];
                                    canMoveTo.ForEach(x => DrawEngine.AddPermanent("selectedTiles", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0));
                                    phase = states.MovePhase1;
                                    DrawEngine.AddPause(2f);
                                    pause = true;


                                }
                                #endregion
                            }
                            #endregion
                        }
                        else {
                            #region Health under 1/3
                            if (listCanHitMe.Count > 1) {
                                for (int i = 0; i < canMoveTo.Count; i++) {
                                    listCanHitMe.Clear();
                                    allEnemies.ForEach(x => {
                                        DrawEngine.ClearPermanent("selectedTiles");
                                        World.SelectedTiles.Clear();
                                        x.Attack.PrepareSkill(x, World.Map, World.SelectedTiles);
                                        if (World.SelectedTiles.Exists(y => y == canMoveTo[i]))
                                            listCanHitMe.Add(x);
                                        DrawEngine.ClearPermanent("selectedTiles");
                                        World.SelectedTiles.Clear();
                                    });
                                    if (listCanHitMe.Count <= 1) {
                                        canMoveTo.ForEach(x => DrawEngine.AddPermanent("selectedTiles", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0));
                                        World.SelectedTiles = canMoveTo;
                                        target = canMoveTo[i];
                                        phase = states.MovePhase1;
                                        DrawEngine.AddPause(2f);
                                        pause = true;
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    //Paused
                    else if (pause) {
                        switch (phase) {
                            case states.MovePhase1:
                                World.SelectedTiles = canMoveTo;
                                turnMaster.Inhabited.MoveInhabited(target);
                                phase = states.ChoosePhase;
                                pause = false;
                                break;
                            case states.SkillPhase1:
                                break;
                            case states.AttackPhase1:
                                turnMaster.Attack.InvokeSkill(turnMaster, World.Map, World.SelectedTiles, target);
                                phase = states.ChoosePhase;
                                pause = false;
                                break;
                            default:
                                break;
                        }
                    }

                    #region old
                    /*else if (pause) {
                        DrawEngine.ClearPermanent("selectedTiles");
                        World.SelectedTiles.Clear();
                        if (turnMaster.Health > turnMaster.MaxHealth / 3) {
                            #region Health over 1/3
                            if (listCanHitMe.Count > 2) {
                                #region 3 Can hit me
                                if (canMoveTo.Count > 0) {
                                    #region Can move
                                    for (int i = 0; i < canMoveTo.Count; i++) {
                                        listCanHitMe.Clear();
                                        allEnemies.ForEach(x => {
                                            DrawEngine.ClearPermanent("selectedTiles");
                                            World.SelectedTiles.Clear();
                                            x.Attack.PrepareSkill(x, World.Map, World.SelectedTiles);
                                            if (World.SelectedTiles.Exists(y => y == canMoveTo[i]))
                                                listCanHitMe.Add(x);
                                            DrawEngine.ClearPermanent("selectedTiles");
                                            World.SelectedTiles.Clear();
                                        });
                                        if (listCanHitMe.Count <= 2) {
                                            turnMaster.Inhabited.MoveInhabited(target);
                                            pause = false;
                                            break;
                                        }
                                    }
                                    if (pause) {
                                        if (listCanHit.Count > 0) {
                                            turnMaster.Attack.InvokeSkill(turnMaster, World.Map, World.SelectedTiles, target);
                                            pause = false;
                                        }
                                        else {
                                            World.SelectedTiles = canMoveTo;
                                            turnMaster.Inhabited.MoveInhabited(canMoveTo[r.Next(canMoveTo.Count)]);
                                            pause = false;
                                        }
                                    }
                                    #endregion
                                }
                                else {
                                    #region Can't move
                                    if (listCanHit.Count > 0) {
                                        turnMaster.Attack.PrepareSkill(turnMaster, World.Map, World.SelectedTiles);
                                        turnMaster.Attack.InvokeSkill(turnMaster, World.Map, World.SelectedTiles, listCanHit.OrderBy(x => x.Health).Last().Inhabited);
                                        pause = false;
                                    }
                                    else {
                                        //Skill
                                    }
                                    #endregion
                                }

                                //Move
                                #endregion
                            }
                            else {
                                #region Less than 3 can hit me

                                #endregion
                            }
                            #endregion
                        }
                        else {
                            #region Health under 1/3
                            if (listCanHit.Count == 0) {
                                canMoveTo.ForEach(x => DrawEngine.AddPermanent("selectedTiles", move, x.MapPosition, Vector2.Zero, 0.2f, 2, 0));
                                DrawEngine.AddPause(2f);
                                pause = true;
                            }
                            #endregion
                        }
                    }*/
                    #endregion

                    if (!pause) {
                        World.SelectedTiles.Clear();
                        DrawEngine.ClearPermanent("selectedTiles");
                        World.OrderNumber++;
                    }
                }
                #endregion

                if (World.OrderNumber >= World.Order.Count) {
                    World.OrderNumber = 0;
                }
            }



            DrawEngine.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            // TODO: Add your update logic here
            oldMouse = Mouse.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.    
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.get_transformation(GraphicsDevice));

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            /*for (int i = 0; i < 3; i++) {
                spriteBatch.Draw(Content.Load<Texture2D>("DSC_0089"), new Vector2(378 * i + 0, 0), Color.White);
            }*/

            World.Map.ForEach(x => x.Draw(spriteBatch, Content.Load<Texture2D>("Bushes and dirt_0"), 1));
            World.Map.ForEach(x => x.Draw(spriteBatch, Content.Load<Texture2D>("Bushes and dirt_6"), 0.99f));

            DrawEngine.Draw(spriteBatch, GothicFont);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
