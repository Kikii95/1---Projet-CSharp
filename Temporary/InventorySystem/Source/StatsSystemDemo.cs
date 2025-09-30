using System;

public static class StatsSystemDemo
{
    public static void RunCompleteDemo()
    {
        Console.WriteLine("=== DÉMONSTRATION SYSTÈME DE STATISTIQUES ===");
        Console.WriteLine("Appuyez sur Entrée pour continuer...\n");

        // === DEMO 1: CRÉATION DE SHIPS ===
        Console.WriteLine("📋 Demo 1: Création de Ships");
        DemoShipCreation();
        Console.ReadLine();

        // === DEMO 2: SYSTÈME DE TYPES ===
        Console.WriteLine("\n📋 Demo 2: Système de Types et Efficacité");
        DemoTypeEffectiveness();
        Console.ReadLine();

        // === DEMO 3: SYSTÈME DE STATS ===
        Console.WriteLine("\n📋 Demo 3: Statistiques et Modificateurs");
        DemoStatsAndModifiers();
        Console.ReadLine();

        // === DEMO 4: SYSTÈME DE NIVEAU ===
        Console.WriteLine("\n📋 Demo 4: Système de Niveau et XP");
        DemoLevelSystem();
        Console.ReadLine();

        // === DEMO 5: COMBAT COMPLET ===
        Console.WriteLine("\n📋 Demo 5: Simulation de Combat Complet");
        DemoCompleteCombat();

        Console.WriteLine("\n✅ Démonstration terminée !");
    }

    private static void DemoShipCreation()
    {
        Console.WriteLine("=== Création de Ships avec différents types ===");

        var playerStats = new Stats(120, 25, 12, 18);
        var player = new PlayerShip("Phoenix", ShipType.FEU, playerStats);

        var enemy1Stats = new Stats(80, 18, 8, 15);
        var enemy1 = new EnemyShip("Frost Drone", ShipType.GLACE, enemy1Stats, 50, 25);

        var enemy2Stats = new Stats(100, 20, 15, 10);
        var enemy2 = new EnemyShip("Rock Golem", ShipType.ROCHE, enemy2Stats, 75, 40);

        player.DisplayInfo();
        Console.WriteLine();
        enemy1.DisplayInfo();
        Console.WriteLine();
        enemy2.DisplayInfo();

        Console.WriteLine("\n✅ Ships créés avec succès");
    }

    private static void DemoTypeEffectiveness()
    {
        Console.WriteLine("=== Test des efficacités de types ===\n");

        // Test 1: Super efficace
        Console.WriteLine("Test 1: FEU vs GLACE");
        float mult1 = TypeEffectiveness.GetEffectiveness(AttackType.FEU, ShipType.GLACE);
        Console.WriteLine($"Multiplicateur: {mult1}x");
        Console.WriteLine(TypeEffectiveness.GetEffectivenessMessage(mult1));

        // Test 2: Peu efficace
        Console.WriteLine("\nTest 2: FEU vs EAU");
        float mult2 = TypeEffectiveness.GetEffectiveness(AttackType.FEU, ShipType.EAU);
        Console.WriteLine($"Multiplicateur: {mult2}x");
        Console.WriteLine(TypeEffectiveness.GetEffectivenessMessage(mult2));

        // Test 3: STAB
        Console.WriteLine("\nTest 3: FEU utilisé par un Ship FEU sur GLACE");
        float mult3 = TypeEffectiveness.GetTotalMultiplier(AttackType.FEU, ShipType.FEU, ShipType.GLACE);
        Console.WriteLine($"Multiplicateur total (avec STAB): {mult3}x");
        Console.WriteLine(TypeEffectiveness.GetEffectivenessMessage(mult3));

        // Test 4: Sans STAB
        Console.WriteLine("\nTest 4: FEU utilisé par un Ship NORMAL sur GLACE");
        float mult4 = TypeEffectiveness.GetTotalMultiplier(AttackType.FEU, ShipType.NORMAL, ShipType.GLACE);
        Console.WriteLine($"Multiplicateur total (sans STAB): {mult4}x");

        Console.WriteLine("\n✅ Système de types validé");
    }

    private static void DemoStatsAndModifiers()
    {
        Console.WriteLine("=== Test des modificateurs de stats ===\n");

        var stats = new Stats(100, 20, 10, 15);
        Console.WriteLine("Stats de base:");
        stats.DisplayStats();

        Console.WriteLine("\n--- Ajout d'un buff d'attaque temporaire ---");
        var attackBuff = new StatModifier("Rage", ModifiableStat.ATTACK, ModifierType.PERCENTAGE, 50, 3);
        stats.AddModifier(attackBuff);
        stats.DisplayStats();

        Console.WriteLine("\n--- Ajout d'un debuff de vitesse ---");
        var speedDebuff = new StatModifier("Slow", ModifiableStat.SPEED, ModifierType.FLAT, -5, 2);
        stats.AddModifier(speedDebuff);
        stats.DisplayStats();

        Console.WriteLine("\n--- Ajout d'un buff permanent de défense ---");
        var defenseBuffPerm = new StatModifier("Armor Plating", ModifiableStat.DEFENSE, ModifierType.FLAT, 5);
        stats.AddModifier(defenseBuffPerm);
        stats.DisplayStats();

        Console.WriteLine("\n--- Simulation de 3 tours ---");
        for (int i = 1; i <= 3; i++)
        {
            Console.WriteLine($"\nTour {i}:");
            stats.UpdateModifiers();
            stats.DisplayStats();
        }

        Console.WriteLine("\n✅ Modificateurs testés avec succès");
    }

    private static void DemoLevelSystem()
    {
        Console.WriteLine("=== Test du système de niveau ===\n");

        var stats = new Stats(100, 20, 10, 15);
        var player = new PlayerShip("Apprentice", ShipType.NORMAL, stats);

        player.DisplayInfo();

        Console.WriteLine("\n--- Gain de 150 XP ---");
        player.GainXP(150);
        player.DisplayInfo();

        Console.WriteLine("\n--- Gain de 200 XP supplémentaires ---");
        player.GainXP(200);
        player.DisplayInfo();

        Console.WriteLine("\n--- Gain de 500 XP (plusieurs niveaux) ---");
        player.GainXP(500);
        player.DisplayInfo();

        Console.WriteLine("\n✅ Système de niveau validé");
    }

    private static void DemoCompleteCombat()
    {
        Console.WriteLine("=== Simulation de Combat Complet ===\n");

        var playerStats = new Stats(150, 30, 15, 20);
        var player = new PlayerShip("Blazer", ShipType.FEU, playerStats);

        var enemyStats = new Stats(100, 20, 10, 15);
        var enemy = new EnemyShip("Ice Titan", ShipType.GLACE, enemyStats, 200, 100);

        Console.WriteLine("--- COMBATTANTS ---");
        player.DisplayInfo();
        Console.WriteLine();
        enemy.DisplayInfo();

        Console.WriteLine("\n\n=== DÉBUT DU COMBAT ===\n");

        int turn = 1;

        while (player.IsAlive && enemy.IsAlive)
        {
            Console.WriteLine($"\n{'='} TOUR {turn} {'='}");

            Console.WriteLine("\n[TOUR JOUEUR]");

            if (turn == 1)
            {
                Console.WriteLine("Le joueur active 'Rage' !");
                var rage = new StatModifier("Rage", ModifiableStat.ATTACK, ModifierType.PERCENTAGE, 50, 3);
                player.AddStatModifier(rage);
            }

            Console.WriteLine($"\n{player.Name} attaque !");
            if (player.RollHitCheck(enemy))
            {
                int damage = player.GetEffectiveAttack();
                
                if (player.RollCriticalCheck())
                {
                    damage = player.ApplyCriticalDamage(damage);
                }

                enemy.TakeDamage(damage, AttackType.FEU, ShipType.FEU);
            }

            if (!enemy.IsAlive)
            {
                break;
            }

            Console.WriteLine("\n[TOUR ENNEMI]");
            Console.WriteLine($"\n{enemy.Name} attaque !");
            if (enemy.RollHitCheck(player))
            {
                int damage = enemy.GetEffectiveAttack();
                
                if (enemy.RollCriticalCheck())
                {
                    damage = enemy.ApplyCriticalDamage(damage);
                }

                player.TakeDamage(damage, AttackType.GLACE, ShipType.GLACE);
            }

            player.Update();
            enemy.Update();

            turn++;

            if (turn > 10)
            {
                Console.WriteLine("\n⏰ Combat trop long, arrêt forcé");
                break;
            }
        }

        Console.WriteLine("\n\n=== FIN DU COMBAT ===");

        if (player.IsAlive && !enemy.IsAlive)
        {
            Console.WriteLine($"\n🏆 VICTOIRE ! {player.Name} remporte le combat !");
            Console.WriteLine($"\nRécompenses:");
            player.GainXP(enemy.XPReward);
            player.GainGold(enemy.GoldReward);
        }
        else if (!player.IsAlive)
        {
            Console.WriteLine($"\n💀 DÉFAITE ! {player.Name} a été vaincu...");
        }
        else
        {
            Console.WriteLine("\n⚔️ Match nul !");
        }

        Console.WriteLine("\n--- ÉTAT FINAL ---");
        player.DisplayInfo();
        Console.WriteLine();
        enemy.DisplayInfo();

        Console.WriteLine("\n✅ Combat simulé avec succès");
    }
}
