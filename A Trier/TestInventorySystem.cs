using System;

/// <summary>
/// Programme de test pour valider le système d'inventaire
/// Test des fonctionnalités selon les cours POO
/// </summary>
class TestInventorySystem
{
    static void Main(string[] args)
    {
        Console.WriteLine("🚀 === TEST SYSTÈME INVENTAIRE ===");
        Console.WriteLine("Appuyez sur Entrée pour continuer entre chaque test...\n");

        // === TEST 1: SINGLETON PATTERN ===
        Console.WriteLine("📋 Test 1: Pattern Singleton");
        TestSingleton();
        Console.ReadLine();

        // === TEST 2: CRÉATION D'ITEMS ===
        Console.WriteLine("\n📋 Test 2: Création Items");
        TestItemCreation();
        Console.ReadLine();

        // === TEST 3: GESTION INVENTAIRE ===
        Console.WriteLine("\n📋 Test 3: Gestion Inventaire");
        TestInventoryManagement();
        Console.ReadLine();

        // === TEST 4: UTILISATION CAPACITÉS ===
        Console.WriteLine("\n📋 Test 4: Utilisation Capacités");
        TestCapacityUsage();
        Console.ReadLine();

        // === TEST 5: POWERUPS TEMPORAIRES ===
        Console.WriteLine("\n📋 Test 5: PowerUps Temporaires");
        TestTemporaryPowerUps();
        Console.ReadLine();

        // === TEST 6: POWERUPS PERMANENTS ===
        Console.WriteLine("\n📋 Test 6: PowerUps Permanents");
        TestPermanentPowerUps();
        Console.ReadLine();

        // === TEST 7: SIMULATION JEU ===
        Console.WriteLine("\n📋 Test 7: Simulation Tour de Jeu");
        TestGameTurnSimulation();

        Console.WriteLine("\n✅ Tests terminés ! Appuyez sur Entrée pour quitter.");
        Console.ReadLine();
    }

    /// <summary>
    /// Test du pattern Singleton
    /// </summary>
    static void TestSingleton()
    {
        var inventory1 = Inventory.Instance;
        var inventory2 = Inventory.Instance;

        Console.WriteLine($"Instance 1: {inventory1.GetHashCode()}");
        Console.WriteLine($"Instance 2: {inventory2.GetHashCode()}");
        Console.WriteLine($"Même instance: {inventory1 == inventory2}");
    }

    /// <summary>
    /// Test de création des différents types d'items
    /// </summary>
    static void TestItemCreation()
    {
        // Capacités
        var dash = new Capacity("Turbo Dash", "Déplacement ultra-rapide", CapacityType.DASH, 3);
        var shield = new Capacity("Force Shield", "Bouclier de force", CapacityType.SHIELD, 5);

        // PowerUps temporaires
        var speedBoost = new TemporaryPowerUp("Nitro Boost", "Vitesse x3", PowerUpStat.SPEED, 10, 8);
        var attackBoost = new TemporaryPowerUp("Rage Mode", "Attaque x2", PowerUpStat.ATTACK, 15, 6);

        // PowerUps permanents
        var hpUpgrade = new PermanentPowerUp("Armor Plating", "PV Max +25", PowerUpStat.MAX_HP, 25);
        var defenseUpgrade = new PermanentPowerUp("Reinforced Hull", "Défense +8", PowerUpStat.DEFENSE, 8);

        Console.WriteLine("Items créés:");
        Console.WriteLine($"  {dash.GetDisplayInfo()}");
        Console.WriteLine($"  {shield.GetDisplayInfo()}");
        Console.WriteLine($"  {speedBoost.GetDisplayInfo()}");
        Console.WriteLine($"  {attackBoost.GetDisplayInfo()}");
        Console.WriteLine($"  {hpUpgrade.GetDisplayInfo()}");
        Console.WriteLine($"  {defenseUpgrade.GetDisplayInfo()}");
    }

    /// <summary>
    /// Test des fonctionnalités de base de l'inventaire
    /// </summary>
    static void TestInventoryManagement()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory(); // Reset pour test propre

        // Créer des items
        var dash = new Capacity("Test Dash", "Dash de test", CapacityType.DASH, 2);
        var speedBoost = new TemporaryPowerUp("Test Speed", "Speed de test", PowerUpStat.SPEED, 5, 3);
        var hpUpgrade = new PermanentPowerUp("Test HP", "HP de test", PowerUpStat.MAX_HP, 10);

        // Ajouter items
        inventory.AddItem(dash);
        inventory.AddItem(speedBoost, 3);
        inventory.AddItem(hpUpgrade, 2);

        // Afficher inventaire
        inventory.DisplayInventory();

        // Tester quantités
        Console.WriteLine($"\nTest Speed quantity: {inventory.GetItemQuantity(speedBoost)}");
        Console.WriteLine($"Has Test Dash: {inventory.HasItem(dash)}");

        // Retirer items
        inventory.RemoveItem(speedBoost, 1);
        Console.WriteLine($"Test Speed après retrait: {inventory.GetItemQuantity(speedBoost)}");
    }

    /// <summary>
    /// Test d'utilisation des capacités avec cooldown
    /// </summary>
    static void TestCapacityUsage()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        var missiles = new Capacity("Plasma Missiles", "Missiles plasma", CapacityType.MISSILES, 4);
        inventory.AddItem(missiles);

        Console.WriteLine("=== Test Capacity Cooldown ===");
        
        // Première utilisation (devrait marcher)
        Console.WriteLine("Première utilisation:");
        inventory.UseItem(missiles);

        // Deuxième utilisation (devrait échouer - cooldown)
        Console.WriteLine("\nDeuxième utilisation immédiate:");
        inventory.UseItem(missiles);

        // Simuler passage de temps
        Console.WriteLine("\nSimulation de 2 tours:");
        missiles.ReduceCooldown();
        missiles.ReduceCooldown();
        Console.WriteLine($"Cooldown restant: {missiles.CurrentCooldown}");

        Console.WriteLine("\nTroisième utilisation (encore en cooldown):");
        inventory.UseItem(missiles);

        // Finir le cooldown
        Console.WriteLine("\nSimulation de 2 tours supplémentaires:");
        missiles.ReduceCooldown();
        missiles.ReduceCooldown();
        Console.WriteLine($"Cooldown restant: {missiles.CurrentCooldown}");

        Console.WriteLine("\nQuatrième utilisation (devrait marcher):");
        inventory.UseItem(missiles);
    }

    /// <summary>
    /// Test des PowerUps temporaires
    /// </summary>
    static void TestTemporaryPowerUps()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        var speedBoost = new TemporaryPowerUp("Temporal Speed", "Vitesse temporaire", PowerUpStat.SPEED, 8, 5);
        inventory.AddItem(speedBoost, 2);

        Console.WriteLine("=== Test PowerUp Temporaire ===");
        inventory.DisplayInventory();

        // Première utilisation
        Console.WriteLine("\nPremière utilisation:");
        inventory.UseItem(speedBoost);
        Console.WriteLine($"Effet actif: {speedBoost.IsActive}, Durée: {speedBoost.RemainingDuration}");

        // Tentative d'utilisation pendant effet actif
        Console.WriteLine("\nTentative utilisation pendant effet actif:");
        inventory.UseItem(speedBoost);

        // Simuler passage de temps
        Console.WriteLine("\nSimulation de 3 tours:");
        for (int i = 0; i < 3; i++)
        {
            inventory.UpdateTemporaryEffects();
            Console.WriteLine($"Tour {i + 1}: Durée restante = {speedBoost.RemainingDuration}");
        }

        Console.WriteLine("\nSimulation de 3 tours supplémentaires:");
        for (int i = 0; i < 3; i++)
        {
            inventory.UpdateTemporaryEffects();
            Console.WriteLine($"Tour {i + 4}: Durée restante = {speedBoost.RemainingDuration}");
        }

        Console.WriteLine($"\nEffet terminé: {!speedBoost.IsActive}");
    }

    /// <summary>
    /// Test des PowerUps permanents
    /// </summary>
    static void TestPermanentPowerUps()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        var attackUpgrade = new PermanentPowerUp("Weapon Upgrade", "Amélioration d'arme", PowerUpStat.ATTACK, 12);
        inventory.AddItem(attackUpgrade, 2);

        Console.WriteLine("=== Test PowerUp Permanent ===");
        inventory.DisplayInventory();

        // Première utilisation (consomme l'item)
        Console.WriteLine("\nPremière utilisation:");
        Console.WriteLine($"Quantité avant: {inventory.GetItemQuantity(attackUpgrade)}");
        inventory.UseItem(attackUpgrade);
        Console.WriteLine($"Quantité après: {inventory.GetItemQuantity(attackUpgrade)}");
        Console.WriteLine($"Item utilisé: {attackUpgrade.IsUsed}");

        // Tentative d'utilisation d'un item déjà utilisé
        Console.WriteLine("\nTentative utilisation item déjà utilisé:");
        inventory.UseItem(attackUpgrade);

        inventory.DisplayInventory();
    }

    /// <summary>
    /// Simulation complète d'un tour de jeu
    /// </summary>
    static void TestGameTurnSimulation()
    {
        var inventory = Inventory.Instance;
        inventory.ClearInventory();

        Console.WriteLine("=== Simulation Tour de Jeu ===");
        
        // Ajouter items de debug
        inventory.AddDebugItems();
        inventory.DisplayInventory();

        // Simuler quelques actions de jeu
        Console.WriteLine("\n--- Actions Joueur ---");
        
        var items = inventory.GetAllItems().Keys.ToArray();
        
        // Utiliser une capacité
        var dashItem = items.FirstOrDefault(i => i is Capacity && ((Capacity)i).Type == CapacityType.DASH);
        if (dashItem != null)
        {
            Console.WriteLine("Action: Utiliser Dash");
            inventory.UseItem(dashItem);
        }

        // Utiliser un PowerUp temporaire
        var speedItem = items.FirstOrDefault(i => i is TemporaryPowerUp && ((TemporaryPowerUp)i).Stat == PowerUpStat.SPEED);
        if (speedItem != null)
        {
            Console.WriteLine("\nAction: Utiliser Speed Boost");
            inventory.UseItem(speedItem);
        }

        // Utiliser un PowerUp permanent
        var hpItem = items.FirstOrDefault(i => i is PermanentPowerUp && ((PermanentPowerUp)i).Stat == PowerUpStat.MAX_HP);
        if (hpItem != null)
        {
            Console.WriteLine("\nAction: Utiliser HP Upgrade");
            inventory.UseItem(hpItem);
        }

        // Simuler plusieurs tours
        Console.WriteLine("\n--- Simulation 6 Tours ---");
        for (int turn = 1; turn <= 6; turn++)
        {
            Console.WriteLine($"\n🎮 Tour {turn}:");
            inventory.UpdateTemporaryEffects();
            inventory.DisplaySummary();
        }

        Console.WriteLine("\n--- État Final ---");
        inventory.DisplayInventory();
    }
}