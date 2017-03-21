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

        SpriteFont GothicFont;
        public static Texture2D Cross;
        
        public static Texture2D Move;

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

            Cross = Content.Load<Texture2D>("Cross");
            GothicFont = Content.Load<SpriteFont>("Gothic");
            Move = Content.Load<Texture2D>("MoveAni");

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

            if (turnMaster.SelectedTiles.Exists(x => x.Collision.Intersects(musRec))) {
                int tileNumber = turnMaster.SelectedTiles.FindIndex(x => x.Collision.Intersects(musRec));
                if (phase == states.MovePhase1) {
                    List<List<Vector2>> Path = new List<List<Vector2>>();

                    map.Find(x => x.MapPosition == playerCharacters[indexNumber].MapPosition).Occupied = false;
                    int mapNumber = map.FindIndex(x => x.Collision.Intersects(musRec));
                    playerCharacters[indexNumber].Position = map[mapNumber].Position;
                    playerCharacters[indexNumber].MapPosition = map[mapNumber].MapPosition;
                    map[mapNumber].Occupied = true;
                    turnMaster.SelectedTiles.Clear();

                    //Round over/Move over
                    phase = states.ChoosePhase;
                    orderNumber++;
                }
                else if (phase == states.AttackPhase1 && turnMaster.SelectedTiles[tileNumber].Occupied == true) {
                    //Damage
                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == turnMaster.SelectedTiles[tileNumber].MapPosition).Health);
                    playerCharacters.Where(x => x.MapPosition == turnMaster.SelectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].Damage);

                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == turnMaster.SelectedTiles[tileNumber].MapPosition).Health);
                    turnMaster.SelectedTiles.Clear();


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
            World.AllCharacters.ForEach(x => x.AnimationUpdate((float)gameTime.ElapsedGameTime.TotalSeconds));
            if (DrawEngine.QueuedAnimations.Count == 0) {

                World.Map.Where(x => x.Inhabited).ToList().ForEach(x => {
                    if (x.Inhabitant.Health <= 0) {
                        World.AllCharacters.Remove(x.Inhabitant);
                        x.RemoveInhabitor();
                    }
                });

                if (World.OrderNumber >= World.AllCharacters.Count) {
                    World.OrderNumber = 0;
                }

                var turnMaster = World.AllCharacters[World.OrderNumber];

                turnMaster.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                if (turnMaster.AllEnemies.Count <= 0) {
                    //Won or Lost
                    throw new NotImplementedException();
                }

            }



            DrawEngine.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            // TODO: Add your update logic here
            World.oldMouse = Mouse.GetState();
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

            for (int i = 0; i < World.AllCharacters.Count; i++) {
                spriteBatch.Draw(Content.Load<Texture2D>("Art"), new Vector2(30, 180 + 50 * i), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.Draw(World.AllCharacters[i].Texture, new Vector2(32, 182 + 50 * i), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            if (World.OrderNumber < World.AllCharacters.Count) {
                spriteBatch.Draw(Content.Load<Texture2D>("Circle"), new Vector2(30, 180 + 50 * World.OrderNumber), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

            World.Buttons.ForEach(x => x.Draw(spriteBatch));

            World.Map.ForEach(x => x.Draw(spriteBatch));
            World.Map.ForEach(x => x.Draw(spriteBatch));

            DrawEngine.Draw(spriteBatch, GothicFont);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
