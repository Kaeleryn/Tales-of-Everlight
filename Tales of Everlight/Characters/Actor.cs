using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tales_of_Everlight.Damage;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Tales_of_Everlight.Characters;

public enum AnimationState
{
    Running,
    Jumping,
    Attacking,
    Death
}

public abstract class Actor
{
    private bool _isFacingRight = true; // Напрямок персонажа
    private Texture2D _runningTexture; // Текстура бігу
    private Texture2D _jumpingTexture; // Текстура стрибка
    private Texture2D _attackTexture; // Текстура атаки
    private Texture2D _currentTexture;
    private Texture2D _deathTexture;

    private float _invincibilityTimer; // Тривалість неуразливості

    private const float INVINCIBILITY_DURATION = 1.0f;


    private float _deathAnimationTimer = 0f;
    private const float DEATH_ANIMATION_DURATION = 1.5f;

    private bool _isInvincible = false; // Чи персонаж неуразливий
    public bool IsDead = false; // Чи персонаж мертвий
    public bool IsDying = false;
    public bool IsLanded = false; // Чи персонаж приземлився

    // public static int Health { get; set; } = 100; // Здоров'я персонажа
    public static int Health { get; set; } = 200; // Здоров'я персонажа
    public static int MaxHealth { get; set; } = 20000; // Максимальне здоров'я персонажа

    public static int Damage { get; set; } = 30;
    public static int StandardDamage { get; set; } = 30;
    public static float Speed { get; set; } = 8.5f;
    public static float StandardSpeed { get; set; } = 8.5f;

    public AnimationState AnimationState { get; set; } // Стан анімації

    private Vector2 _velocity; // Швидкість
    private Rectangle _rect { get; set; } // Головний прямокутник

    public Rectangle Rect
    {
        get => _rect;
        set => _rect = value;
    }

    private Rectangle _srect { get; set; } // Прямокутник для вибору кадру

    public Rectangle Srect
    {
        get => _srect;
        set => _srect = value;
    }

    private const float Gravity = 0.65f; // Гравітація
    private const float FrameTime = 0.1f; // Час між кадрами


    public bool IsMoving { get; set; } = true;
    public bool IsOnGround { get; set; }

    public bool IsAttacking { get; set; } = false;

    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }

    private int Rows { get; set; } = 1;
    private int Columns { get; set; }

    public int _currentFrame;
    private int _totalFrames;
    private int _stepsDone { get; set; }
    public int StepsDone => _stepsDone;
    private double _timeSinceLastFrame;

    private int _jumpCount = 0; // Додаємо лічильник стрибків
    private const int MaxJumps = 2; // Два стрибки дозволені


    public Rectangle SourceRectangle;
    public Rectangle DestinationRectangle;
    public SpriteEffects SpriteEffects;

    protected Actor(ContentManager content, Rectangle rect, Rectangle srect)
    {
        Health = MaxHealth;
        _currentFrame = 0;
        _runningTexture = content.Load<Texture2D>("animatedSprite");
        _jumpingTexture = content.Load<Texture2D>("animatedSpriteJumping");
        _attackTexture = content.Load<Texture2D>("animatedSpriteAttacking");
        _deathTexture = content.Load<Texture2D>("animatedSpriteDeath");

        _rect = rect;
        _srect = srect;
        _velocity = Vector2.Zero;
    }

    public void StartDying()
    {
        IsDying = true;
        AnimationState = AnimationState.Death;
        Console.WriteLine("Started Dying");
        Velocity = Vector2.Zero;
        _currentFrame = 0;
    }

    protected Actor() => _velocity = Vector2.Zero;

    public void HandleMovement(KeyboardState keystate, KeyboardState previousState, MouseState mouseState,
        MouseState previousMState, GameTime gameTime)
    {

        if (_isInvincible)
        {
            _invincibilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_invincibilityTimer >= INVINCIBILITY_DURATION)
            {
                _isInvincible = false;
                _invincibilityTimer = 0f;
            }
        }

        if (IsDead || IsDying)
        {
            AnimationState = AnimationState.Death;
            _velocity.Y += Gravity;
            _velocity.Y = Math.Min(20.0f, _velocity.Y);
            UpdateFrame(gameTime);
            return;
        }


        if (AnimationState != AnimationState.Attacking)
        {
            HandleVerticalMovement(keystate, previousState, gameTime);
            HandleHorisontalMovement(keystate, previousState, gameTime);
        }
        // HandleAttack(mouseState, previousMState, gameTime);

        HandleAttackMovement(keystate, mouseState, previousMState, gameTime);
    }

    public void HandleVerticalMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        _velocity.Y += Gravity;
        _velocity.Y = Math.Min(20.0f, _velocity.Y);

        if (IsOnGround)
        {
            _jumpCount = 0; // Якщо персонаж на землі, скидаємо лічильник стрибків

            if (AnimationState == AnimationState.Jumping)
            {
                AnimationState = AnimationState.Running;
                _currentFrame = 0;
            }
        }

        if (keystate.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space) && _jumpCount < MaxJumps)
        {
            IsLanded = false;
            _velocity.Y = -16;
            AnimationState = AnimationState.Jumping;
            IsOnGround = false;
            _jumpCount++; // Збільшуємо кількість стрибків
        }
    }


    public void TakeDamage(int damage)
    {
        if (_isInvincible || IsDead || IsDying) return;
        Health -= damage;

        StartInvincibility();


        if (Health <= 0 && !IsDead && !IsDying)
        {
            StartDying();
        }
    }

    public void StartInvincibility()
    {
        _isInvincible = true;
        _invincibilityTimer = 0f;
    }

    public void HandleHorisontalMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        Decelerate();
       
            if (keystate.IsKeyDown(Keys.D))
            {
                _velocity.X = Speed;
                _isFacingRight = true;
                IsMoving = true;
                UpdateFrame(gameTime);
                if (IsOnGround) AnimationState = AnimationState.Running;
            }
            else if (keystate.IsKeyDown(Keys.A))
            {
                _velocity.X = -Speed;
                _isFacingRight = false;
                IsMoving = true;
                UpdateFrame(gameTime);
                if (IsOnGround) AnimationState = AnimationState.Running;
            }
            else
            {
                IsMoving = false;
            }
        
    }


    public void HandleAttackMovement(KeyboardState keystate, MouseState mousestate, MouseState previousMState, GameTime gameTime)
    {
        if ((mousestate.LeftButton == ButtonState.Pressed || keystate.IsKeyDown(Keys.Enter)) && !IsAttacking && IsOnGround)
        {
            _velocity = Vector2.Zero; // Зупиняємо персонажа
            IsAttacking = true;
            AnimationState = AnimationState.Attacking; // Змінюємо стан анімації
            _currentFrame = 0; // Скидаємо кадр анімації
        }

        if (AnimationState == AnimationState.Attacking)
        {
            UpdateFrame(gameTime);
        }
    }

    public void Decelerate()
    {
        _velocity.X *= 0.8f;

        if (_velocity.X < 0.5f && _velocity.X > -0.5f)
            _velocity.X = 0;
    }


    public void UpdateFrame(GameTime gameTime)
    {
        _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeSinceLastFrame >= FrameTime)
        {
            _timeSinceLastFrame -= FrameTime;

            if (AnimationState == AnimationState.Death)
            {
                // Handle death animation
                if (_currentFrame < _totalFrames - 1)
                {
                    _currentFrame = (_currentFrame + 1) % _totalFrames;
                    // Track total death animation time
                    _deathAnimationTimer += FrameTime;
                    Console.WriteLine("Death animation frame: " + _currentFrame);
                }
                else
                {
                    // Death animation complete
                    IsDying = true;
                    IsDead = true;

                    // Keep on last frame
                    _currentFrame = _totalFrames - 1;
                    Console.WriteLine("died");
                }
            }
            else if (AnimationState == AnimationState.Attacking)
            {
                // Move to next frame in attack animation
                if (_currentFrame < _totalFrames - 1)
                {
                    _currentFrame = (_currentFrame + 1) % _totalFrames;
                    if (_currentFrame == 5) Attack.Execute(AttackerType.Player);
                }
                else
                {
                    // Attack animation completed
                    _currentFrame = 0;
                    IsAttacking = false;

                    // Return to previous state when attack is done
                    AnimationState = IsOnGround ? AnimationState.Running : AnimationState.Jumping;
                }
            }
            else if (AnimationState == AnimationState.Jumping && !IsOnGround)
            {
                if (_currentFrame < _totalFrames - 1)
                {
                    _currentFrame = (_currentFrame + 1) % _totalFrames;
                }
            }
            else
            {
                _currentFrame = (_currentFrame + 1) % _totalFrames;

                if (AnimationState == AnimationState.Running && (_currentFrame == 0 || _currentFrame == 3))
                {
                    _stepsDone++;
                }
            }
        }
    }

    private void AnimationHandler()
    {
        switch (AnimationState)
        {
            case AnimationState.Running:
                _currentTexture = _runningTexture;
                Columns = 6;
                Rows = 1;
                break;
            case AnimationState.Jumping:
                _currentTexture = _jumpingTexture;
                Columns = 7;
                Rows = 1;
                break;
            case AnimationState.Attacking:
                _currentTexture = _attackTexture;
                Columns = 1;
                Rows = 8;
                break;
            case AnimationState.Death:
                _currentTexture = _deathTexture;
                Columns = 1;
                Rows = 10;
                break;
        }

        if (IsOnGround && !IsMoving && !IsDead && !IsDying)
        {
            _currentFrame = 0;
            _currentTexture = _runningTexture;
            Columns = 6;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_runningTexture, _rect, _srect, Color.White);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 location)
    {
        AnimationHandler();

        Color drawColor = Color.White;
        if (_isInvincible)
        {
            // Flash on and off every 0.1 seconds
            if ((int)(_invincibilityTimer * 10) % 2 == 0)
            {
                drawColor = Color.Red;
            }
        }

        _totalFrames = Columns * Rows;
        int width = _currentTexture.Width / Columns;
        int height = _currentTexture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;


        if (AnimationState == AnimationState.Death)
        {
            if (_isFacingRight)
            {
                SourceRectangle = new Rectangle(width * column, height * row, width, height);
                DestinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);
                SpriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                SourceRectangle = new Rectangle(width * column, height * row, width, height);
                DestinationRectangle = new Rectangle((int)location.X - 80, (int)location.Y, width, height);
                SpriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
        }
        else if (AnimationState != AnimationState.Attacking)
        {
            SourceRectangle = new Rectangle(width * column, height * row, width, height);
            DestinationRectangle = new Rectangle((int)location.X - 80, (int)location.Y, width, height);
            SpriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
        else // Анімація атаки
        {
            if (_isFacingRight)
            {
                SourceRectangle = new Rectangle(width * column, height * row, width, height);
                DestinationRectangle = new Rectangle((int)location.X - 20, (int)location.Y - 41, width, height);
                SpriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else // Чесно, не моє поняття як це зробити без хардкода
            {
                SourceRectangle = new Rectangle(width * column, height * row, width, height);
                DestinationRectangle = new Rectangle((int)location.X - 80, (int)location.Y - 41, width, height);
                SpriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
        }


        spriteBatch.Draw(_currentTexture, DestinationRectangle, SourceRectangle, drawColor, 0f, Vector2.Zero,
            SpriteEffects, 0f);
    }

    public void DrawBoundingBox(SpriteBatch spriteBatch, Texture2D hitboxTexture)
    {
        Rectangle boundingBox = Rect;
        spriteBatch.Draw(hitboxTexture, new Rectangle(boundingBox.X, boundingBox.Y, boundingBox.Width, 1),
            Color.Red);
        spriteBatch.Draw(hitboxTexture, new Rectangle(boundingBox.X, boundingBox.Y, 1, boundingBox.Height),
            Color.Red);
        spriteBatch.Draw(hitboxTexture,
            new Rectangle(boundingBox.X, boundingBox.Y + boundingBox.Height - 1, boundingBox.Width, 1),
            Color.Red);
        spriteBatch.Draw(hitboxTexture,
            new Rectangle(boundingBox.X + boundingBox.Width - 1, boundingBox.Y, 1, boundingBox.Height),
            Color.Red);
    }
}