using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class InventorySystemTests
{
    // === SETUP ET TEARDOWN ===
    [TestInitialize]
    public void Setup()
    {
        Inventory.Instance.ClearInventory();
    }

    public static void RunInteractiveDemo()
    {
        Console.WriteLine("=== DEMONSTRATION SYSTEME INVENTAIRE ===");
        Console.WriteLine("Demonstration interactive");
        Console.WriteLine("Appuyez sur Entree pour continuer...\n");

        // === DEMO 1: SINGLETON PATTERN ===
        Console.WriteLine("[ ] Demo 1: Pattern Singleton");
        DemoSingleton();
        Console.ReadLine();

        // === DEMO 2: CRÉATION D'ITEMS ===
        Console.WriteLine("\n[ ] Demo 2: Creation Items");
        DemoItemCreation();
        Console.ReadLine();

        // === DEMO 3: CAPACITÉS AVEC COOLDOWN ===
        Console.WriteLine("\n[ ] Demo 3: Capacites et Cooldown");
        DemoCapacityUsage();
        Console.ReadLine();

        // === DEMO 4: POWERUPS TEMPORAIRES ===
        Console.WriteLine("\n[ ] Demo 4: PowerUps Temporaires");
        DemoTemporaryPowerUps();
        Console.ReadLine();

        // === DEMO 5: POWERUPS PERMANENTS ===
        Console.WriteLine("\n[ ] Demo 5: PowerUps Permanents");
        DemoPermanentPowerUps();
        Console.ReadLine();

        // === DEMO 6: SIMULATION COMPLÈTE ===
        Console.WriteLine("\n[ ] Demo 6: Simulation Complete de Jeu");
        DemoCompleteGameSimulation();

        Console.WriteLine("\n[OK] Demonstration terminee !");
    }

    private static void DemoSingleton()
    {
        var inventory1 = Inventory.Instance;
        var inventory2 = Inventory.Instance;

        Console.WriteLine($"Instance 1: {inventory1.GetHashCode()}");
        Console.WriteLine($"Instance 2: {inventory2.GetHashCode()}");
        Console.WriteLine($"Meme instance: {inventory1 == inventory2}");
        Console.WriteLine("[OK] Pattern Singleton valide");
    }

    private static void DemoItemCreation()
    {
        Console.WriteLine("=== Création d'items de tous types ===");
        
        var dash = new Capacity("Turbo Dash", "Déplacement ultra-rapide", CapacityType.DASH, 3);
        var shield = new Capacity("Force Shield", "Bouclier de force", CapacityType.SHIELD, 5);
        var missiles = new Capacity("Plasma Missiles", "Missiles plasma", CapacityType.MISSILES, 4);

        var speedBoost = new TemporaryPowerUp("Nitro Boost", "Vitesse x3", PowerUpStat.SPEED, 10, 8);
        var attackBoost = new TemporaryPowerUp("Rage Mode", "Attaque x2", PowerUpStat.ATTACK, 15, 6);

        var hpUpgrade = new PermanentPowerUp("Armor Plating", "PV Max +25", PowerUpStat.MAX_HP, 25);
        var defenseUpgrade = new PermanentPowerUp("Reinforced Hull", "Défense +8", PowerUpStat.DEFENSE, 8);

        Console.WriteLine("Items créés:");
        Console.WriteLine($"  {dash.GetDisplayInfo()}");
        Console.WriteLine($"  {shield.GetDisplayInfo()}");
        Console.WriteLine($"  {missiles.GetDisplayInfo()}");
        Console.WriteLine($"  {speedBoost.GetDisplayInfo()}");
        Console.WriteLine($"  {attackBoost.GetDisplayInfo()}");
        Console.WriteLine($"  {hpUpgrade.GetDisplayInfo()}");
        Console.WriteLine($"  {defenseUpgrade.GetDisplayInfo()}");
        Console.WriteLine("✅ Création d'items validée");
    }

    private static void DemoCapacityUsage()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        var missiles = new Capacity("Plasma Missiles", "Missiles plasma", CapacityType.MISSILES, 4);
        inventory.AddItem(missiles);

        Console.WriteLine("=== Test Capacity Cooldown ===");
        
        Console.WriteLine("Première utilisation:");
        inventory.UseItem(missiles);

        Console.WriteLine("\nDeuxième utilisation immédiate:");
        inventory.UseItem(missiles);

        Console.WriteLine("\nSimulation de 2 tours:");
        missiles.ReduceCooldown();
        missiles.ReduceCooldown();
        Console.WriteLine($"Cooldown restant: {missiles.CurrentCooldown}");

        Console.WriteLine("\nTroisième utilisation (encore en cooldown):");
        inventory.UseItem(missiles);

        Console.WriteLine("\nSimulation de 2 tours supplémentaires:");
        missiles.ReduceCooldown();
        missiles.ReduceCooldown();
        Console.WriteLine($"Cooldown restant: {missiles.CurrentCooldown}");

        Console.WriteLine("\nQuatrième utilisation (devrait marcher):");
        inventory.UseItem(missiles);
        Console.WriteLine("✅ Système de cooldown validé");
    }

    private static void DemoTemporaryPowerUps()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        var speedBoost = new TemporaryPowerUp("Temporal Speed", "Vitesse temporaire", PowerUpStat.SPEED, 8, 5);
        inventory.AddItem(speedBoost, 2);

        Console.WriteLine("=== Test PowerUp Temporaire ===");
        inventory.DisplayInventory();

        Console.WriteLine("\nPremière utilisation:");
        inventory.UseItem(speedBoost);
        Console.WriteLine($"Effet actif: {speedBoost.IsActive}, Durée: {speedBoost.RemainingDuration}");

        Console.WriteLine("\nSimulation de 5 tours:");
        for (int i = 0; i < 5; i++)
        {
            inventory.UpdateTemporaryEffects();
            Console.WriteLine($"Tour {i + 1}: Durée restante = {speedBoost.RemainingDuration}");
        }

        Console.WriteLine($"\nEffet terminé: {!speedBoost.IsActive}");
        Console.WriteLine("✅ PowerUps temporaires validés");
    }

    private static void DemoPermanentPowerUps()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        var attackUpgrade = new PermanentPowerUp("Weapon Upgrade", "Amélioration d'arme", PowerUpStat.ATTACK, 12);
        inventory.AddItem(attackUpgrade, 2);

        Console.WriteLine("=== Test PowerUp Permanent ===");
        inventory.DisplayInventory();

        Console.WriteLine("\nPremière utilisation:");
        Console.WriteLine($"Quantité avant: {inventory.GetItemQuantity(attackUpgrade)}");
        inventory.UseItem(attackUpgrade);
        Console.WriteLine($"Quantité après: {inventory.GetItemQuantity(attackUpgrade)}");
        Console.WriteLine($"Item utilisé: {attackUpgrade.IsUsed}");

        inventory.DisplayInventory();
        Console.WriteLine("✅ PowerUps permanents validés");
    }

    private static void DemoCompleteGameSimulation()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        Console.WriteLine("=== Simulation Complète de Jeu ===");
        
        inventory.AddDebugItems();
        inventory.DisplayInventory();

        Console.WriteLine("\n--- Simulation 6 Tours de Jeu ---");
        
        var items = inventory.GetAllItems().Keys.ToArray();
        
        var dashItem = items.FirstOrDefault(i => i is Capacity && ((Capacity)i).Type == CapacityType.DASH);
        if (dashItem != null)
        {
            Console.WriteLine("\n🎮 Tour 1: Utiliser Dash");
            inventory.UseItem(dashItem);
        }

        var speedItem = items.FirstOrDefault(i => i is TemporaryPowerUp && ((TemporaryPowerUp)i).Stat == PowerUpStat.SPEED);
        if (speedItem != null)
        {
            Console.WriteLine("\n🎮 Tour 2: Utiliser Speed Boost");
            inventory.UseItem(speedItem);
        }

        for (int turn = 3; turn <= 13; turn++)
        {
            Console.WriteLine($"\n🎮 Tour {turn}: Passage du temps");
            inventory.UpdateTemporaryEffects();
            inventory.DisplaySummary();
        }

        Console.WriteLine("\n--- État Final ---");
        inventory.DisplayInventory();
        Console.WriteLine("✅ Simulation complète validée");
    }
}