using System;
using Tales_of_Everlight.Characters;
using static Tales_of_Everlight.Main;
using MainHero = Tales_of_Everlight.Characters.MainHero;

namespace Tales_of_Everlight.Damage;

public enum AttackerType
{
    Player,
    Enemy
}

public static class Attack
{
    public static int Damage { get; set; }
    public static AttackerType AttackerType { get; set; }


    public static void Execute(AttackerType attackerType)
    {
        if (AttackerType == AttackerType.Player)
        {
            foreach (var enemy in EnemyList)
            {
                if(!enemy.IsDead && Math.Abs(enemy.Rect.X - Main.MainHero.Rect.X) < 150 && Math.Abs(enemy.Rect.Y - Main.MainHero.Rect.Y) < 150)
                {
                    enemy.TakeDamage(MainHero.Damage);
                    enemy.Damaged= true;
                    
                }
            }
        }
    }

    public static void ExecuteByEnemy(int damage)
    {
        Main.MainHero.TakeDamage(damage);
    }

}