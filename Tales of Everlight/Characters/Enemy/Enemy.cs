using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;

namespace Tales_of_Everlight;

public abstract class Enemy
{
    public static int Health { get; set; }

    public EnemyState State;
    public EnemyAnimationState AnimationState;

    public float StateTimer = 0f;

    private const float MOVEMENT_DURATION = 2.0f;
    private const float IDLE_DURATION = 1.0f;


    public SpriteEffects SpriteEffects { get; set; }
    public bool Damaged { get; set; }
    public bool IsDead { get; set; }
    public double _timeSinceLastFrame;
    public Rectangle SourceRectangle { get; set; }
    public Rectangle DestinationRectangle { get; set; }

    public float _deathAnimationTime = 0f;
    public float _deathAnimationDuration = 1.0f; // Duration of the death animation in seconds
    public double FrameTime = 0.1;
    public Texture2D IdleTexture { get; set; }
    public Texture2D CurrentTexture;
    public Texture2D AttackTexture;
    public bool IsAttacking = false;

    public Texture2D MovingTexture { get; set; }

    private Rectangle _rect;
    private Rectangle _srect;

    public Rectangle Rect
    {
        get => _rect;
        set => _rect = value;
    }

    public Rectangle Srect
    {
        get => _srect;
        set => _srect = value;
    }

    public bool IsOnGround { get; set; } = false;
    protected int Rows { get; set; }
    protected int Columns { get; set; }

    public int _currentFrame;
    public int _totalFrames;

    public bool IsFacingRight = false;
    private bool isMoving = true;
    private bool isJumping = true;

    private const float GRAVITY = 1.2f;

    public bool IsJumping
    {
        get => isJumping;
        set => isJumping = value;
    }

    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    public Vector2 Velocity { get; set; }

    public void TakeDamage(int damage)
    {
        Console.WriteLine("TakeDamage method called");
        Health -= damage;
        Damaged = true;

        if (Health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        IsDead = true;
        // Handle enemy death logic here
        Console.WriteLine("Enemy has died.");
        // For example, you might want to remove the enemy from the game or play a death animation.
    }

    protected Enemy(ContentManager content, Rectangle rect, Rectangle srect)
    {
        Health = 100;


        _currentFrame = 0;


        _rect = rect;
        _srect = srect;
        Velocity = Vector2.Zero;
    }

    protected Enemy()
    {
        Velocity = Vector2.Zero;
    }


    public void MovementHandler()
    {
        if (IsDead) return;
        Velocity = Velocity with { Y = Velocity.Y + GRAVITY };
        Velocity = Velocity with { Y = Math.Min(20.0f, Velocity.Y) };
    }

    public virtual void MoveLeft()
    {
        Velocity = Velocity with { X = -5f };
        IsFacingRight = false;
    }

    public virtual void MoveRight()
    {
        Velocity = Velocity with { X = 5f };
        IsFacingRight = true;
    }

    public virtual void BehaviorHandler(GameTime gameTime)
    {
        if (IsDead) return;
        
    }

    public virtual void SearchEnemy()
    {
        
    }
    
    public virtual void PerformAttack()
    {
        
    }


    public virtual void Update(GameTime gameTime)
    {
        if (IsDead)
        {
            _deathAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateFrame(gameTime);
            if (_deathAnimationTime >= _deathAnimationDuration)
            {
                //Main.RemoveEnemy(this);
                Console.WriteLine("Enemy has been removed.");
            }

            return;
        }

        BehaviorHandler(gameTime);
        UpdateFrame(gameTime);
        
    }

    public virtual void UpdateFrame(GameTime gametime)
    {
        _timeSinceLastFrame += gametime.ElapsedGameTime.TotalSeconds;
        if (_timeSinceLastFrame >= FrameTime)
        {
            _timeSinceLastFrame -= FrameTime;

            _currentFrame++;
            if (_currentFrame >= _totalFrames)
            {
                _currentFrame = 0;
            }
        }
    }

    public virtual void AnimationHandler()
    {
        Columns = 1;
        Rows = 5;
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 location, Texture2D hitboxTexture)
    {
        AnimationHandler();
        _totalFrames = Columns * Rows;
        int width = CurrentTexture.Width / Columns;
        int height = CurrentTexture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;


        SourceRectangle = new Rectangle(width * column, height * row, width, height);
        DestinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);
        SpriteEffects spriteEffects = IsFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


        if (IsDead)
        {
            float alpha = 1.0f - (_deathAnimationTime / _deathAnimationDuration);
            Color deathColor = Color.White * alpha;
            spriteBatch.Draw(CurrentTexture, location, SourceRectangle, deathColor, 0f, Vector2.Zero, 1f, spriteEffects,
                0f);
        }
        else if (Damaged)
        {
            spriteBatch.Draw(CurrentTexture, location, SourceRectangle, Color.Red, 0f, Vector2.Zero, 1f, spriteEffects,
                0f);
            Damaged = false;
        }
        else
        {
            spriteBatch.Draw(CurrentTexture, location, SourceRectangle, Color.White, 0f, Vector2.Zero, 1f,
                spriteEffects, 0f);
        }
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