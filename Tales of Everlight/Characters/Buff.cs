using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Tales_of_Everlight.Characters;
public enum BuffType
{
    Heal,
    IncreaseDamage,
    IncreaseSpeed,
}

public class Buff
{
    
    public BuffType Type;
   
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    protected float DURATION  = 15f;
    protected float TimeLeft { get; set; } = 0;
    
    public bool IsActive { get; set; }
    private bool IsExpired { get; set; } = false;
    protected float Amount { get; set; }


    public Buff(BuffType type, Vector2 position, float amount, ContentManager content)
    {
        Type = type;
        Position = position;
        Amount = amount;
        TimeLeft = 0;
        
        switch (type)
        {
            case BuffType.Heal:
                Texture = content.Load<Texture2D>("buff_health");
                break;
            case BuffType.IncreaseDamage:
                Texture = content.Load<Texture2D>("buff_damage");
                break;
            case BuffType.IncreaseSpeed:
                Texture = content.Load<Texture2D>("buff_speed");
                break;
        }
        
    }


    public void ApplyEffect()
    {
        switch (Type)
        {
            case BuffType.Heal:
                Heal((int)Amount);
                // Healing is instant, so we can deactivate the buff immediately
                IsActive = true;
                break;
            case BuffType.IncreaseDamage:
                MainHero.Damage += (int)Amount;
                IsActive = true;
                TimeLeft = 0;
                break;
            case BuffType.IncreaseSpeed:
                MainHero.Speed += Amount;
                IsActive = true;
                TimeLeft = 0;
                break;
        }
    }
    
    private void RemoveEffect()
    {
        if (!IsExpired)
        {
            IsExpired = true;



            // Only remove effects for buffs that need to be reversed
            switch (Type)
            {
                case BuffType.IncreaseDamage:
                    MainHero.Damage = (int)Amount;
                    break;
                case BuffType.IncreaseSpeed:
                    MainHero.Speed -= Amount;
                    break;
                // Heal doesn't need to be reversed as it's an instant effect
            }
        }
    }
    
    public void Update(GameTime gameTime)
    {
        if (!IsActive)
            return;
            
        // If it's a healing buff, it's already been applied and deactivated
        if (Type == BuffType.Heal)
            RemoveEffect();
            
        // Decrease the timer
        TimeLeft += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Check if the buff has expired
        if (TimeLeft >= DURATION)
        {
            RemoveEffect();
            
        }
    }
    

    public void Heal(int amount)
    {
        MainHero.Health += amount;
        if (MainHero.Health > MainHero.MaxHealth)
        {
            MainHero.Health = MainHero.MaxHealth;
        }
    }
    
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        if (!IsActive)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }
    }
    
    
}