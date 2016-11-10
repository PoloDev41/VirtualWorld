using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameConsole;
using System;

namespace VirtualWorld
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game, IConsoleCommand
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Monde _monde;
        private Camera2D _camera;
        GameConsole console;
        bool GameOnPause = false;
        int _speedMultiplier = 1;
        public SpriteFont Font { get; private set; }
        KeyboardState prevKeyboard;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1440;
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _monde = new Monde();
            _camera = new Camera2D(GraphicsDevice.Viewport);
            _camera.Zoom = 0.35f;
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("ConsoleFont");

            _monde.LoadContent(Content);
            _monde.CreateWorld(125, 80);
            _camera.Position = new Vector2(125 * 9, 80 * 9);

            Services.AddService(typeof(SpriteBatch), spriteBatch);

            console = new GameConsole(this, spriteBatch, new GameConsoleOptions
            {
                ToggleKey = (int)Keys.F1,
                Font = Content.Load<SpriteFont>("ConsoleFont"),
                FontColor = Color.LawnGreen,
                Prompt = "~>",
                PromptColor = Color.Crimson,
                CursorColor = Color.OrangeRed,
                BackgroundColor = new Color(Color.Black, 150),
                PastCommandOutputColor = Color.Aqua,
                BufferColor = Color.Gold
            });
            console.AddCommand(_monde, 
                                this);
            this.prevKeyboard = Keyboard.GetState();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            // movement of camera
            if(this.console.Opened == false)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    _camera.Position -= new Vector2(0, 500) * deltaTime;
                if (keyboardState.IsKeyDown(Keys.Down))
                    _camera.Position += new Vector2(0, 500) * deltaTime;
                if (keyboardState.IsKeyDown(Keys.Left))
                    _camera.Position -= new Vector2(500, 0) * deltaTime;
                if (keyboardState.IsKeyDown(Keys.Right))
                    _camera.Position += new Vector2(500, 0) * deltaTime;
                if (keyboardState.IsKeyDown(Keys.NumPad0))
                    _camera.Zoom -= 0.3f * deltaTime;
                if (keyboardState.IsKeyDown(Keys.NumPad1))
                    _camera.Zoom += 0.3f * deltaTime;

                if (keyboardState.IsKeyDown(Keys.Space) && prevKeyboard.IsKeyDown(Keys.Space) == false)
                    GameOnPause = !GameOnPause;
            }

            if (GameOnPause == false)
                this._monde.Update(deltaTime * this._speedMultiplier);

            this.prevKeyboard = keyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var viewMatrix = _camera.GetViewMatrix();

            spriteBatch.Begin(transformMatrix: viewMatrix);

            _monde.DrawGround(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack,
                transformMatrix: viewMatrix);

            _monde.DrawActors(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(Font, _monde.SaisonCourante.ToString(), new Vector2(20, 20), Color.Black);
            spriteBatch.DrawString(Font, "Plantes: " + _monde.Plantes.Count.ToString(), new Vector2(20, 40), Color.Black);
            spriteBatch.DrawString(Font, "Graines: " + _monde.Graines.Count.ToString(), new Vector2(20, 60), Color.Black);
            spriteBatch.DrawString(Font, "Fruits: " + _monde.Fruits.Count.ToString(), new Vector2(20, 80), Color.Black);

            if(this.GameOnPause)
                spriteBatch.DrawString(Font, "PAUSE", new Vector2(20, 100), Color.Black);
            else
                spriteBatch.DrawString(Font, "Game speed: " + this._speedMultiplier, new Vector2(20, 100), Color.Black);


            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region IConsoleCommand

        public string Name
        {
            get
            {
                return "Game";
            }
        }

        public string Description
        {
            get
            {
                return "-Speed <speed multiplier>";
            }
        }

        public string Execute(string[] arguments)
        {
            if(arguments.Length > 0)
            {
                switch (arguments[0])
                {
                    case ("Speed"):
                        if(arguments.Length > 1)
                        {
                            try
                            {
                                _speedMultiplier = int.Parse(arguments[1]);
                                return "Speed changed";
                            }
                            catch (Exception)
                            {
                                return "Error parameter";
                            }
                        }
                        return "missing argument";
                    default:
                        return "missing argument";
                }
            }
            return "missing argument";
        }

        #endregion


    }
}
