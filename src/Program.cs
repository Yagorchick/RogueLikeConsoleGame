using System;
using System.Collections.Generic;

public class Player
{
    public string Name { get; private set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; private set; }
    public Weapon Weapon { get; private set; }
    public Aid Aid { get; private set; }
    public int Score { get; set; }
    private int turnsWithoutAid;

    public Player(string name)
    {
        Name = name;
        MaxHealth = 100;
        CurrentHealth = MaxHealth;
        Score = 0;
        turnsWithoutAid = 0;
    }

    public void GenerateWeapon()
    {
        Random rnd = new Random();
        List<Weapon> weapons = new List<Weapon>
        {
            new Weapon("Фламберг", 20, 100),
            new Weapon("Острый Язык", 15, 80),
            new Weapon("Боевой топор", 25, 90)
        };

        Weapon = weapons[rnd.Next(weapons.Count)];
        GenerateAid();
        Console.WriteLine($"Вам был ниспослан - {Weapon.Name} ({Weapon.Damage} Урона), а также - {Aid.Name} ({Aid.HealthRestore} Восстановления Здоровья).");
    }

    public void GenerateAid()
    {
        Random rnd = new Random();
        List<Aid> aids = new List<Aid>
        {
            new Aid("Малая аптечка", 25),
            new Aid("Средняя аптечка", 40),
            new Aid("Большая аптечка", 65)
        };

        Aid = aids[rnd.Next(aids.Count)];
        Console.WriteLine($"\nУ вас {CurrentHealth} Здоровья.");
    }

    public void Attack(Enemy enemy)
    {
        Console.WriteLine($"\n{Name} ударил противника - {enemy.Name}");
        enemy.CurrentHealth -= Weapon.Damage;
    }

    public void UseAid()
    {
        if (Aid != null)
        {
            CurrentHealth += Aid.HealthRestore;
            if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
            Console.WriteLine($"\n{Name} использовал аптечку. У вас {CurrentHealth} Здоровья.");
            Aid = null;
            turnsWithoutAid = 0;
        }
        else
        {
            Console.WriteLine("\nУ вас нет аптечек!");
        }
    }

    public void CheckForNewAid()
    {
        turnsWithoutAid++;
        if (turnsWithoutAid >= 3)
        {
            Aid newAid = GenerateTemporaryAid();
            if (Aid == null)
            {
                Aid = newAid;
                Console.WriteLine($"\n{Name} нашел новую аптечку: {Aid.Name} ({Aid.HealthRestore} Восстановления Здоровья)!");
            }
            else
            {
                Console.WriteLine($"\nИз Врага выпал Предмет - {newAid.Name}. У вас в Инвентаре уже есть Аптечка: {Aid.Name}.");
            }
            turnsWithoutAid = 0;
        }
    }

    private Aid GenerateTemporaryAid()
    {
        Random rnd = new Random();
        List<Aid> aids = new List<Aid>
        {
            new Aid("Малая аптечка", 25),
            new Aid("Средняя аптечка", 40),
            new Aid("Большая аптечка", 65)
        };

        return aids[rnd.Next(aids.Count)];
    }
}


public class Enemy
{
    public string Name { get; private set; }
    public int CurrentHealth { get; set; }
    public Weapon Weapon { get; private set; }

    public Enemy(string name, int health, Weapon weapon)
    {
        Name = name;
        CurrentHealth = health;
        Weapon = weapon;
    }

    public void Attack(Player player)
    {
        Console.WriteLine($"\n{Name} ударил вас!");
        player.CurrentHealth -= Weapon.Damage;
    }
}

public class Aid
{
    public string Name { get; private set; }
    public int HealthRestore { get; private set; }

    public Aid(string name, int healthRestore)
    {
        Name = name;
        HealthRestore = healthRestore;
    }
}

public class Weapon
{
    public string Name { get; private set; }
    public int Damage { get; private set; }
    public int Durability { get; private set; }

    public Weapon(string name, int damage, int durability)
    {
        Name = name;
        Damage = damage;
        Durability = durability;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Добро пожаловать на Арену Сражений!");
        Console.Write("Кто ты, Воин?: ");
        string playerName = Console.ReadLine();
        Console.WriteLine($"\nНа Арену выходит - {playerName}!");

        Player player = new Player(playerName);
        player.GenerateWeapon();

        Random rnd = new Random();
        List<Enemy> enemies = GenerateEnemies(rnd);

        foreach (var enemy in enemies)
        {
            Console.WriteLine($"\nВы встречаете врага - {enemy.Name} ({enemy.CurrentHealth} Здоровья), у врага на поясе сияет оружие - {enemy.Weapon.Name} ({enemy.Weapon.Damage})");
            while (enemy.CurrentHealth > 0 && player.CurrentHealth > 0)
            {
                Console.WriteLine("\nЧто вы будете делать?");
                Console.WriteLine("1. Ударить");
                Console.WriteLine("2. Пропустить ход");
                Console.WriteLine("3. Использовать аптечку");
                Console.Write("\nВаш ход: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        player.Attack(enemy);
                        if (enemy.CurrentHealth > 0)
                        {
                            enemy.Attack(player);
                        }
                        break;
                    case "2":
                        Console.WriteLine($"\n{player.Name} пропустил ход.");
                        enemy.Attack(player);
                        break;
                    case "3":
                        player.UseAid();
                        break;
                    default:
                        Console.WriteLine("\nНеверный выбор!");
                        break;
                }

                player.CheckForNewAid();

                Console.WriteLine($"\nУ противника {enemy.CurrentHealth} Здоровья, у вас {player.CurrentHealth} Здоровья");
            }

            if (player.CurrentHealth <= 0)
            {
                Console.WriteLine("\nВы погибли! Игра окончена.");
                return;
            }
            else
            {
                player.Score++;
                Console.WriteLine($"\nВы победили {enemy.Name}! Ваши счёт: {player.Score}");
            }
        }

        Console.WriteLine("\nВы победили всех врагов! Поздравляем!");
    }

    static List<Enemy> GenerateEnemies(Random rnd)
    {
        List<Enemy> enemies = new List<Enemy>
        {
            new Enemy("Скелет в Доспехах", rnd.Next(30, 70), new Weapon("Меч", 10, 100)),
            new Enemy("Павший Ассасин", rnd.Next(20, 50), new Weapon("Кинжал", 5, 50)),
            new Enemy("Древний Варвар", rnd.Next(40, 90), new Weapon("Топор", 15, 80)),
            new Enemy("Палладин", rnd.Next(25, 60), new Weapon("Экскалибур", 8, 60)),
            new Enemy("Виверна", rnd.Next(50, 100), new Weapon("Когти", 12, 70))
        };

        return enemies;
    }
}
