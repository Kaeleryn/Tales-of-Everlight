using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tales_of_Everlight
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Camera _camera;
        private Level1 _level1;
        private bool _isHudVisible;
        private Texture2D _mainHeroSprite;
        private Texture2D _squareSprite;
        private Texture2D _rectangleTexture;
        //private Texture2D _hudTexture;
        private SpriteFont _hudFont;
        private Color _backgroundColor = new(145, 221, 207, 255);
        private MainHero _mainHero = new();
        private Square _square = new();
        private KeyboardState _keyState;
        private KeyboardState _previousKeyState;
        private const int Tilesize = 64;
        private List<Rectangle> intersections;
        private Dictionary<Vector2, int> foreground;
        private Dictionary<Vector2, int> collisions;
        private Texture2D _hitboxTexture;
        private SplashScreen _splashScreen;
        private bool _isSplashScreenVisible;
        private Texture2D _splashTexture;
        public MainMenu MainMenu { get; set; }
        public PauseMenu PauseMenu { get; set; }

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;

            _camera = new Camera(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            intersections = new List<Rectangle>();
            _level1 = new Level1();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _previousKeyState = Keyboard.GetState();
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _mainHero.Position = new Vector2(100, 100);
            _square.Position = new Vector2(1000, 100);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mainHeroSprite = Content.Load<Texture2D>("animatedSprite");
            _squareSprite = Content.Load<Texture2D>("enemy1");
            _mainHero = new MainHero(_mainHeroSprite, new Vector2(500, 1000), 1, 6);
            _square = new Square(_squareSprite, new Vector2(1000, 100), 5, 1);

            //_hudTexture = Content.Load<Texture2D>("hud+");
            _hudFont = Content.Load<SpriteFont>("hudFont");

            _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            _hitboxTexture = new Texture2D(GraphicsDevice, 1, 1);
            _hitboxTexture.SetData(new[] { Color.White });

            _splashTexture = Content.Load<Texture2D>("splashScreen");
            _splashScreen = new SplashScreen(_splashTexture, 3.0); // Display splash screen for 3 seconds
            _isSplashScreenVisible = true;

            _level1.Initialize(Content);
            
            MainMenu = MainMenu.LoadContent(this, Content);
            PauseMenu = PauseMenu.LoadContent(this, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_isSplashScreenVisible)
            {
                _splashScreen.Update(gameTime);
                if (_splashScreen.IsFinished)
                {
                    _isSplashScreenVisible = false;
                    MainMenu.IsVisible = true;
                }
            }
            else
            {
                HandleInput();

                if (MainMenu.IsVisible)
                {
                    MainMenu.Update(Mouse.GetState());
                }
                else if (PauseMenu.IsVisible)
                {
                    PauseMenu.Update(Mouse.GetState());
                }
                else
                {
                    UpdateCameraPosition();
                    UpdateGameElements(gameTime);
                }
            }

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(Keys.I) && !_previousKeyState.IsKeyDown(Keys.I))
                _isHudVisible = !_isHudVisible;

            if (currentKeyState.IsKeyDown(Keys.Escape) && !_previousKeyState.IsKeyDown(Keys.Escape))
            {
                PauseMenu.IsVisible = !PauseMenu.IsVisible;
            }

            _previousKeyState = currentKeyState;
        }

        private void UpdateCameraPosition()
        {
            Vector2 characterPosition = _mainHero.Position;

            if (characterPosition.X >= (_graphics.PreferredBackBufferWidth / 2.0f) && characterPosition.X <= 3000)
            {
                _camera.Position = new Vector2(characterPosition.X, _camera.Position.Y);
            }
        }

        private void UpdateGameElements(GameTime gameTime)
        {
            _mainHero.CollisionHandler(_level1.Collisions, Tilesize);
            _mainHero.MovementHandler(gameTime);

            _square.CollisionHandler(_level1.Collisions, Tilesize);
            _square.MovementHandler();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            if (_isSplashScreenVisible)
            {
                _splashScreen.Draw(_spriteBatch);
            }
            else if (MainMenu.IsVisible)
            {
                MainMenu.Draw(_spriteBatch);
            }
            else if (PauseMenu.IsVisible)
            {
                PauseMenu.Draw(_spriteBatch);
            }
            else
            {
                DrawGameElements();
                DrawHudElements();
            }

            base.Draw(gameTime);
        }

        private void DrawGameElements()
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformation());

            _mainHero.Draw(_spriteBatch, _mainHero.Position, _hitboxTexture);
            _square.Draw(_spriteBatch, _square.Position, _hitboxTexture);
            _level1.Draw(_spriteBatch);

            foreach (var rect in intersections)
            {
                DrawRectHollow(_spriteBatch, new Rectangle(rect.X * Tilesize, rect.Y * Tilesize, Tilesize, Tilesize), 4);
            }

            _spriteBatch.End();
        }

        private void DrawHudElements()
        {
            _spriteBatch.Begin();

            if (_isHudVisible)
            {
                _spriteBatch.DrawString(_hudFont, $"Velocity: {_mainHero.Velocity}", new Vector2(10, 0), Color.White);
                _spriteBatch.DrawString(_hudFont, $"isMoving: {_mainHero.IsMoving}", new Vector2(10, 30), Color.White);
                _spriteBatch.DrawString(_hudFont, $"isJumping: {_mainHero.IsJumping}", new Vector2(10, 60), Color.White);
                _spriteBatch.DrawString(_hudFont, $"Steps done: {_mainHero.StepsDone}", new Vector2(10, 90), Color.White);
            }

            _spriteBatch.End();
        }

        private void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        {
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), Color.White);
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), Color.White);
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), Color.White);
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), Color.White);
        }
    }
}