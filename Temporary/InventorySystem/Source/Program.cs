using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🎮 === SYSTÈME COMPLET ===");

        try
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("MENU PRINCIPAL");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine("1. Démonstration Système d'Inventaire");
                Console.WriteLine("2. Démonstration Système de Statistiques");
                Console.WriteLine("3. Démonstration Complète (Inventaire + Stats)");
                Console.WriteLine("4. Quitter");
                Console.WriteLine(new string('=', 60));
                Console.Write("Votre choix: ");

                string choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        RunInventoryDemo();
                        break;
                    case "2":
                        RunStatsDemo();
                        break;
                    case "3":
                        RunCompleteDemo();
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("❌ Choix invalide");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\n\nAppuyez sur Entrée pour revenir au menu...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }

            DisplayIntegrationInfo();

            Console.WriteLine("\n✅ Programme terminé. Appuyez sur Entrée pour quitter...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur: {ex.Message}");
            Console.WriteLine("Vérifiez que tous les fichiers source sont présents.");
            Console.ReadLine();
        }
    }

    static void RunInventoryDemo()
    {
        Console.WriteLine("🎒 === DÉMONSTRATION SYSTÈME INVENTAIRE ===\n");
        InventorySystemTests.RunInteractiveDemo();
    }

    static void RunStatsDemo()
    {
        Console.WriteLine("📊 === DÉMONSTRATION SYSTÈME STATISTIQUES ===\n");
        StatsSystemDemo.RunCompleteDemo();
    }

    static void RunCompleteDemo()
    {
        Console.WriteLine("🎮 === DÉMONSTRATION COMPLÈTE DU JEU ===\n");
        Console.WriteLine("Cette démo combine inventaire + statistiques + combat\n");
        Console.WriteLine("Appuyez sur Entrée pour commencer...");
        Console.ReadLine();

        var playerStats = new Stats(150, 30, 15, 20);
        var player = new PlayerShip("Hero", ShipType.FEU, playerStats);
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        Console.WriteLine("\n=== PHASE 1: PRÉPARATION ===");
        player.DisplayInfo();

        Console.WriteLine("\n--- Récupération d'items ---");
        var dashCapacity = new Capacity("Turbo Dash", "Déplacement rapide", CapacityType.DASH, 3);
        var speedBoost = new TemporaryPowerUp("Nitro", "Vitesse x2", PowerUpStat.SPEED, 10, 5);
        var attackUpgrade = new PermanentPowerUp("Weapon Upgrade", "Attaque +5", PowerUpStat.ATTACK, 5);

        inventory.AddItem(dashCapacity);
        inventory.AddItem(speedBoost, 2);
        inventory.AddItem(attackUpgrade);
        inventory.DisplayInventory();

        Console.WriteLine("\n--- Utilisation du PowerUp Permanent ---");
        PowerUpHelper.ApplyPowerUpToShip(player, attackUpgrade, inventory);
        player.DisplayInfo();

        Console.WriteLine("\n\nAppuyez sur Entrée pour continuer...");
        Console.ReadLine();

        Console.WriteLine("\n=== PHASE 2: COMBAT ===");
        var enemyStats = new Stats(100, 25, 10, 15);
        var enemy = new EnemyShip("Ice Golem", ShipType.GLACE, enemyStats, 150, 75);
        enemy.DisplayInfo();

        Console.WriteLine("\n--- Début du combat ---");
        Console.WriteLine("Le joueur utilise le Nitro Boost !");
        PowerUpHelper.ApplyPowerUpToShip(player, speedBoost, inventory);

        int turn = 1;
        while (player.IsAlive && enemy.IsAlive && turn <= 5)
        {
            Console.WriteLine($"\n--- Tour {turn} ---");

            if (player.RollHitCheck(enemy))
            {
                int damage = player.GetEffectiveAttack();
                if (player.RollCriticalCheck())
                {
                    damage = player.ApplyCriticalDamage(damage);
                }
                enemy.TakeDamage(damage, AttackType.FEU, ShipType.FEU);
            }

            if (!enemy.IsAlive) break;

            if (enemy.RollHitCheck(player))
            {
                int damage = enemy.GetEffectiveAttack();
                player.TakeDamage(damage, AttackType.GLACE, ShipType.GLACE);
            }

            player.Update();
            enemy.Update();
            inventory.UpdateTemporaryEffects();

            turn++;
        }

        Console.WriteLine("\n=== RÉSULTAT DU COMBAT ===");
        if (player.IsAlive && !enemy.IsAlive)
        {
            Console.WriteLine("🏆 VICTOIRE !");
            player.GainXP(enemy.XPReward);
            player.GainGold(enemy.GoldReward);
        }
        else if (!player.IsAlive)
        {
            Console.WriteLine("💀 DÉFAITE...");
        }

        Console.WriteLine("\n=== ÉTAT FINAL ===");
        player.DisplayInfo();
        inventory.DisplayInventory();

        Console.WriteLine("\n✅ Démonstration complète terminée !");
    }

    static void DisplayIntegrationInfo()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🔗 POINTS D'INTÉGRATION");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("Le système de stats est prêt pour l'intégration avec:");
        Console.WriteLine("\n📦 GameObject (IAttack, IMove, ICollide):");
        Console.WriteLine("  - Ship hérite de GameObject");
        Console.WriteLine("  - IAttack peut appeler:");
        Console.WriteLine("    • ship.TakeDamage(damage, attackType, attackerType)");
        Console.WriteLine("    • ship.GetEffectiveAttack()");
        Console.WriteLine("    • ship.RollHitCheck(target)");
        Console.WriteLine("    • ship.RollCriticalCheck()");
        Console.WriteLine("\n🎯 Méthodes disponibles pour IAttack:");
        Console.WriteLine("  - Calcul automatique des dégâts avec types");
        Console.WriteLine("  - Gestion de la précision et esquive");
        Console.WriteLine("  - Système de coups critiques");
        Console.WriteLine("  - Application du STAB (Same Type Attack Bonus)");
        Console.WriteLine("\n📋 Classes disponibles:");
        Console.WriteLine("  - IHealth : Interface pour la santé");
        Console.WriteLine("  - Stats : Gestion complète des statistiques");
        Console.WriteLine("  - StatModifier : Buffs/Debuffs temporaires et permanents");
        Console.WriteLine("  - Ship : Classe abstraite pour les vaisseaux");
        Console.WriteLine("  - PlayerShip / EnemyShip : Implémentations concrètes");
        Console.WriteLine("  - ShipType / AttackType : Enums pour les types");
        Console.WriteLine("  - TypeEffectiveness : Calcul des efficacités");
        Console.WriteLine(new string('=', 60));
    }
}
