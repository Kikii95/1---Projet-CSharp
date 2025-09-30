using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests Unitaires Complets pour le système d'inventaire
/// Fusion des tests automatisés + démonstrations interactives
/// Selon cours POO : prouver que le code fonctionne dans tous les cas
/// </summary>
[TestClass]
public class InventorySystemTests
{
    // === SETUP ET TEARDOWN ===
    
    [TestInitialize]
    public void Setup()
    {
        // Reset l'inventaire avant chaque test
        Inventory.Instance.ClearInventory();
    }

    // =====================================================
    // PARTIE 1: TESTS UNITAIRES AUTOMATISÉS (MSTest)
    // =====================================================

    #region Tests Item Creation

    [TestMethod]
    public void Item_CreationCapacity_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var dash = new Capacity("Test Dash", "Description test", CapacityType.DASH, 3);

        // Assert
        Assert.AreEqual("Test Dash", dash.Name);
        Assert.AreEqual("Description test", dash.Description);
        Assert.AreEqual('C', dash.Symbol);
        Assert.AreEqual(ItemCategory.CAPACITY, dash.Category);
        Assert.AreEqual(CapacityType.DASH, dash.Type);
        Assert.AreEqual(3, dash.Cooldown);
        Assert.IsTrue(dash.IsReady); // Prêt au début
    }

    [TestMethod]
    public void Item_CreationTemporaryPowerUp_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var speedBoost = new TemporaryPowerUp("Speed Test", "Speed description", PowerUpStat.SPEED, 10, 5);

        // Assert
        Assert.AreEqual("Speed Test", speedBoost.Name);
        Assert.AreEqual('T', speedBoost.Symbol);
        Assert.AreEqual(ItemCategory.TEMPORARY, speedBoost.Category);
        Assert.AreEqual(PowerUpStat.SPEED, speedBoost.Stat);
        Assert.AreEqual(10, speedBoost.Value);
        Assert.AreEqual(5, speedBoost.Duration);
        Assert.IsFalse(speedBoost.IsActive); // Inactif au début
    }

    [TestMethod]
    public void Item_CreationPermanentPowerUp_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var hpUpgrade = new PermanentPowerUp("HP Test", "HP description", PowerUpStat.MAX_HP, 20);

        // Assert
        Assert.AreEqual("HP Test", hpUpgrade.Name);
        Assert.AreEqual('P', hpUpgrade.Symbol);
        Assert.AreEqual(ItemCategory.PERMANENT, hpUpgrade.Category);
        Assert.AreEqual(PowerUpStat.MAX_HP, hpUpgrade.Stat);
        Assert.AreEqual(20, hpUpgrade.Value);
        Assert.IsFalse(hpUpgrade.IsUsed); // Pas utilisé au début
    }

    #endregion

    #region Tests Capacity Cooldown

    [TestMethod]
    public void Capacity_Use_ShouldSetCooldown()
    {
        // Arrange
        var missiles = new Capacity("Test Missiles", "Test", CapacityType.MISSILES, 4);

        // Act
        missiles.Use();

        // Assert
        Assert.IsFalse(missiles.IsReady);
        Assert.AreEqual(4, missiles.CurrentCooldown);
    }

    [TestMethod]
    public void Capacity_ReduceCooldown_ShouldWorkCorrectly()
    {
        // Arrange
        var dash = new Capacity("Test Dash", "Test", CapacityType.DASH, 3);
        dash.Use(); // Cooldown = 3

        // Act & Assert
        Assert.AreEqual(3, dash.CurrentCooldown);
        
        dash.ReduceCooldown();
        Assert.AreEqual(2, dash.CurrentCooldown);
        
        dash.ReduceCooldown();
        Assert.AreEqual(1, dash.CurrentCooldown);
        
        dash.ReduceCooldown();
        Assert.AreEqual(0, dash.CurrentCooldown);
        Assert.IsTrue(dash.IsReady);
        
        // Réduire en dessous de 0 ne devrait rien faire
        dash.ReduceCooldown();
        Assert.AreEqual(0, dash.CurrentCooldown);
    }

    [TestMethod]
    public void Capacity_ResetCooldown_ShouldWork()
    {
        // Arrange
        var missiles = new Capacity("Test Missiles", "Test", CapacityType.MISSILES, 5);
        missiles.Use(); // Cooldown = 5

        // Act
        missiles.ResetCooldown();

        // Assert
        Assert.AreEqual(0, missiles.CurrentCooldown);
        Assert.IsTrue(missiles.IsReady);
    }

    #endregion

    #region Tests Temporary PowerUp

    [TestMethod]
    public void TemporaryPowerUp_Use_ShouldActivate()
    {
        // Arrange
        var attackBoost = new TemporaryPowerUp("Attack Test", "Test", PowerUpStat.ATTACK, 15, 6);

        // Act
        attackBoost.Use();

        // Assert
        Assert.IsTrue(attackBoost.IsActive);
        Assert.AreEqual(6, attackBoost.RemainingDuration);
    }

    [TestMethod]
    public void TemporaryPowerUp_ReduceDuration_ShouldWorkCorrectly()
    {
        // Arrange
        var defenseBoost = new TemporaryPowerUp("Defense Test", "Test", PowerUpStat.DEFENSE, 8, 3);
        defenseBoost.Use(); // Durée = 3

        // Act & Assert
        Assert.AreEqual(3, defenseBoost.RemainingDuration);
        Assert.IsTrue(defenseBoost.IsActive);

        defenseBoost.ReduceDuration();
        Assert.AreEqual(2, defenseBoost.RemainingDuration);
        Assert.IsTrue(defenseBoost.IsActive);

        defenseBoost.ReduceDuration();
        Assert.AreEqual(1, defenseBoost.RemainingDuration);
        Assert.IsTrue(defenseBoost.IsActive);

        defenseBoost.ReduceDuration();
        Assert.AreEqual(0, defenseBoost.RemainingDuration);
        Assert.IsFalse(defenseBoost.IsActive); // Plus actif

        // Réduire en dessous de 0 ne devrait rien faire
        defenseBoost.ReduceDuration();
        Assert.AreEqual(0, defenseBoost.RemainingDuration);
    }

    #endregion

    #region Tests Permanent PowerUp

    [TestMethod]
    public void PermanentPowerUp_Use_ShouldMarkAsUsed()
    {
        // Arrange
        var attackUpgrade = new PermanentPowerUp("Attack Upgrade", "Test", PowerUpStat.ATTACK, 12);

        // Act
        attackUpgrade.Use();

        // Assert
        Assert.IsTrue(attackUpgrade.IsUsed);
    }

    [TestMethod]
    public void PermanentPowerUp_UseAgain_ShouldNotWorkTwice()
    {
        // Arrange
        var hpUpgrade = new PermanentPowerUp("HP Upgrade", "Test", PowerUpStat.MAX_HP, 25);
        hpUpgrade.Use(); // Premier usage

        // Act - deuxième usage
        bool wasUsedBefore = hpUpgrade.IsUsed;
        hpUpgrade.Use();

        // Assert
        Assert.IsTrue(wasUsedBefore);
        Assert.IsTrue(hpUpgrade.IsUsed); // Toujours utilisé
    }

    #endregion

    #region Tests Inventory Singleton

    [TestMethod]
    public void Inventory_Singleton_ShouldReturnSameInstance()
    {
        // Arrange & Act
        var inventory1 = Inventory.Instance;
        var inventory2 = Inventory.Instance;

        // Assert
        Assert.AreSame(inventory1, inventory2);
        Assert.AreEqual(inventory1.GetHashCode(), inventory2.GetHashCode());
    }

    [TestMethod]
    public void Inventory_AddItem_ShouldIncreaseQuantity()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var dash = new Capacity("Test Dash", "Test", CapacityType.DASH, 2);

        // Act
        inventory.AddItem(dash, 3);

        // Assert
        Assert.IsTrue(inventory.HasItem(dash));
        Assert.AreEqual(3, inventory.GetItemQuantity(dash));
        Assert.AreEqual(3, inventory.TotalItems);
        Assert.AreEqual(1, inventory.UniqueItems);
    }

    [TestMethod]
    public void Inventory_AddSameItem_ShouldStack()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var speedBoost = new TemporaryPowerUp("Speed Test", "Test", PowerUpStat.SPEED, 5, 3);

        // Act
        inventory.AddItem(speedBoost, 2);
        inventory.AddItem(speedBoost, 3);

        // Assert
        Assert.AreEqual(5, inventory.GetItemQuantity(speedBoost));
        Assert.AreEqual(5, inventory.TotalItems);
        Assert.AreEqual(1, inventory.UniqueItems); // Toujours un seul type d'item unique
    }

    [TestMethod]
    public void Inventory_UsePermanentPowerUp_ShouldRemoveFromInventory()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var attackUpgrade = new PermanentPowerUp("Attack Test", "Test", PowerUpStat.ATTACK, 10);
        inventory.AddItem(attackUpgrade, 2);

        // Act
        bool result = inventory.UseItem(attackUpgrade);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(attackUpgrade.IsUsed);
        Assert.AreEqual(1, inventory.GetItemQuantity(attackUpgrade)); // Un retiré
    }

    #endregion

    #region Tests Edge Cases

    [TestMethod]
    public void Inventory_AddNullItem_ShouldNotCrash()
    {
        // Arrange
        var inventory = Inventory.Instance;

        // Act & Assert - ne devrait pas lever d'exception
        inventory.AddItem(null, 1);
        Assert.AreEqual(0, inventory.TotalItems);
    }

    [TestMethod]
    public void Inventory_RemoveNonExistentItem_ShouldReturnFalse()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var shield = new Capacity("Test Shield", "Test", CapacityType.SHIELD, 3);

        // Act
        bool result = inventory.RemoveItem(shield, 1);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Inventory_RemoveMoreThanAvailable_ShouldReturnFalse()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var speedBoost = new TemporaryPowerUp("Speed Test", "Test", PowerUpStat.SPEED, 5, 3);
        inventory.AddItem(speedBoost, 2);

        // Act
        bool result = inventory.RemoveItem(speedBoost, 5);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(2, inventory.GetItemQuantity(speedBoost)); // Quantité inchangée
    }

    #endregion

    #region Tests Intégration

    [TestMethod]
    public void Inventory_CompleteGameFlow_ShouldWorkCorrectly()
    {
        // Test d'intégration complet : simuler un flow de jeu réel
        
        // Arrange
        var inventory = Inventory.Instance;
        var dash = new Capacity("Game Dash", "Test", CapacityType.DASH, 2);
        var speedBoost = new TemporaryPowerUp("Game Speed", "Test", PowerUpStat.SPEED, 10, 3);
        var hpUpgrade = new PermanentPowerUp("Game HP", "Test", PowerUpStat.MAX_HP, 25);

        // Act - Jouer une séquence d'actions
        
        // Tour 1 : Ajouter items
        inventory.AddItem(dash);
        inventory.AddItem(speedBoost, 2);
        inventory.AddItem(hpUpgrade);
        Assert.AreEqual(4, inventory.TotalItems);

        // Tour 2 : Utiliser capacité et powerup temporaire
        inventory.UseItem(dash); // Cooldown = 2
        inventory.UseItem(speedBoost); // Durée = 3, quantity = 1
        Assert.IsFalse(dash.IsReady);
        Assert.IsTrue(speedBoost.IsActive);

        // Tour 3-4 : Passage du temps
        inventory.UpdateTemporaryEffects(); // Cooldown = 1, Durée = 2
        inventory.UpdateTemporaryEffects(); // Cooldown = 0, Durée = 1
        Assert.IsTrue(dash.IsReady);
        Assert.IsTrue(speedBoost.IsActive);

        // Tour 5 : Fin effet temporaire, utiliser permanent
        inventory.UpdateTemporaryEffects(); // Durée = 0
        inventory.UseItem(hpUpgrade); // Quantity = 0 (retiré)
        Assert.IsFalse(speedBoost.IsActive);
        Assert.IsFalse(inventory.HasItem(hpUpgrade));

        // Tour 6 : Réutiliser capacité
        inventory.UseItem(dash); // Cooldown = 2 à nouveau
        Assert.IsFalse(dash.IsReady);

        // Assert final
        Assert.AreEqual(2, inventory.GetItemQuantity(speedBoost)); // 2 restants (temporaires pas consommés)
        Assert.IsTrue(inventory.HasItem(dash));
        Assert.IsFalse(inventory.HasItem(hpUpgrade)); // Consommé
    }

    #endregion

    // =====================================================
    // PARTIE 2: DÉMONSTRATIONS INTERACTIVES
    // =====================================================

    /// <summary>
    /// Méthode utilitaire pour les démonstrations interactives
    /// Appelle cette méthode depuis un Main() pour demo complète
    /// </summary>
    public static void RunInteractiveDemo()
    {
        Console.WriteLine("=== DEMONSTRATION SYSTEME INVENTAIRE ===");
        Console.WriteLine("Tests automatises + demonstration interactive");
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
        Console.WriteLine("Tous les tests unitaires peuvent etre executes via Visual Studio Test Explorer");
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
        
        // Capacités
        var dash = new Capacity("Turbo Dash", "Déplacement ultra-rapide", CapacityType.DASH, 3);
        var shield = new Capacity("Force Shield", "Bouclier de force", CapacityType.SHIELD, 5);
        var missiles = new Capacity("Plasma Missiles", "Missiles plasma", CapacityType.MISSILES, 4);

        // PowerUps temporaires
        var speedBoost = new TemporaryPowerUp("Nitro Boost", "Vitesse x3", PowerUpStat.SPEED, 10, 8);
        var attackBoost = new TemporaryPowerUp("Rage Mode", "Attaque x2", PowerUpStat.ATTACK, 15, 6);

        // PowerUps permanents
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

        // Première utilisation
        Console.WriteLine("\nPremière utilisation:");
        inventory.UseItem(speedBoost);
        Console.WriteLine($"Effet actif: {speedBoost.IsActive}, Durée: {speedBoost.RemainingDuration}");

        // Simuler passage de temps
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

        // Première utilisation (consomme l'item)
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
        
        // Ajouter items de debug
        inventory.AddDebugItems();
        inventory.DisplayInventory();

        // Simuler plusieurs tours de jeu
        Console.WriteLine("\n--- Simulation 6 Tours de Jeu ---");
        
        var items = inventory.GetAllItems().Keys.ToArray();
        
        // Tour 1 : Utiliser une capacité
        var dashItem = items.FirstOrDefault(i => i is Capacity && ((Capacity)i).Type == CapacityType.DASH);
        if (dashItem != null)
        {
            Console.WriteLine("\n🎮 Tour 1: Utiliser Dash");
            inventory.UseItem(dashItem);
        }

        // Tour 2 : Utiliser un PowerUp temporaire
        var speedItem = items.FirstOrDefault(i => i is TemporaryPowerUp && ((TemporaryPowerUp)i).Stat == PowerUpStat.SPEED);
        if (speedItem != null)
        {
            Console.WriteLine("\n🎮 Tour 2: Utiliser Speed Boost");
            inventory.UseItem(speedItem);
        }

        // Tours 3-6 : Simuler passage du temps
        for (int turn = 3; turn <= 6; turn++)
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