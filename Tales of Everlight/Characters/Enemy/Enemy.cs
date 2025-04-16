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
    public bool Damaged { get; set; }
    public bool IsDead { get; private set; }
    private double _timeSinceLastFrame;

    private float _deathAnimationTime = 0f;
    private float _deathAnimationDuration = 1.0f; // Duration of the death animation in seconds
    public double FrameTime = 0.1;
    private Texture2D Texture { get; set; }

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
    private int Rows { get; set; }
    private int Columns { get; set; }

    private int _currentFrame;
    private int _totalFrames;

    private bool isFacingLeft = false;
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
        Texture = content.Load<Texture2D>("enemy1");


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


    public void Update(GameTime gameTime)
    {
        if (IsDead)
        {
            _deathAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateFrame(gameTime);
            if (_deathAnimationTime >= _deathAnimationDuration)
            {
                Main.RemoveEnemy(this);
                Console.WriteLine("Enemy has been removed.");
            }

            return;
        }

        UpdateFrame(gameTime);
    }

    public void UpdateFrame(GameTime gametime)
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

    public void AnimationHandler()
    {
        Columns = 1;
        Rows = 5;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 location, Texture2D hitboxTexture)
    {
        AnimationHandler();
        _totalFrames = Columns * Rows;
        int width = Texture.Width / Columns;
        int height = Texture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;


        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        SpriteEffects spriteEffects = isFacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);


        if (IsDead)
        {
            float alpha = 1.0f - (_deathAnimationTime / _deathAnimationDuration);
            Color deathColor = Color.White * alpha;
            spriteBatch.Draw(Texture, location, sourceRectangle, deathColor, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }
        else if (Damaged)
        {
            spriteBatch.Draw(Texture, location, sourceRectangle, Color.Red, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
            Damaged = false;
        }
        else
        {
            spriteBatch.Draw(Texture, location, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
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