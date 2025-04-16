using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight;

public class Goblin : Enemy
{
    public Goblin(ContentManager content, Rectangle rect, Rectangle srect) : base(content, rect, srect)
    {
        Texture = content.Load<Texture2D>("GoblinIdle-Sheet");
        IsFacingRight = false;
    }

    public Goblin()
    {
        
    }
    
    public override void AnimationHandler()
    {
        Columns = 8;
        Rows = 1;
    }
    
    public override void Draw(SpriteBatch spriteBatch, Vector2 location, Texture2D hitboxTexture)
    {
        AnimationHandler();
        _totalFrames = Columns * Rows;
        int width = Texture.Width / Columns;
        int height = Texture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;

        if (IsFacingRight)
        {
            SourceRectangle = new Rectangle(width * column, height * row, width, height);
            DestinationRectangle = new Rectangle((int)location.X , (int)location.Y, width, height);
            SpriteEffects = IsFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            
        }
else
{ 
    SourceRectangle = new Rectangle(width * column, height * row, width, height);
    DestinationRectangle = new Rectangle((int)location.X+60, (int)location.Y, width, height);
    SpriteEffects = IsFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;}
       
        


        if (IsDead)
        {
            float alpha = 1.0f - (_deathAnimationTime / _deathAnimationDuration);
            Color deathColor = Color.White * alpha;
            spriteBatch.Draw(Texture, location, SourceRectangle, deathColor, 0f, Vector2.Zero, 1f, SpriteEffects, 0f);
        }
        else if (Damaged)
        {
            spriteBatch.Draw(Texture, location, SourceRectangle, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects, 0f);
            Damaged = false;
        }
        else
        {
            spriteBatch.Draw(Texture, location, SourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects, 0f);
        }
    }
}