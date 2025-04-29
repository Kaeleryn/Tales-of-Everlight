using System;
using Microsoft.Xna.Framework;

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
            foreach (var enemy in Main.EnemyList)
            {
                if(!enemy.IsDead && Math.Abs(enemy.Rect.X - Main._mainHero.Rect.X) < 150 && Math.Abs(enemy.Rect.Y - Main._mainHero.Rect.Y) < 150)
                {
                    enemy.TakeDamage(10);
                    enemy.Damaged= true;
                    Console.WriteLine("Execute method called");
                    // if (enemy.Health <= 0)
                    // {
                    //     Main.EnemyList.Remove(enemy);
                    // }
                }
            }
        }
    }

    public static void ExecuteByEnemy()
    {
        Console.WriteLine("Execute method called by enemy");
    }

}