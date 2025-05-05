using System;
using System.Collections.Generic;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using Tales_of_Everlight.Characters;

namespace Tales_of_Everlight
{
    public class Main : Game
    {
        private GumService Gum => GumService.Default;
        private Panel _mainPanel;
        private bool _isGame;
        private bool _isPaused;
        private bool _isDialog;

        // private Texture2D _menuBackgroundTexture;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly Camera _camera;
        private Level1 _level1;
        private bool _isHudVisible;
        private Texture2D _mainHeroSprite;
        //private Texture2D _goblinSprite;

        private Texture2D _rectangleTexture;

        //private Texture2D _hudTexture;
        private SpriteFont _hudFont;
        private readonly Color _backgroundColor = new(145, 221, 207, 255);
        public static MainHero MainHero = new();


        private Goblin _goblin = new();
        private Sceleton _skeleton = new();
        private Mushroom _mushroom = new();
        private Worm _worm = new();


        private KeyboardState _previousKeyState;
        private MouseState _previousMState;
        private const int Tilesize = 64;
        private List<Rectangle> _intersections;

        private Texture2D _hitboxTexture;

        public static List<Enemy> EnemyList;


        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;

            _camera = new Camera(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight));
            _intersections = new List<Rectangle>();
            _level1 = new Level1();

            EnemyList = new();
        }

        protected override void Initialize()
        {
            Gum.Initialize(this);
            InitMainMenu();

            base.Initialize();
            _previousKeyState = Keyboard.GetState();
            _previousMState = Mouse.GetState();
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        private void InitMainMenu()
        {
            Gum.Root.Children.Clear();

            _mainPanel = new Panel();
            _mainPanel.Visual.AddToRoot();
            _mainPanel.Dock(Dock.Fill);

            var titleLabel = new Label();
            titleLabel.Text = "Tales of Everlight";
            titleLabel.Anchor(Anchor.Top);
            _mainPanel.AddChild(titleLabel);

            var buttonPanel = new StackPanel();
            buttonPanel.Spacing = 3;
            buttonPanel.Anchor(Anchor.Center);

            var startButton = new Button();
            startButton.Text = "Start Game";
            startButton.Visual.Width = 200;
            startButton.Click += InitGame;
            buttonPanel.AddChild(startButton);

            var optionsButton = new Button();
            optionsButton.Text = "Options";
            optionsButton.Visual.Width = 200;
            optionsButton.Click += InitOptions;
            buttonPanel.AddChild(optionsButton);

            var exitButton = new Button();
            exitButton.Text = "Exit";
            exitButton.Visual.Width = 200;
            exitButton.Click += InitExit;
            buttonPanel.AddChild(exitButton);

            _mainPanel.AddChild(buttonPanel);
        }

        private void ShowDialog()
        {
            Gum.Root.Children.Clear();
            _isDialog = true;

            var gamePanel = new Panel();
            gamePanel.Visual.AddToRoot();
            gamePanel.Dock(Dock.Fill);

            // Label Panel (Top)
            var label = new Label();
            label.Text = "MAMU IBAV";
            label.Anchor(Anchor.Top);
            gamePanel.AddChild(label);

            // Button Panel (Center)
            var buttonPanel = new StackPanel();
            buttonPanel.Spacing = 3;
            buttonPanel.Anchor(Anchor.Center);

            var resumeButton = new Button();
            resumeButton.Text = "OK";
            resumeButton.Visual.Width = 200;
            resumeButton.Click += (_, _) =>
            {
                Gum.Root.Children.Clear();
                _isDialog = false;
            };
            buttonPanel.AddChild(resumeButton);

            gamePanel.AddChild(buttonPanel);
        }

        private void InitGame(object sender, EventArgs e)
        {
            Gum.Root.Children.Clear();

            // Reset game states
            _isGame = true;
            _isPaused = false;

            // Reset camera position
            _camera.Position = Vector2.Zero;

            // Reset main hero
            MainHero = new MainHero(Content,
                new Rectangle(0, 0, 64, 128),
                new Rectangle(0, 0, 128, 128));

            // Reset enemies
            EnemyList.Clear();
            _goblin = new Goblin(Content, new Rectangle(1000, 100, 64, 64), new Rectangle(0, 0, 70, 70));
            _skeleton = new Sceleton(Content, new Rectangle(1000, 100, 64, 128), new Rectangle(0, 0, 128, 128));
            _mushroom = new Mushroom(Content, new Rectangle(1000, 100, 64, 128), new Rectangle(0, 0, 128, 128));
            _worm = new Worm(Content, new Rectangle(1000, 100, 64, 64), new Rectangle(0, 0, 128, 128));
            EnemyList.Add(_goblin);
            EnemyList.Add(_skeleton);
            EnemyList.Add(_mushroom);
            EnemyList.Add(_worm);

            // Reinitialize level
            _level1 = new Level1();
            _level1.Initialize(Content);

            // Reset other states if needed
            _intersections.Clear();
            _isHudVisible = false;


            ShowDialog();
        }

        private void InitOptions(object sender, EventArgs e)
        {
            Gum.Root.Children.Clear();

            var optionsPanel = new Panel();
            optionsPanel.Visual.AddToRoot();
            optionsPanel.Dock(Dock.Fill);

            var label = new Label();
            label.Text = "Options";
            label.Anchor(Anchor.Top);
            optionsPanel.AddChild(label);

            var optionsControls = new StackPanel();
            optionsControls.Anchor(Anchor.Center); // Center the optionsControls within the optionsPanel
            optionsControls.Spacing = 3;
            optionsPanel.AddChild(optionsControls);

            var fullscreenCheck = new CheckBox();
            fullscreenCheck.IsChecked = false;
            fullscreenCheck.Text = "Fullscreen";
            fullscreenCheck.Checked += (sender, _) => System.Diagnostics.Debug.WriteLine(
                $"Checkbox checked? {(sender as CheckBox).IsChecked}");
            fullscreenCheck.Unchecked += (sender, _) => System.Diagnostics.Debug.WriteLine(
                $"Checkbox checked? {(sender as CheckBox).IsChecked}");
            optionsControls.AddChild(fullscreenCheck);

            var twoColumn = new StackPanel();
            twoColumn.Visual.ChildrenLayout = ChildrenLayout.LeftToRightStack;
            twoColumn.Visual.WidthUnits = DimensionUnitType.RelativeToChildren;
            optionsControls.AddChild(twoColumn);

            var spLabels = new StackPanel();
            spLabels.Spacing = 3;
            twoColumn.AddChild(spLabels);

            var volumeLabel = new Label();
            volumeLabel.Text = "Volume";
            spLabels.AddChild(volumeLabel);

            var musicLabel = new Label();
            musicLabel.Text = "Music";
            spLabels.AddChild(musicLabel);

            var spOptions = new StackPanel();
            spOptions.Spacing = 3;
            twoColumn.AddChild(spOptions);

            var volumeSlider = new Slider();
            volumeSlider.Value = 75;
            volumeSlider.ValueChanged += (sender, _) => System.Diagnostics.Debug.WriteLine(
                $"Volume Slider Value is {(sender as Slider).Value}");
            spOptions.AddChild(volumeSlider);

            var musicSlider = new Slider();
            musicSlider.Value = 50;
            musicSlider.ValueChanged += (sender, _) => System.Diagnostics.Debug.WriteLine(
                $"Music Slider Value is {(sender as Slider).Value}");
            spOptions.AddChild(musicSlider);

            var backButton = new Button();
            backButton.Text = "Back to Main Menu";
            backButton.Visual.Width = 200;
            backButton.Click += (_, _) =>
            {
                Gum.Root.Children.Clear();
                _mainPanel.AddToRoot();
            };
            optionsControls.AddChild(backButton);
        }

        private void InitExit(object sender, EventArgs e)
        {
            Exit();
        }

        private void InitPause()
        {
            Gum.Root.Children.Clear();

            if (_isPaused)
            {
                var gamePanel = new Panel();
                gamePanel.Visual.AddToRoot();
                gamePanel.Dock(Dock.Fill);

                // Label Panel (Top)
                var label = new Label();
                label.Text = "Pause";
                label.Anchor(Anchor.Top);
                gamePanel.AddChild(label);

                // Button Panel (Center)
                var buttonPanel = new StackPanel();
                buttonPanel.Spacing = 3;
                buttonPanel.Anchor(Anchor.Center);

                var resumeButton = new Button();
                resumeButton.Text = "Resume";
                resumeButton.Visual.Width = 200;
                resumeButton.Click += (_, _) =>
                {
                    Gum.Root.Children.Clear();
                    _isPaused = !_isPaused;
                };
                buttonPanel.AddChild(resumeButton);

                var backButton = new Button();
                backButton.Text = "Back to Main Menu";
                backButton.Visual.Width = 200;
                backButton.Click += (_, _) =>
                {
                    Gum.Root.Children.Clear();
                    _mainPanel.AddToRoot();
                    _isGame = false; // Ensure the game state is reset
                };
                buttonPanel.AddChild(backButton);
                gamePanel.AddChild(buttonPanel);
            }
        }

        protected override void LoadContent()
        {
            //_menuBackgroundTexture = Content.Load<Texture2D>("menu_background");
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //_mainHeroSprite = Content.Load<Texture2D>("animatedSprite");
            _mainHeroSprite = Content.Load<Texture2D>("animatedSprite");
            //_goblinSprite = Content.Load<Texture2D>("enemy1");
            //_mainHero = new MainHero(_mainHeroSprite, new Vector2(500, 1000), 1, 6);
            MainHero = new MainHero(Content,
                new Rectangle(0, 0, 64,
                    128), //rect це позиція персонажа, srect треба для відладки, але тоді треба використовувати інший Draw метод і текстурку player_static
                new Rectangle(0, 0, 128, 128));

            // Goblin = new Goblin(_goblinSprite, new Vector2(1000, 100), 5, 1);
             _goblin = new Goblin(Content,
                 new Rectangle(1000, 100, 64, 64),
                 //rect це позиція персонажа, srect треба для відладки, але тоді треба використовувати інший Draw метод і текстурку player_static);
                 new Rectangle(0, 0, 70, 70));
             _skeleton = new Sceleton(Content,
                 new Rectangle(1000, 100, 64, 128),
                 new Rectangle(0, 0, 128, 128));
            _mushroom = new Mushroom(Content,
                new Rectangle(1000, 100, 64, 128),
                new Rectangle(0, 0, 128, 128));
            _worm = new Worm(Content,
                new Rectangle(1000, 100, 64, 64),
                new Rectangle(0, 0, 128, 128));
            EnemyList.Add(_goblin);
            EnemyList.Add(_skeleton);
            EnemyList.Add(_mushroom);
            EnemyList.Add(_worm);

            //_hudTexture = Content.Load<Texture2D>("hud+");
            _hudFont = Content.Load<SpriteFont>("hudFont");

            _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            _hitboxTexture = new Texture2D(GraphicsDevice, 1, 1);
            _hitboxTexture.SetData(new[] { Color.White });


            _level1.Initialize(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Delete))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !_previousKeyState.IsKeyDown(Keys.Escape) && _isGame)
            {
                _isPaused = !_isPaused;
                InitPause();
            }

            // if (isPaused && !isGame)
            // {
            //     _backgroundColor = new Color(100, 150, 200); // Ensure background color is set for the main menu
            // }
            //Console.WriteLine(2);
            //Attack.Execute();

            if (_isGame && !_isPaused && !_isDialog)
            {
                MainHero.HandleMovement(Keyboard.GetState(), _previousKeyState, Mouse.GetState(), _previousMState,
                    gameTime);

                #region Main Hero Collision Handler

                // add player's velocity and grab the intersecting tiles
                MainHero.Rect = MainHero.Rect with { X = MainHero.Rect.X + (int)MainHero.Velocity.X };
                _intersections = GetIntersectingTilesHorizontal(MainHero.Rect);

                foreach (var rect in _intersections)
                {
                    // handle collisions if the tile position exists in the tile map layer.
                    if (_level1.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
                    {
                        // create temp rect to handle collisions (not necessary, you can optimize!)
                        Rectangle collision = new(
                            rect.X * Tilesize,
                            rect.Y * Tilesize,
                            Tilesize,
                            Tilesize
                        );

                        // handle collisions based on the direction the player is moving
                        if (MainHero.Velocity.X > 0.0f)
                        {
                            MainHero.Rect = MainHero.Rect with { X = collision.Left - MainHero.Rect.Width };
                        }
                        else if (MainHero.Velocity.X < 0.0f)
                        {
                            MainHero.Rect = MainHero.Rect with { X = collision.Right };
                        }
                    }
                }

                // same as horizontal collisions

                MainHero.Rect = MainHero.Rect with { Y = MainHero.Rect.Y + (int)MainHero.Velocity.Y };
                _intersections = GetIntersectingTilesVertical(MainHero.Rect);

                MainHero.IsOnGround = false;

                foreach (var rect in _intersections)
                {
                    if (_level1.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
                    {
                        Rectangle collision = new(
                            rect.X * Tilesize,
                            rect.Y * Tilesize,
                            Tilesize,
                            Tilesize
                        );

                        if (MainHero.Velocity.Y > 0.0f)
                        {
                            MainHero.Rect = MainHero.Rect with { Y = collision.Top - MainHero.Rect.Height };
                            MainHero.IsOnGround = true;
                            MainHero.Velocity = MainHero.Velocity with { Y = 0.0f };
                            if(!MainHero.IsDead || !MainHero.IsDying)MainHero.AnimationState = AnimationState.Running;

                            //_mainHero._currentFrame = 0;
                        }
                        else if (MainHero.Velocity.Y < 0.0f)
                        {
                            MainHero.Rect = MainHero.Rect with { Y = collision.Bottom };
                            MainHero.Velocity = MainHero.Velocity with { Y = 0.0f };
                        }
                    }
                }

                #endregion


                #region Main Hero Spike Damage Handler

                // add player's velocity and grab the intersecting tiles

                // same as horizontal collisions


                _intersections = GetIntersectingTilesVertical(MainHero.Rect);


                foreach (var rect in _intersections)
                {
                    if (_level1.Spikes.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
                    {
                        MainHero.TakeDamage(10);
                    }
                }

                #endregion

                #region Enemy Collision Handler

                foreach (var enemy in EnemyList)
                {
                    enemy.Rect = enemy.Rect with { X = enemy.Rect.X + (int)enemy.Velocity.X };
                    _intersections = GetIntersectingTilesHorizontal(enemy.Rect);

                    foreach (var rect in _intersections)
                    {
                        if (_level1.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
                        {
                            Rectangle collision = new(
                                rect.X * Tilesize,
                                rect.Y * Tilesize,
                                Tilesize,
                                Tilesize
                            );

                            if (enemy.Velocity.X > 0.0f)
                            {
                                enemy.Rect = enemy.Rect with { X = collision.Left - enemy.Rect.Width };
                            }
                            else if (enemy.Velocity.X < 0.0f)
                            {
                                enemy.Rect = enemy.Rect with { X = collision.Right };
                            }
                        }
                    }

                    enemy.Rect = enemy.Rect with { Y = enemy.Rect.Y + (int)enemy.Velocity.Y };
                    _intersections = GetIntersectingTilesVertical(enemy.Rect);

                    enemy.IsOnGround = false;

                    foreach (var rect in _intersections)
                    {
                        if (_level1.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
                        {
                            Rectangle collision = new(
                                rect.X * Tilesize,
                                rect.Y * Tilesize,
                                Tilesize,
                                Tilesize
                            );

                            if (enemy.Velocity.Y > 0.0f)
                            {
                                enemy.Rect = enemy.Rect with { Y = collision.Top - enemy.Rect.Height };
                                enemy.IsOnGround = true;
                                enemy.Velocity = enemy.Velocity with { Y = 0.0f };
                            }
                            else if (enemy.Velocity.Y < 0.0f)
                            {
                                enemy.Rect = enemy.Rect with { Y = collision.Bottom };
                            }
                        }
                    }
                }

                #endregion

                foreach (var enemy in EnemyList)
                {
                    enemy.Update(gameTime);
                    enemy.MovementHandler();
                }


                HandleInput();

                UpdateCameraPosition();
                // UpdateGameElements(gameTime);
            }


            Gum.Update(gameTime);

            _previousKeyState = Keyboard.GetState();
            base.Update(gameTime);
        }

        private void HandleInput()
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(Keys.I) && !_previousKeyState.IsKeyDown(Keys.I))
                _isHudVisible = !_isHudVisible;

            _previousKeyState = currentKeyState;

            MouseState mouseState = Mouse.GetState();
            _previousMState = mouseState;
        }

        private void UpdateCameraPosition()
        {
            float cameraX = _camera.Position.X;
            float cameraY = MainHero.Rect.Y + 100;

            // Ensure the camera stays within level boundaries
            if (MainHero.Rect.X >= (_graphics.PreferredBackBufferWidth / 2.0f) &&
                MainHero.Rect.X <= _level1.Width - (_graphics.PreferredBackBufferWidth / 2.0f))
            {
                cameraX = MainHero.Rect.X;
            }

            // Clamp the camera position to the level boundaries
            cameraX = Math.Clamp(cameraX, _graphics.PreferredBackBufferWidth / 2.0f,
                _level1.Width - (_graphics.PreferredBackBufferWidth / 2.0f));

            _camera.Position = new Vector2(cameraX, cameraY);
        }

        // private void UpdateGameElements(GameTime gameTime)
        // {
        //     Goblin.CollisionHandler(_level1.Collisions, Tilesize);
        //     Goblin.MovementHandler();
        // }

        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(_backgroundColor);

            if (!_isGame || _isPaused || _isDialog)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue); // Replace with your desired color

                //_spriteBatch.Begin();
                //_spriteBatch.Draw(_menuBackgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                //_spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(_backgroundColor);
                DrawGameElements();
                DrawHudElements();
            }

            Gum.Draw();
            base.Draw(gameTime);
        }

        private void DrawGameElements()
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformation());


            //_mainHero.Draw(_spriteBatch);

            _level1.Draw(_spriteBatch);
            MainHero.Draw(_spriteBatch, new Vector2(MainHero.Rect.X, MainHero.Rect.Y)
                // , _hitboxTexture
            );
            MainHero.DrawBoundingBox(_spriteBatch, _hitboxTexture);

            foreach (var enemy in EnemyList)
            {
                enemy.Draw(_spriteBatch, new Vector2(enemy.Rect.X, enemy.Rect.Y), _hitboxTexture);
                enemy.DrawBoundingBox(_spriteBatch, _hitboxTexture);
            }

            _spriteBatch.End();
        }

        private void DrawHudElements()
        {
            _spriteBatch.Begin();

            if (_isHudVisible)
            {
                _spriteBatch.DrawString(_hudFont, $"Velocity: {MainHero.Velocity}", new Vector2(10, 0), Color.White);
                _spriteBatch.DrawString(_hudFont, $"isMoving: {MainHero.IsMoving}", new Vector2(10, 30), Color.White);
                _spriteBatch.DrawString(_hudFont, $"IsOnGround: {MainHero.IsOnGround}", new Vector2(10, 60),
                    Color.White);
                _spriteBatch.DrawString(_hudFont, $"Steps done: {MainHero.StepsDone}", new Vector2(10, 90),
                    Color.White);
            }

            _spriteBatch.End();
        }

        private List<Rectangle> GetIntersectingTilesHorizontal(Rectangle target)
        {
            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % Tilesize)) / Tilesize;
            int heightInTiles = (target.Height - (target.Height % Tilesize)) / Tilesize;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {
                    intersections.Add(new Rectangle(
                        (target.X + x * Tilesize) / Tilesize,
                        (target.Y + y * (Tilesize - 1)) / Tilesize,
                        Tilesize,
                        Tilesize
                    ));
                }
            }

            return intersections;
        }

        private List<Rectangle> GetIntersectingTilesVertical(Rectangle target)
        {
            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % Tilesize)) / Tilesize;
            int heightInTiles = (target.Height - (target.Height % Tilesize)) / Tilesize;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {
                    intersections.Add(new Rectangle(
                        (target.X + x * (Tilesize - 1)) / Tilesize,
                        (target.Y + y * Tilesize) / Tilesize,
                        Tilesize,
                        Tilesize
                    ));
                }
            }

            return intersections;
        }
    }
}