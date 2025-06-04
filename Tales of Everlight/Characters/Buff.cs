using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight.Characters
{
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
        private const float DURATION = 15f;
        private float TimeLeft { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired { get; set; }
        private float Amount { get; set; }
        public bool IsOnMap { get; set; } = true;
        public Buff(BuffType type, Vector2 position, float amount, ContentManager content)
        {
            Type = type;
            Position = position;
            Amount = amount;
            IsActive = false;
            IsExpired = false;
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
            if (IsActive || IsExpired) return;

            Console.WriteLine($"Buff {Type} activated at {Position}");

            switch (Type)
            {
                case BuffType.Heal:
                    Heal((int)Amount);
                    IsExpired = true;
                    break;
                case BuffType.IncreaseDamage:
                    MainHero.Damage += (int)Amount;
                    IsActive = true;
                    TimeLeft = DURATION;
                    break;
                case BuffType.IncreaseSpeed:
                    MainHero.Speed += Amount;
                    IsActive = true;
                    TimeLeft = DURATION;
                    break;
            }
        }

        private void RemoveEffect()
        {
            if (!IsActive || IsExpired) return;

            Console.WriteLine($"Buff {Type} expired at {Position}");

            switch (Type)
            {
                case BuffType.IncreaseDamage:
                    MainHero.Damage -= (int)Amount;
                    break;
                case BuffType.IncreaseSpeed:
                    MainHero.Speed -= Amount;
                    break;
            }
            IsActive = false;
            IsExpired = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive || IsExpired) return;

            if (Type == BuffType.Heal)
            {
                RemoveEffect();
                return;
            }

            TimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeLeft <= 0)
            {
                RemoveEffect();
            }
        }

        private void Heal(int amount)
        {
            MainHero.Health += amount;
            if (MainHero.Health > MainHero.MaxHealth)
                MainHero.Health = MainHero.MaxHealth;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (!IsExpired && IsOnMap)
            {
                spriteBatch.Draw(Texture, position, Color.White);
            }
        }
    }
}