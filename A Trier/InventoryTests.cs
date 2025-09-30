using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

/// <summary>
/// Tests Unitaires pour le système d'inventaire
/// Selon cours POO : prouver que le code fonctionne dans tous les cas
/// Gestion des cas "exotiques" et mauvaises utilisations
/// </summary>
[TestClass]
public class InventoryTests
{
    // === SETUP ET TEARDOWN ===
    [TestInitialize]
    public void Setup()
    {
        // Reset l'inventaire avant chaque test
        Inventory.Instance.ClearInventory();
    }

    // === TESTS ITEM DE BASE ===
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

    // === TESTS CAPACITY COOLDOWN ===

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
    public void Capacity_UseInCooldown_ShouldNotWork()
    {
        // Arrange
        var shield = new Capacity("Test Shield", "Test", CapacityType.SHIELD, 3);
        shield.Use(); // Premier usage

        // Act & Assert - deuxième usage immédiat
        Assert.IsFalse(shield.IsReady);
        // Note: On ne peut pas tester la sortie console facilement, 
        // mais on vérifie que le cooldown n'a pas changé
        var cooldownBefore = shield.CurrentCooldown;
        shield.Use();
        Assert.AreEqual(cooldownBefore, shield.CurrentCooldown);
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

    // === TESTS TEMPORARY POWERUP ===

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
    public void TemporaryPowerUp_UseWhileActive_ShouldNotRestart()
    {
        // Arrange
        var speedBoost = new TemporaryPowerUp("Speed Test", "Test", PowerUpStat.SPEED, 5, 4);
        speedBoost.Use(); // Première activation
        speedBoost.ReduceDuration(); // Durée = 3

        // Act - tentative de réactivation
        speedBoost.Use();

        // Assert - durée ne devrait pas être remise à 4
        Assert.AreEqual(3, speedBoost.RemainingDuration);
        Assert.IsTrue(speedBoost.IsActive);
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

    // === TESTS PERMANENT POWERUP ===

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

    // === TESTS INVENTORY SINGLETON ===

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
    public void Inventory_RemoveItem_ShouldDecreaseQuantity()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var missiles = new Capacity("Test Missiles", "Test", CapacityType.MISSILES, 4);
        inventory.AddItem(missiles, 5);

        // Act
        bool result = inventory.RemoveItem(missiles, 2);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(3, inventory.GetItemQuantity(missiles));
        Assert.AreEqual(3, inventory.TotalItems);
    }

    [TestMethod]
    public void Inventory_RemoveAllItems_ShouldRemoveFromInventory()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var hpUpgrade = new PermanentPowerUp("HP Test", "Test", PowerUpStat.MAX_HP, 20);
        inventory.AddItem(hpUpgrade, 3);

        // Act
        bool result = inventory.RemoveItem(hpUpgrade, 3);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(inventory.HasItem(hpUpgrade));
        Assert.AreEqual(0, inventory.GetItemQuantity(hpUpgrade));
        Assert.AreEqual(0, inventory.TotalItems);
        Assert.AreEqual(0, inventory.UniqueItems);
    }

    // === TESTS EDGE CASES ===

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
    public void Inventory_AddNegativeQuantity_ShouldNotAdd()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var dash = new Capacity("Test Dash", "Test", CapacityType.DASH, 2);

        // Act
        inventory.AddItem(dash, -5);

        // Assert
        Assert.IsFalse(inventory.HasItem(dash));
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

    [TestMethod]
    public void Inventory_UseNonExistentItem_ShouldReturnFalse()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var missiles = new Capacity("Test Missiles", "Test", CapacityType.MISSILES, 4);

        // Act
        bool result = inventory.UseItem(missiles);

        // Assert
        Assert.IsFalse(result);
    }

    // === TESTS INTÉGRATION ===

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

    [TestMethod]
    public void Inventory_GetItemsByCategory_ShouldFilterCorrectly()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var dash = new Capacity("Dash", "Test", CapacityType.DASH, 2);
        var shield = new Capacity("Shield", "Test", CapacityType.SHIELD, 3);
        var speedBoost = new TemporaryPowerUp("Speed", "Test", PowerUpStat.SPEED, 5, 4);
        var hpUpgrade = new PermanentPowerUp("HP", "Test", PowerUpStat.MAX_HP, 20);

        inventory.AddItem(dash);
        inventory.AddItem(shield);
        inventory.AddItem(speedBoost);
        inventory.AddItem(hpUpgrade);

        // Act
        var capacities = inventory.GetItemsByCategory(ItemCategory.CAPACITY);
        var temporaries = inventory.GetItemsByCategory(ItemCategory.TEMPORARY);
        var permanents = inventory.GetItemsByCategory(ItemCategory.PERMANENT);

        // Assert
        Assert.AreEqual(2, capacities.Count);
        Assert.AreEqual(1, temporaries.Count);
        Assert.AreEqual(1, permanents.Count);
        Assert.IsTrue(capacities.Contains(dash));
        Assert.IsTrue(capacities.Contains(shield));
        Assert.IsTrue(temporaries.Contains(speedBoost));
        Assert.IsTrue(permanents.Contains(hpUpgrade));
    }

    [TestMethod]
    public void Inventory_UpdateTemporaryEffects_ShouldReduceAllCooldowns()
    {
        // Arrange
        var inventory = Inventory.Instance;
        var dash = new Capacity("Dash", "Test", CapacityType.DASH, 3);
        var missiles = new Capacity("Missiles", "Test", CapacityType.MISSILES, 2);
        var speedBoost = new TemporaryPowerUp("Speed", "Test", PowerUpStat.SPEED, 5, 4);

        inventory.AddItem(dash);
        inventory.AddItem(missiles);
        inventory.AddItem(speedBoost);

        // Utiliser les items pour déclencher cooldown/durée
        dash.Use(); // Cooldown = 3
        missiles.Use(); // Cooldown = 2
        speedBoost.Use(); // Durée = 4

        // Act
        inventory.UpdateTemporaryEffects();

        // Assert
        Assert.AreEqual(2, dash.CurrentCooldown);
        Assert.AreEqual(1, missiles.CurrentCooldown);
        Assert.AreEqual(3, speedBoost.RemainingDuration);
    }

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
        Assert.AreEqual(1, inventory.GetItemQuantity(speedBoost)); // 1 restant
        Assert.IsTrue(inventory.HasItem(dash));
        Assert.IsFalse(inventory.HasItem(hpUpgrade)); // Consommé
    }
}