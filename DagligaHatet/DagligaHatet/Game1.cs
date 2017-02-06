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
        List<Tile> map = new List<Tile>();
        Button moveButton;
        Button attackButton;
        Button skillButton;
        List<Tile> selectedTiles = new List<Tile>();

        List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();
        List<string> order = new List<string>();
        int orderNumber = 0;

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
            moveButton = new DagligaHatet.Button(new Rectangle(30, 30, 100, 60), Content.Load<Texture2D>("Move"));
            attackButton = new Button(new Rectangle(200, 30, 100, 60), Content.Load<Texture2D>("Attack"));
            skillButton = new Button(new Rectangle(400, 30, 100, 60), Content.Load<Texture2D>("Skill"));
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Clicked += MouseClick;


            for (int x = 0; x < 20; x++) {
                for (int y = 0; y < 16; y++) {
                    map.Add(new Tile(new Vector2(x * 40 + 100, y * 40 + 100), new Vector2(x, y)));
                }
            }
            


            order.Add("Knight");
            playerCharacters.Add(new PlayerCharacter(Content.Load<Texture2D>("Knight1"), map[22].Position, map[22].MapPosition, "Knight", new AttackMelee(Content.Load<Texture2D>("Sword"), Content.Load<Texture2D>("Cross")), 4, 3, new KnightSkillWhirlwind(Content.Load<Texture2D>("Heal2")), 100, 4, 10));

            order.Add("Wizard");
            playerCharacters.Add(new PlayerCharacter(Content.Load<Texture2D>("Wizard1"), map[45].Position, map[45].MapPosition, "Wizard", new AttackRangeCross(Content.Load<Texture2D>("Sword"),Content.Load<Texture2D>("Cross")), 4, 3, new SkillWizardHeal(Content.Load<Texture2D>("Heal2")), 100, 4, 10));

            order.Add("Ranger");
            playerCharacters.Add(new PlayerCharacter(Content.Load<Texture2D>("Ranger1"), map[85].Position, map[85].MapPosition, "Ranger", new AttackRangeXCross(Content.Load<Texture2D>("Sword"), Content.Load<Texture2D>("Cross")), 5, 2, new SkillRangerBomb(Content.Load<Texture2D>("Sword")), 100, 5, 13));

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

            if (selectedTiles.Exists(x => x.Collision.Intersects(musRec))) {
                int tileNumber = selectedTiles.FindIndex(x => x.Collision.Intersects(musRec));
                if (phase == states.MovePhase1) {
                    List<List<Vector2>> Path = new List<List<Vector2>>();

                    map.Find(x => x.MapPosition == playerCharacters[indexNumber].MapPosition).Occupied = false;
                    int mapNumber = map.FindIndex(x => x.Collision.Intersects(musRec));
                    playerCharacters[indexNumber].Position = map[mapNumber].Position;
                    playerCharacters[indexNumber].MapPosition = map[mapNumber].MapPosition;
                    map[mapNumber].Occupied = true;
                    selectedTiles.Clear();

                    //Round over/Move over
                    phase = states.ChoosePhase;
                    orderNumber++;
                }
                else if (phase == states.AttackPhase1 && selectedTiles[tileNumber].Occupied == true) {
                    //Damage
                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
                    playerCharacters.Where(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].Damage);

                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
                    selectedTiles.Clear();


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

            if (playerCharacters.Exists(x => x.Name == order[orderNumber])) {
                int indexNumber = playerCharacters.FindIndex(x => x.Name == order[orderNumber]);

                if (moveButton.Update(Mouse.GetState(), oldMouse)) {
                    if (phase == states.ChoosePhase) {
                        selectedTiles.Clear();
                        selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].MoveSpeed && x.Occupied == false));

                        selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
                        phase = states.MovePhase1;
                    }
                    else if (phase == states.MovePhase1) {
                        selectedTiles.Clear();
                        phase = states.ChoosePhase;
                    }
                }

                if (attackButton.Update(Mouse.GetState(), oldMouse)) {
                    if (phase == states.ChoosePhase) {
                        selectedTiles.Clear();
                        playerCharacters[indexNumber].Attack.PrepareSkill(playerCharacters, indexNumber, map, selectedTiles);
                        phase = states.AttackPhase1;
                    }
                    else if (phase == states.AttackPhase1) {
                        selectedTiles.Clear();
                        phase = states.ChoosePhase;
                    }
                }

                if (skillButton.Update(Mouse.GetState(), oldMouse)) {
                    if (phase == states.ChoosePhase) {
                        selectedTiles.Clear();
                        playerCharacters[indexNumber].Skill.PrepareSkill(playerCharacters, indexNumber, map, selectedTiles);
                        phase = states.SkillPhase1;
                    }
                    else if (phase == states.SkillPhase1) {
                        selectedTiles.Clear();
                        phase = states.ChoosePhase;
                    }
                }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released) {
                    MouseState mus = Mouse.GetState();
                    Rectangle musRec = new Rectangle(mus.X, mus.Y, 1, 1);

                    if (selectedTiles.Exists(x => x.Collision.Intersects(musRec))) {
                        int tileNumber = selectedTiles.FindIndex(x => x.Collision.Intersects(musRec));
                        if (phase == states.MovePhase1) {
                            List<List<Vector2>> Path = new List<List<Vector2>>();

                            map.Find(x => x.MapPosition == playerCharacters[indexNumber].MapPosition).Occupied = false;
                            int mapNumber = map.FindIndex(x => x.Collision.Intersects(musRec));
                            playerCharacters[indexNumber].Position = map[mapNumber].Position;
                            playerCharacters[indexNumber].MapPosition = map[mapNumber].MapPosition;
                            map[mapNumber].Occupied = true;
                            selectedTiles.Clear();

                            //Round over/Move over
                            phase = states.ChoosePhase;
                            orderNumber++;
                        }
                        else if (phase == states.AttackPhase1 && selectedTiles[tileNumber].Occupied == true) {
                            playerCharacters[indexNumber].Attack.InvokeSkill(playerCharacters, indexNumber, map, selectedTiles, tileNumber);
                            //Round over/Attack over
                            phase = states.ChoosePhase;
                            orderNumber++;
                        }
                        else if (phase == states.SkillPhase1) {
                            playerCharacters[indexNumber].Skill.InvokeSkill(playerCharacters, indexNumber, map, selectedTiles, tileNumber);
                            phase = states.ChoosePhase;
                            orderNumber++;
                        }
                    }
                }
            }




            // TODO: Add your update logic here
            if (orderNumber >= order.Count) {
                orderNumber = 0;
            }
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

            spriteBatch.Begin();

            /*for (int i = 0; i < 3; i++) {
                spriteBatch.Draw(Content.Load<Texture2D>("DSC_0089"), new Vector2(378 * i + 0, 0), Color.White);
            }*/

            switch (phase) {
                case states.MovePhase1:
                    spriteBatch.Draw(moveButton.Texture, moveButton.Hitbox, Color.White);
                    break;
                case states.EnemyPhase:
                    break;
                case states.AttackPhase1:
                    spriteBatch.Draw(attackButton.Texture, attackButton.Hitbox, Color.White);
                    break;
                case states.ChoosePhase:
                    spriteBatch.Draw(moveButton.Texture, moveButton.Hitbox, Color.White);
                    spriteBatch.Draw(attackButton.Texture, attackButton.Hitbox, Color.White);
                    spriteBatch.Draw(skillButton.Texture, skillButton.Hitbox, Color.White);
                    break;
                case states.SkillPhase1:
                    spriteBatch.Draw(skillButton.Texture, skillButton.Hitbox, Color.White);
                    break;
                default:
                    break;
            }

            map.ForEach(x => x.Draw(spriteBatch, Content.Load<Texture2D>("Bushes and dirt_0")));
            map.ForEach(x => x.Draw(spriteBatch, Content.Load<Texture2D>("Bushes and dirt_6")));

            playerCharacters.ForEach(x => x.Draw(spriteBatch));

            if (phase == states.MovePhase1) {
                selectedTiles.ForEach(x => x.Draw(spriteBatch, playerCharacters[playerCharacters.FindIndex(y => y.Name == order[orderNumber])].Texture));
            }
            else if (phase == states.AttackPhase1) {
                playerCharacters[playerCharacters.FindIndex(x => x.Name == order[orderNumber])].Attack.Draw(selectedTiles, spriteBatch);
            }
            else if (phase == states.SkillPhase1) {
                playerCharacters[playerCharacters.FindIndex(x => x.Name == order[orderNumber])].Skill.Draw(selectedTiles, spriteBatch);
            }

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
