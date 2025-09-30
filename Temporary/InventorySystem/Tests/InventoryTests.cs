using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests unitaires pour le système d'inventaire complet
/// Couvre: Item, Capacity, PowerUp (Temporary/Permanent), Inventory
/// </summary>
[TestClass]
public class InventoryTests
{
    // === SETUP ET TEARDOWN ===
    [TestInitialize]
    public void Setup()
    {
        Inventory.Instance.ClearInventory();
    }

    // === TESTS ITEM DE BASE ===
    #region Tests Item Creation

    [TestMethod]
    public void Item_CreationCapacity_ShouldHaveCorrectProperties()
    {
        var dash = new Capacity("Test Dash", "Description test", CapacityType.DASH, 3);

        Assert.AreEqual("Test Dash", dash.Name);
        Assert.AreEqual("Description test", dash.Description);
        Assert.AreEqual('C', dash.Symbol);
        Assert.AreEqual(ItemCategory.CAPACITY, dash.Category);
        Assert.AreEqual(CapacityType.DASH, dash.Type);
        Assert.AreEqual(3, dash.Cooldown);
        Assert.IsTrue(dash.IsReady);
    }

    [TestMethod]
    public void Item_CreationTemporaryPowerUp_ShouldHaveCorrectProperties()
    {
        var speedBoost = new TemporaryPowerUp("Speed Test", "Speed description", PowerUpStat.SPEED, 10, 5);

        Assert.AreEqual("Speed Test", speedBoost.Name);
        Assert.AreEqual('T', speedBoost.Symbol);
        Assert.AreEqual(ItemCategory.TEMPORARY, speedBoost.Category);
        Assert.AreEqual(PowerUpStat.SPEED, speedBoost.Stat);
        Assert.AreEqual(10, speedBoost.Value);
        Assert.AreEqual(5, speedBoost.Duration);
        Assert.IsFalse(speedBoost.IsActive);
    }

    [TestMethod]
    public void Item_CreationPermanentPowerUp_ShouldHaveCorrectProperties()
    {
        var hpUpgrade = new PermanentPowerUp("HP Test", "HP description", PowerUpStat.MAX_HP, 20);

        Assert.AreEqual("HP Test", hpUpgrade.Name);
        Assert.AreEqual('P', hpUpgrade.Symbol);
        Assert.AreEqual(ItemCategory.PERMANENT, hpUpgrade.Category);
        Assert.AreEqual(PowerUpStat.MAX_HP, hpUpgrade.Stat);
        Assert.AreEqual(20, hpUpgrade.Value);
        Assert.IsFalse(hpUpgrade.IsUsed); 
    }

    #endregion

    // === TESTS CAPACITY COOLDOWN ===
    #region Tests Capacity Cooldown

    [TestMethod]
    public void Capacity_Use_ShouldSetCooldown()
    {
        var missiles = new Capacity("Test Missiles", "Test", CapacityType.MISSILES, 4);

        missiles.Use();

        Assert.IsFalse(missiles.IsReady);
        Assert.AreEqual(4, missiles.CurrentCooldown);
    }

    [TestMethod]
    public void Capacity_ReduceCooldown_ShouldWorkCorrectly()
    {
        var dash = new Capacity("Test Dash", "Test", CapacityType.DASH, 3);
        dash.Use(); 

        Assert.AreEqual(3, dash.CurrentCooldown);
        
        dash.ReduceCooldown();
        Assert.AreEqual(2, dash.CurrentCooldown);
        
        dash.ReduceCooldown();
        Assert.AreEqual(1, dash.CurrentCooldown);
        
        dash.ReduceCooldown();
        Assert.AreEqual(0, dash.CurrentCooldown);
        Assert.IsTrue(dash.IsReady);
        
        dash.ReduceCooldown();
        Assert.AreEqual(0, dash.CurrentCooldown);
    }

    [TestMethod]
    public void Capacity_ResetCooldown_ShouldWork()
    {
        var missiles = new Capacity("Test Missiles", "Test", CapacityType.MISSILES, 5);
        missiles.Use();

        missiles.ResetCooldown();

        Assert.AreEqual(0, missiles.CurrentCooldown);
        Assert.IsTrue(missiles.IsReady);
    }

    #endregion

    // === TESTS TEMPORARY POWERUP ===
    #region Tests Temporary PowerUp

    [TestMethod]
    public void TemporaryPowerUp_Use_ShouldActivate()
    {
        var attackBoost = new TemporaryPowerUp("Attack Test", "Test", PowerUpStat.ATTACK, 15, 6);

        attackBoost.Use();

        Assert.IsTrue(attackBoost.IsActive);
        Assert.AreEqual(6, attackBoost.RemainingDuration);
    }

    [TestMethod]
    public void TemporaryPowerUp_ReduceDuration_ShouldWorkCorrectly()
    {
        var defenseBoost = new TemporaryPowerUp("Defense Test", "Test", PowerUpStat.DEFENSE, 8, 3);
        defenseBoost.Use();

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
        Assert.IsFalse(defenseBoost.IsActive); 
    }

    #endregion

    // === TESTS PERMANENT POWERUP ===
    #region Tests Permanent PowerUp

    [TestMethod]
    public void PermanentPowerUp_Use_ShouldMarkAsUsed()
    {
        var attackUpgrade = new PermanentPowerUp("Attack Upgrade", "Test", PowerUpStat.ATTACK, 12);

        attackUpgrade.Use();

        Assert.IsTrue(attackUpgrade.IsUsed);
    }

    [TestMethod]
    public void PermanentPowerUp_UseAgain_ShouldNotWorkTwice()
    {
        var hpUpgrade = new PermanentPowerUp("HP Upgrade", "Test", PowerUpStat.MAX_HP, 25);
        hpUpgrade.Use(); 

        bool wasUsedBefore = hpUpgrade.IsUsed;
        hpUpgrade.Use();

        Assert.IsTrue(wasUsedBefore);
        Assert.IsTrue(hpUpgrade.IsUsed);
    }

    #endregion

    // === TESTS INVENTORY SINGLETON ===
    #region Tests Inventory Singleton

    [TestMethod]
    public void Inventory_Singleton_ShouldReturnSameInstance()
    {
        var inventory1 = Inventory.Instance;
        var inventory2 = Inventory.Instance;

        Assert.AreSame(inventory1, inventory2);
        Assert.AreEqual(inventory1.GetHashCode(), inventory2.GetHashCode());
    }

    [TestMethod]
    public void Inventory_AddItem_ShouldIncreaseQuantity()
    {
        var inventory = Inventory.Instance;
        var dash = new Capacity("Test Dash", "Test", CapacityType.DASH, 2);

        inventory.AddItem(dash, 3);

        Assert.IsTrue(inventory.HasItem(dash));
        Assert.AreEqual(3, inventory.GetItemQuantity(dash));
        Assert.AreEqual(3, inventory.TotalItems);
        Assert.AreEqual(1, inventory.UniqueItems);
    }

    [TestMethod]
    public void Inventory_AddSameItem_ShouldStack()
    {
        var inventory = Inventory.Instance;
        var speedBoost = new TemporaryPowerUp("Speed Test", "Test", PowerUpStat.SPEED, 5, 3);

        inventory.AddItem(speedBoost, 2);
        inventory.AddItem(speedBoost, 3);

        Assert.AreEqual(5, inventory.GetItemQuantity(speedBoost));
        Assert.AreEqual(5, inventory.TotalItems);
        Assert.AreEqual(1, inventory.UniqueItems);
    }

    [TestMethod]
    public void Inventory_UsePermanentPowerUp_ShouldRemoveFromInventory()
    {
        var inventory = Inventory.Instance;
        var attackUpgrade = new PermanentPowerUp("Attack Test", "Test", PowerUpStat.ATTACK, 10);
        inventory.AddItem(attackUpgrade, 2);

        bool result = inventory.UseItem(attackUpgrade);

        Assert.IsTrue(result);
        Assert.IsTrue(attackUpgrade.IsUsed);
        Assert.AreEqual(1, inventory.GetItemQuantity(attackUpgrade));
    }

    #endregion

    // === TESTS EDGE CASES ===
    #region Tests Edge Cases

    [TestMethod]
    public void Inventory_AddNullItem_ShouldNotCrash()
    {
        var inventory = Inventory.Instance;

        inventory.AddItem(null, 1);
        Assert.AreEqual(0, inventory.TotalItems);
    }

    [TestMethod]
    public void Inventory_RemoveNonExistentItem_ShouldReturnFalse()
    {
        var inventory = Inventory.Instance;
        var shield = new Capacity("Test Shield", "Test", CapacityType.SHIELD, 3);

        bool result = inventory.RemoveItem(shield, 1);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Inventory_RemoveMoreThanAvailable_ShouldReturnFalse()
    {
        var inventory = Inventory.Instance;
        var speedBoost = new TemporaryPowerUp("Speed Test", "Test", PowerUpStat.SPEED, 5, 3);
        inventory.AddItem(speedBoost, 2);

        bool result = inventory.RemoveItem(speedBoost, 5);

        Assert.IsFalse(result);
        Assert.AreEqual(2, inventory.GetItemQuantity(speedBoost));
    }

    #endregion

    // === TEST INTÉGRATION COMPLÈTE ===
    #region Tests Intégration

    [TestMethod]
    public void Inventory_CompleteGameFlow_ShouldWorkCorrectly()
    {
        var inventory = Inventory.Instance;
        var dash = new Capacity("Game Dash", "Test", CapacityType.DASH, 2);
        var speedBoost = new TemporaryPowerUp("Game Speed", "Test", PowerUpStat.SPEED, 10, 3);
        var hpUpgrade = new PermanentPowerUp("Game HP", "Test", PowerUpStat.MAX_HP, 25);
        
        // Tour 1 : Ajouter items
        inventory.AddItem(dash);
        inventory.AddItem(speedBoost, 2);
        inventory.AddItem(hpUpgrade);
        Assert.AreEqual(4, inventory.TotalItems);

        // Tour 2 : Utiliser capacité et powerup temporaire
        inventory.UseItem(dash); 
        inventory.UseItem(speedBoost); 
        Assert.IsFalse(dash.IsReady);
        Assert.IsTrue(speedBoost.IsActive);

        // Tour 3-4 : Passage du temps
        inventory.UpdateTemporaryEffects(); 
        inventory.UpdateTemporaryEffects();
        Assert.IsTrue(dash.IsReady);
        Assert.IsTrue(speedBoost.IsActive);

        // Tour 5 : Fin effet temporaire, utiliser permanent
        inventory.UpdateTemporaryEffects();
        inventory.UseItem(hpUpgrade); 
        Assert.IsFalse(speedBoost.IsActive);
        Assert.IsFalse(inventory.HasItem(hpUpgrade));

        // Tour 6 : Réutiliser capacité
        inventory.UseItem(dash);
        Assert.IsFalse(dash.IsReady);

        // Assert final
        Assert.AreEqual(1, inventory.GetItemQuantity(speedBoost));
        Assert.IsTrue(inventory.HasItem(dash));
        Assert.IsFalse(inventory.HasItem(hpUpgrade));
    }

    #endregion
}
