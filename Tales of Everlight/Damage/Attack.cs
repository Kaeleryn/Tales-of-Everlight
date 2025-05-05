using System;
using static Tales_of_Everlight.Main;

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
                    enemy.TakeDamage(20);
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

    public static void ExecuteByEnemy(int damage)
    {
        Main.MainHero.TakeDamage(damage);
    }

}