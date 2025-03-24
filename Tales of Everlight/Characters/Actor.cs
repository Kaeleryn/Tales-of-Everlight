using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Tales_of_Everlight;

public enum AnimationState
{
    Running,
    Jumping,
    Attacking
}

public abstract class Actor
{
    private bool _isFacingRight = true; // Напрямок персонажа
    private Texture2D _runningTexture; // Текстура бігу
    private Texture2D _jumpingTexture; // Текстура стрибка
    private Texture2D _attackTexture; // Текстура атаки
    private Texture2D _currentTexture;

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
    private const double FrameTime = 0.1; // Час між кадрами

    private Rectangle _boundingBox;

    public Rectangle BoundingBox
    {
        get => Rect;
        set => _boundingBox = Rect;
    }

    public bool IsMoving { get; set; } = true;
    public bool IsOnGround { get; set; }

    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }


    private int Rows { get; set; } = 1;
    private int Columns { get; set; }

    private int _currentFrame;
    private int _totalFrames;
    private int _stepsDone { get; set; }
    public int StepsDone => _stepsDone;
    private double _timeSinceLastFrame;

    private int _jumpCount = 0; // Додаємо лічильник стрибків
    private const int MaxJumps = 2; // Два стрибки дозволені


    public Rectangle sourceRectangle;
    public Rectangle destinationRectangle;
    public SpriteEffects spriteEffects;

    protected Actor(ContentManager content, Rectangle rect, Rectangle srect)
    {
        _currentFrame = 0;
        _runningTexture = content.Load<Texture2D>("animatedSprite");
        _jumpingTexture = content.Load<Texture2D>("animatedSpriteJumping");
        _attackTexture = content.Load<Texture2D>("animatedSpriteAttacking");

        _rect = rect;
        _srect = srect;
        _velocity = Vector2.Zero;
    }

    protected Actor() => _velocity = Vector2.Zero;

    public void HandleMovement(KeyboardState keystate, KeyboardState previousState, MouseState mouseState,
        MouseState previousMState, GameTime gameTime)
    {
        if (AnimationState != AnimationState.Attacking)
        {
            HandleVerticalMovement(keystate, previousState, gameTime);
            HandleHorisontalMovement(keystate, previousState, gameTime);
        }
        // HandleAttack(mouseState, previousMState, gameTime);

        HandleAttackMovement(mouseState, previousMState, gameTime);
    }

    public void HandleVerticalMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        _velocity.Y += Gravity;
        _velocity.Y = Math.Min(20.0f, _velocity.Y);

        if (IsOnGround)
        {
            _jumpCount = 0; // Якщо персонаж на землі, скидаємо лічильник стрибків
        }

        if (keystate.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space) && _jumpCount < MaxJumps)
        {
            _velocity.Y = -15;
            AnimationState = AnimationState.Jumping;
            IsOnGround = false;
            _jumpCount++; // Збільшуємо кількість стрибків
        }
    }

    // public void HandleAttack(MouseState mouseState, MouseState previousState, GameTime gameTime)
    // {
    //     if (mouseState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
    //     {
    //         AnimationState = AnimationState.Attacking;
    //
    //         _currentFrame = 0;
    //     }
    //
    //     if (AnimationState == AnimationState.Attacking)
    //     {
    //         UpdateAttackFrame(gameTime);
    //     }
    // }
    //
    // private void UpdateAttackFrame(GameTime gameTime)
    // {
    //     _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;
    //
    //     if (_timeSinceLastFrame >= FrameTime)
    //     {
    //         _timeSinceLastFrame -= FrameTime;
    //
    //         // Move to next frame in attack animation
    //         if (_currentFrame < _totalFrames - 1)
    //         {
    //             _currentFrame++;
    //         }
    //         else
    //         {
    //             // Attack animation completed
    //             _currentFrame = 0;
    //             // Return to previous state when attack is done
    //             AnimationState = IsOnGround ? AnimationState.Running : AnimationState.Jumping;
    //         }
    //     }
    // }


    public void HandleHorisontalMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        Decelerate();

        if (keystate.IsKeyDown(Keys.D))
        {
            _velocity.X = 10;
            _isFacingRight = true;
            IsMoving = true;
            UpdateFrame(gameTime);
            if (IsOnGround) AnimationState = AnimationState.Running;
        }
        else if (keystate.IsKeyDown(Keys.A))
        {
            _velocity.X = -10;
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


    public void HandleAttackMovement(MouseState mousestate, MouseState previousMState, GameTime gameTime)
    {
        if (mousestate.LeftButton == ButtonState.Pressed)
        {
            _velocity = Vector2.Zero; // Зупиняємо персонажа
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

    // private void UpdateFrame(GameTime gameTime)
    // {
    //     _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;
    //
    //     if (_timeSinceLastFrame >= FrameTime)
    //     {
    //         _timeSinceLastFrame -= FrameTime;
    //
    //         if (AnimationState == AnimationState.Jumping && !IsOnGround)
    //         {
    //             if (_currentFrame < _totalFrames - 1)
    //             {
    //                 _currentFrame = (_currentFrame + 1) % _totalFrames;
    //             }
    //         }
    //         else
    //         {
    //             _currentFrame = (_currentFrame + 1) % _totalFrames;
    //
    //             if (AnimationState == AnimationState.Running && (_currentFrame == 0 || _currentFrame == 3))
    //             {
    //                 _stepsDone++;
    //             }
    //         }
    //     }
    // }


    private void UpdateFrame(GameTime gameTime)
    {
        _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeSinceLastFrame >= FrameTime)
        {
            _timeSinceLastFrame -= FrameTime;

            if (AnimationState == AnimationState.Attacking)
            {
                // Move to next frame in attack animation
                if (_currentFrame < _totalFrames - 1)
                {
                    _currentFrame = (_currentFrame + 1) % _totalFrames;
                }
                else
                {
                    // Attack animation completed
                    _currentFrame = 0;
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
        }

        if (IsOnGround && !IsMoving)
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

        _totalFrames = Columns * Rows;
        int width = _currentTexture.Width / Columns;
        int height = _currentTexture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;

        if (AnimationState != AnimationState.Attacking)
        {
            sourceRectangle = new Rectangle(width * column, height * row, width, height);
            destinationRectangle = new Rectangle((int)location.X - 80, (int)location.Y, width, height);
            spriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
        else // Анімація атаки
        {
            if (_isFacingRight)
            {
                sourceRectangle = new Rectangle(width * column, height * row, width, height);
                destinationRectangle = new Rectangle((int)location.X - 20, (int)location.Y - 41, width, height);
                spriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else // Чесно, не моє поняття як це зробити без хардкода
            {
                sourceRectangle = new Rectangle(width * column, height * row, width, height);
                destinationRectangle = new Rectangle((int)location.X - 80, (int)location.Y - 41, width, height);
                spriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
        }


        spriteBatch.Draw(_currentTexture, destinationRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero,
            spriteEffects, 0f);
    }

    public void DrawBoundingBox(SpriteBatch spriteBatch, Texture2D hitboxTexture)
    {
        Rectangle boundingBox = BoundingBox;
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