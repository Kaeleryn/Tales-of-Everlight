using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using SharpDX.Direct2D1.Effects;
using Tales_of_Everlight.Characters;
using Tales_of_Everlight.Screens;

namespace Tales_of_Everlight
{
    public class Main : Game
    {
        private static GumService Gum => GumService.Default;
        private Panel _mainPanel;
        public static bool isGame;
        public static bool isPaused;
        public static bool isDialog;
        public static List<Buff> BuffList;
        public static Portal Portal1;
        public static Portal Portal2;
        private static bool dialogShown = false;
        private static bool dialog2shown = false;
        public static List<Portal> Portals => new List<Portal> { Portal1, Portal2 };
        public static Main Instance { get; set; }
        public static bool isGameOver;
        // private Texture2D _menuBackgroundTexture;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private static Camera _camera;
        private static Level1 _level1 = new Level1();
        private static Level2 _level2 = new Level2();
        private static bool _isHudVisible;
        private Texture2D _mainHeroSprite;
        //private Texture2D _goblinSprite;


        //private Texture2D _hudTexture;
        private Texture2D _healthIcon;
        private Texture2D _rectangleTexture;
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
        private static List<Rectangle> _intersections;

        private Texture2D _hitboxTexture;

        public static List<Enemy> EnemyList;
        public static List<Buff> BuffsList;

        private static ContentManager content;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Instance = this;

            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;

            _camera = new Camera(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight));
            _intersections = new List<Rectangle>();
            _level1 = new Level1();
            _level2 = new Level2();

            EnemyList = new();
        }

        protected override void Initialize()
        {
            Gum.Initialize(this, "GumProject/GumProject.gumx");

            content = Main.Instance.Content;

            var screen = new MainMenu();
            screen.AddToRoot();

            content = Instance.Content;

            base.Initialize();
            _previousKeyState = Keyboard.GetState();
            _previousMState = Mouse.GetState();
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }


        public static void InitGame(object sender, EventArgs e)
        {
            Gum.Root.Children.Clear();

            // Reset game states
            isGame = true;
            isPaused = false;
            isGameOver = false;
            // Reset camera position
            _camera.Position = Vector2.Zero;
            BuffList = new List<Buff>();

            // Reset main hero
            MainHero = new MainHero(content,
                new Rectangle(2000, 25 * Tilesize, 64,
                    128), //rect це позиція персонажа, srect треба для відладки, але тоді треба використовувати інший Draw метод і текстурку player_static
                new Rectangle(0, 0, 128, 128));

            // Reset enemies
            BuffList.Clear();
            EnemyList.Clear();
            _level1.SpawnBuffs(content);
            _level1.SpawnEnemies(content);
            EnemyList = _level1.Enemies;
            BuffList = _level1.Buffs;

            Portal1 = new Portal(content, new Rectangle(251 * Tilesize, 33 * Tilesize, 128, 128));
            Portal2 = new Portal(content, new Rectangle(248 * Tilesize, 63 * Tilesize, 128, 128));
            Portals.Add(Portal1);
            Portals.Add(Portal2);
            // Reinitialize level
            _level1 = new Level1();
            _level2 = new Level2();
            
            _level1.Initialize(content);
            _level2.Initialize(content);

            // Reset other states if needed
            _intersections.Clear();
            _isHudVisible = false;
        }

        protected override void LoadContent()
        {
            //_menuBackgroundTexture = Content.Load<Texture2D>("menu_background");
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            MainHero = new MainHero(Content,
                new Rectangle(2000, 25 * Tilesize, 64,
                    128), //rect це позиція персонажа, srect треба для відладки, але тоді треба використовувати інший Draw метод і текстурку player_static
                new Rectangle(0, 0, 128, 128));


           

            //_hudTexture = Content.Load<Texture2D>("hud+");
            _hudFont = Content.Load<SpriteFont>("hudFont");

            _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            _hitboxTexture = new Texture2D(GraphicsDevice, 1, 1);
            _hitboxTexture.SetData(new[] { Color.White });


            _level1.Initialize(Content);
            
        }

        private async void GameOver()
        {
            isGameOver = true;
            await Task.Delay(2000);
            GumService.Default.Root.Children.Clear();
            var gameOverScreen = new GameOverScreen();
            gameOverScreen.AddToRoot();
        }

        protected override void Update(GameTime gameTime)
        {
            //_level1.IsActive = false;
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Delete))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !_previousKeyState.IsKeyDown(Keys.Escape) && isGame)
            {
                if (!isPaused && !isDialog)
                {
                    if (!isGameOver)
                    {
                        isPaused = true;
                        var pauseScreen = new PauseScreen();
                        pauseScreen.AddToRoot();
                    }
                }
                else
                {
                    if (!isDialog)
                    {
                        if (!isGameOver)
                        {
                            isPaused = false;
                            Gum.Root.Children.Clear();
                        }
                    }
                }
            }

            if (isGame && !isPaused && !isDialog && MainHero.IsDead && !isGameOver)
            {
                GameOver();
            }
            // if (isPaused && !isGame)
            // {
            //     _backgroundColor = new Color(100, 150, 200); // Ensure background color is set for the main menu
            // }
            //Console.WriteLine(2);
            //Attack.Execute();

            if (isGame && !isPaused && !isDialog && !MainHero.IsDead)
            {
                MainHero.HandleMovement(Keyboard.GetState(), _previousKeyState, Mouse.GetState(), _previousMState,
                    gameTime);

                if (!isGameOver)
                {
                    if (_level1.IsActive)
                    {
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
                                    if (!MainHero.IsLanded)
                                    {
                                        MainHero.IsLanded = true;
                                        MainHero._currentFrame = 0;
                                    }

                                    MainHero.IsOnGround = true;
                                    MainHero.Velocity = MainHero.Velocity with { Y = 0.0f };
                                    if (!MainHero.IsDead && !MainHero.IsDying)
                                        MainHero.AnimationState = AnimationState.Running;

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

                    }
                    else
                    {
                        #region Main Hero Collision Handler

                        // add player's velocity and grab the intersecting tiles
                        MainHero.Rect = MainHero.Rect with { X = MainHero.Rect.X + (int)MainHero.Velocity.X };
                        _intersections = GetIntersectingTilesHorizontal(MainHero.Rect);

                        foreach (var rect in _intersections)
                        {
                            // handle collisions if the tile position exists in the tile map layer.
                            if (_level2.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
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
                            if (_level2.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
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
                                    if (!MainHero.IsLanded)
                                    {
                                        MainHero.IsLanded = true;
                                        MainHero._currentFrame = 0;
                                    }

                                    MainHero.IsOnGround = true;
                                    MainHero.Velocity = MainHero.Velocity with { Y = 0.0f };
                                    if (!MainHero.IsDead && !MainHero.IsDying)
                                        MainHero.AnimationState = AnimationState.Running;

                                    //_mainHero._currentFrame = 0;
                                }
                                else if (MainHero.Velocity.Y < 0.0f)
                                {
                                    MainHero.Rect = MainHero.Rect with { Y = collision.Bottom };
                                    MainHero.Velocity = MainHero.Velocity with { Y = 0.0f };
                                }
                            }
                        }

                        if (!dialogShown)
                        {
                            isDialog = true;
                            Gum.Root.Children.Clear();
                            new DialogScreenSecond().AddToRoot();
                            dialogShown = true;
                        }
                        
                        #endregion

                        #region Main Hero Spike Damage Handler

                        // add player's velocity and grab the intersecting tiles

                        // same as horizontal collisions


                        _intersections = GetIntersectingTilesVertical(MainHero.Rect);


                        foreach (var rect in _intersections)
                        {
                            if (_level2.Spikes.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
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
                                if (_level2.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
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
                                if (_level2.Collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _))
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
                        var darkKnight = EnemyList.OfType<DarkKnightFB>().FirstOrDefault();
                        if (darkKnight.IsDead && !dialog2shown)
                        {
                            isDialog = true;
                            Gum.Root.Children.Clear();
                            new DialogScreenTheEnd().AddToRoot();
                            dialog2shown = true;
                        }
                        #endregion
                        if (!dialogShown)
                        {
                            isDialog = true;
                            Gum.Root.Children.Clear();
                            new DialogScreenSecond().AddToRoot();
                            dialogShown = true;
                        }
                    }
                }

                foreach (var enemy in EnemyList)
                {
                    enemy.Update(gameTime);
                    enemy.MovementHandler();
                }

                foreach (var portal in Portals)
                {
                    portal.Update(gameTime);
                    if (MainHero.Rect.Intersects(portal.Rect) && _level1.IsActive && portal.IsVisible)
                    {
                        _level1.IsActive = false;
                        
                        EnemyList.Clear();
                        _level2.SpawnEnemies(content);
                        EnemyList= _level2.Enemies;
                        
                        BuffList.Clear();
                        _level2.SpawnBuffs(content);
                        BuffList = _level2.Buffs;
                        
                        portal.IsVisible = false;
                        MainHero.Rect = new Rectangle(87*Tilesize, 20 * Tilesize, 64, 128);
                        
                    }
                }

                if (!isGameOver)
                {
                    foreach (var buff in BuffList)
                    {
                        Rectangle buffRect = new Rectangle((int)buff.Position.X, (int)buff.Position.Y,
                            buff.Texture.Width, buff.Texture.Height);

                        buff.Update(gameTime);

                        if (MainHero.Rect.Intersects(buffRect) && !buff.IsActive)
                        {
                            buff.IsActive = true;
                            buff.ApplyEffect();
                        }
                    }
                }

                HandleInput();
                UpdateCameraPosition();
                // UpdateGameElements(gameTime);
            }


            _previousKeyState = Keyboard.GetState();

            Gum.Update(gameTime);

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


            if (_level1.IsActive)
            {
                if ( (MainHero.Rect.X >= (_graphics.PreferredBackBufferWidth / 2.0f) &&
                      MainHero.Rect.X <= _level1.Width - (_graphics.PreferredBackBufferWidth / 2.0f)))
                {
                    cameraX = MainHero.Rect.X;
                }
                
            }
            else
            {
                if ( (MainHero.Rect.X >= (_graphics.PreferredBackBufferWidth / 2.0f) &&
                     MainHero.Rect.X <= _level2.Width - (_graphics.PreferredBackBufferWidth / 2.0f)))
                {
                    cameraX = MainHero.Rect.X;
                }
                
            }
            // Ensure the camera stays within level boundaries
            
           

            if (_level1.IsActive)
            {
                cameraX = Math.Clamp(cameraX, ((_graphics.PreferredBackBufferWidth / 2.0f)/ _camera.Zoom),
                    _level1.Width - ((_graphics.PreferredBackBufferWidth / 2.0f)/ _camera.Zoom));
                
            }
            else
            {
                cameraX = Math.Clamp(cameraX, ((_graphics.PreferredBackBufferWidth / 2.0f)/ _camera.Zoom),
                    _level2.Width - ((_graphics.PreferredBackBufferWidth / 2.0f)/ _camera.Zoom));
            }
            // Clamp the camera position to the level boundaries
            

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

            if (isGame)
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
            if (_level1.IsActive)
            {
                _level1.Draw(_spriteBatch);
            }
            else
            {
                _level2.Draw(_spriteBatch);
            }

            MainHero.Draw(_spriteBatch, new Vector2(MainHero.Rect.X, MainHero.Rect.Y)
                // , _hitboxTexture
            );
            foreach (var portal in Portals)
            {
                portal.Draw(_spriteBatch);
                portal.Draw(_spriteBatch);
            }

            foreach (var enemy in EnemyList)
            {
                enemy.Draw(_spriteBatch, new Vector2(enemy.Rect.X, enemy.Rect.Y), _hitboxTexture);
                enemy.DrawBoundingBox(_spriteBatch, _hitboxTexture);
            }

            foreach (var buff in BuffList)
            {
                buff.Draw(_spriteBatch, buff.Position);
            }


            _spriteBatch.End();
        }

        private void DrawHealthBar()
        {
            _healthIcon = Content.Load<Texture2D>("HealthBar"); // Load the PNG image
            _rectangleTexture = Content.Load<Texture2D>("progressbar");
            _hudFont = Content.Load<SpriteFont>("hudFont");

            // Define the position and size of the health bar
            int barWidth = 304;
            int barHeight = 76;
            int xPosition = 15;
            int yPosition = 15;

            // Calculate the width of the filled portion based on the hero's health
            float healthPercentage = (float)MainHero.Health / (float)MainHero.MaxHealth;
            int filledWidth = (int)(224 * healthPercentage);


            // Draw the background of the health bar
            // _spriteBatch.Draw(_rectangleTexture, new Rectangle(xPosition - 5, yPosition - 5, barWidth + 10, barHeight + 10), Color.Black);
            _spriteBatch.Draw(_rectangleTexture, new Rectangle(xPosition + 76, yPosition + 32, 224, 16), Color.DarkRed);
            _spriteBatch.Draw(_rectangleTexture, new Rectangle(xPosition + 76, yPosition + 32, filledWidth, 16),
                Color.IndianRed);

            _spriteBatch.Draw(_healthIcon, new Rectangle(xPosition, yPosition, barWidth, barHeight), Color.White);
        }

        private void DrawHudElements()
        {
            _spriteBatch.Begin();

            DrawHealthBar();

            
            
            // Draw up to 3 active buff icons in the top right corner
            int iconSize = 48;
            int spacing = 30;
            int startX = _graphics.PreferredBackBufferWidth - iconSize - spacing;
            int y = spacing;

            // Get only active buffs, take up to 3
            var activeBuffs = BuffList.Where(b => b.IsActive).Take(3).ToList();

            for (int i = 0; i < activeBuffs.Count; i++)
            {
                var buff = activeBuffs[i];
                int x = startX - i * (iconSize + spacing);
                _spriteBatch.Draw(buff.Texture, new Rectangle(x, y, iconSize, iconSize), Color.White);
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