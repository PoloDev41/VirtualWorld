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
        private MouseState prevMouseState;
        private bool _followingIndividu = false;
        private Individu _selectedIndividu;

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
            _monde.CreateWorld(100, 70);
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
            this.prevMouseState = Mouse.GetState();
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
            MouseState mouseState = Mouse.GetState();

            // movement of camera
            if (this.console.Opened == false)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    _followingIndividu = false;
                    _camera.Position -= new Vector2(0, 500) * deltaTime;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    _followingIndividu = false;
                    _camera.Position += new Vector2(0, 500) * deltaTime;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    _followingIndividu = false;
                    _camera.Position -= new Vector2(500, 0) * deltaTime;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    _followingIndividu = false;
                    _camera.Position += new Vector2(500, 0) * deltaTime;
                }
                if (keyboardState.IsKeyDown(Keys.NumPad0))
                {
                    _camera.Zoom -= 0.3f * deltaTime;
                }
                if (keyboardState.IsKeyDown(Keys.NumPad1))
                {
                    _camera.Zoom += 0.3f * deltaTime;
                }

                if (keyboardState.IsKeyDown(Keys.Space) && prevKeyboard.IsKeyDown(Keys.Space) == false)
                    GameOnPause = !GameOnPause;
            }

            if (GameOnPause == false)
                this._monde.Update(deltaTime * this._speedMultiplier);

            //click on individus to see it
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                _followingIndividu = false;
                // do something here
                Vector2 worldPosition = Vector2.Transform(mouseState.Position.ToVector2(), Matrix.Invert(_camera.GetViewMatrix()));
                //find the good individu
                Rectangle rec;
                bool find = false;
                for (int i = 0; i < this._monde.Individus.Count; i++)
                {
                    rec = new Rectangle(
                        (int)(this._monde.Individus[i].PositionImage.X),
                        (int)(this._monde.Individus[i].PositionImage.Y),
                        (int)(this._monde.Individus[i].PictureUsed.Width * this._monde.Individus[i].FactorAgrandissement * this._camera.Zoom),
                        (int)(this._monde.Individus[i].PictureUsed.Height * this._monde.Individus[i].FactorAgrandissement * this._camera.Zoom));
                    if(rec.Contains(worldPosition))
                    {
                        this._selectedIndividu = this._monde.Individus[i];
                        find = true;
                        break;
                    }
                }
                if (find == false)
                {
                    this._selectedIndividu = null;
                }
            }

            if(this._selectedIndividu != null && this._selectedIndividu.Mort == true)
            {
                this._selectedIndividu = null;
                _followingIndividu = false;
            }

            if(_followingIndividu == true && this._selectedIndividu != null)
            {
                /*Vector2 screenPosition = Vector2.Transform(this._selectedIndividu.Position, _camera.GetViewMatrix());
                screenPosition -= new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);*/
                this._camera.Position = this._selectedIndividu.Position - 
                                        new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);
            }

            this.prevKeyboard = keyboardState;
            this.prevMouseState = mouseState;
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

            DrawWorldData(spriteBatch);
            DrawIndividuData(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawWorldData(SpriteBatch spriteBatch)
        {
            int yPosition = 20;
            spriteBatch.DrawString(Font, "Age: " + _monde.Years, new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            if (this._monde.GlobalWarmingAction)
                spriteBatch.DrawString(Font, _monde.SaisonCourante.ToString() + " global warming !", new Vector2(20, yPosition), Color.Black);
            else
                spriteBatch.DrawString(Font, _monde.SaisonCourante.ToString(), new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Plantes: " + _monde.Plantes.Count.ToString(), new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Graines: " + _monde.Graines.Count.ToString(), new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Fruits: " + _monde.Fruits.Count.ToString(), new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Individus: " + _monde.Individus.Count.ToString(), new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Eggs: " + _monde.Eggs.Count.ToString(), new Vector2(20, yPosition), Color.Black);
            yPosition += 20;

            if (this.GameOnPause)
                spriteBatch.DrawString(Font, "PAUSE", new Vector2(20, yPosition), Color.Black);
            else
                spriteBatch.DrawString(Font, "Game speed: " + this._speedMultiplier, new Vector2(20, yPosition), Color.Black);
        }

        private void DrawIndividuData(SpriteBatch spriteBatch)
        {
            if (this._selectedIndividu == null)
                return;

            int yPosition = GraphicsDevice.Viewport.Bounds.Height - 140;
            int xPosition = GraphicsDevice.Viewport.Bounds.Width - 100;

            spriteBatch.DrawString(Font, "Life: " + _selectedIndividu.PointDeVie.ToString("0") + "//" + _selectedIndividu.PointDeVieDemarrage, 
                                    new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Age: " + _selectedIndividu.Age.ToString("0"),
                                    new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Number of children: " + _selectedIndividu.NumberChild,
                                    new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Size: " + _selectedIndividu.FactorAgrandissement,
                                    new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Ideal temperature: " + _selectedIndividu.IdealTemperature + "//" + _selectedIndividu.RefParcelle.Temperature,
                                    new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "Number of neurones: " + _selectedIndividu.Intelligence.Neurones.Length,
                                    new Vector2(20, yPosition), Color.Black);
            yPosition += 20;
            spriteBatch.DrawString(Font, "eggs: time " + _selectedIndividu.TempsEgg + " seuil " + _selectedIndividu.SeuilEgg + "//" + _selectedIndividu.PointDeVieDemarrage,
                                    new Vector2(20, yPosition), Color.Black);
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
                return "-Speed <speed multiplier>" + Environment.NewLine +
                        "-Follow" + Environment.NewLine +
                        "-Find [Oldest, Thinkest]";
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
                    case ("Follow"):
                        if (_selectedIndividu != null)
                        {
                            this._followingIndividu = true;
                            return "Follow the selected individu";
                        }
                        else
                        {
                            return "No selected individu";
                        }
                    case ("Find"):
                        {
                            if (arguments.Length > 1)
                            {
                                if(arguments[1] == "Oldest")
                                {
                                    this._followingIndividu = true;
                                    this._selectedIndividu = this._monde.Individus[0];
                                    return "Follow the oldest individu";
                                }
                                else if(arguments[1] == "Thinkest")
                                {
                                    int nbrNeurone = 0;
                                    Individu indRef = null;
                                    for (int i = 0; i < this._monde.Individus.Count; i++)
                                    {
                                        if(this._monde.Individus[i].Intelligence.Neurones.Length > nbrNeurone)
                                        {
                                            nbrNeurone = this._monde.Individus[i].Intelligence.Neurones.Length;
                                            indRef = this._monde.Individus[i];
                                        }
                                    }
                                    this._followingIndividu = true;
                                    this._selectedIndividu = indRef;
                                    return "Follow the thinkest individu";
                                }
                                else
                                {
                                    this._followingIndividu = false;
                                    return "unknow find command";
                                }
                            }
                            else
                                break;
                        }
                    default:
                        return "missing argument";
                }
            }
            return "missing argument";
        }

        #endregion


    }
}
