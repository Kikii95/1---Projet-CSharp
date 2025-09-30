using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests unitaires pour PowerUpHelper
/// Couvre: Conversion PowerUp → StatModifier et intégration Inventory + Ship
/// </summary>
[TestClass]
public class PowerUpHelperTests
{
    // === SETUP ===
    [TestInitialize]
    public void Setup()
    {
        Inventory.Instance.ClearInventory();
    }

    // === TESTS CONVERSION PERMANENT POWERUP ===
    #region Tests Conversion Permanent

    [TestMethod]
    public void PowerUpHelper_ConvertPermanentPowerUp_ShouldCreateCorrectModifier()
    {
        var powerUp = new PermanentPowerUp("Attack Boost", "Test", PowerUpStat.ATTACK, 10);

        StatModifier modifier = PowerUpHelper.ConvertToStatModifier(powerUp);

        Assert.IsNotNull(modifier);
        Assert.AreEqual("Attack Boost", modifier.Name);
        Assert.AreEqual(ModifiableStat.ATTACK, modifier.Stat);
        Assert.AreEqual(ModifierType.FLAT, modifier.Type);
        Assert.AreEqual(10, modifier.Value);
        Assert.IsTrue(modifier.IsPermanent);
    }

    [TestMethod]
    public void PowerUpHelper_ConvertPermanentPowerUp_AllStats_ShouldWork()
    {
        var attackPU = new PermanentPowerUp("ATK", "Test", PowerUpStat.ATTACK, 5);
        var defensePU = new PermanentPowerUp("DEF", "Test", PowerUpStat.DEFENSE, 5);
        var speedPU = new PermanentPowerUp("SPD", "Test", PowerUpStat.SPEED, 5);
        var hpPU = new PermanentPowerUp("HP", "Test", PowerUpStat.MAX_HP, 5);

        var atkMod = PowerUpHelper.ConvertToStatModifier(attackPU);
        var defMod = PowerUpHelper.ConvertToStatModifier(defensePU);
        var spdMod = PowerUpHelper.ConvertToStatModifier(speedPU);
        var hpMod = PowerUpHelper.ConvertToStatModifier(hpPU);

        Assert.AreEqual(ModifiableStat.ATTACK, atkMod.Stat);
        Assert.AreEqual(ModifiableStat.DEFENSE, defMod.Stat);
        Assert.AreEqual(ModifiableStat.SPEED, spdMod.Stat);
        Assert.AreEqual(ModifiableStat.MAX_HP, hpMod.Stat);
    }

    #endregion

    // === TESTS CONVERSION TEMPORARY POWERUP ===
    #region Tests Conversion Temporary

    [TestMethod]
    public void PowerUpHelper_ConvertTemporaryPowerUp_ShouldCreateCorrectModifier()
    {
        var powerUp = new TemporaryPowerUp("Speed Boost", "Test", PowerUpStat.SPEED, 15, 5);

        StatModifier modifier = PowerUpHelper.ConvertToStatModifier(powerUp);

        Assert.IsNotNull(modifier);
        Assert.AreEqual("Speed Boost", modifier.Name);
        Assert.AreEqual(ModifiableStat.SPEED, modifier.Stat);
        Assert.AreEqual(ModifierType.FLAT, modifier.Type);
        Assert.AreEqual(15, modifier.Value);
        Assert.AreEqual(5, modifier.Duration);
        Assert.IsFalse(modifier.IsPermanent);
    }

    #endregion

    // === TESTS CONVERSION POLYMORPHE ===
    #region Tests Conversion Polymorphe

    [TestMethod]
    public void PowerUpHelper_ConvertPowerUp_WithPermanent_ShouldWork()
    {
        PowerUp powerUp = new PermanentPowerUp("Test", "Test", PowerUpStat.ATTACK, 10);

        StatModifier modifier = PowerUpHelper.ConvertToStatModifier(powerUp);

        Assert.IsNotNull(modifier);
        Assert.IsTrue(modifier.IsPermanent);
    }

    [TestMethod]
    public void PowerUpHelper_ConvertPowerUp_WithTemporary_ShouldWork()
    {
        PowerUp powerUp = new TemporaryPowerUp("Test", "Test", PowerUpStat.SPEED, 10, 3);

        StatModifier modifier = PowerUpHelper.ConvertToStatModifier(powerUp);

        Assert.IsNotNull(modifier);
        Assert.IsFalse(modifier.IsPermanent);
    }

    [TestMethod]
    public void PowerUpHelper_ConvertPowerUp_WithNull_ShouldReturnNull()
    {
        StatModifier modifier = PowerUpHelper.ConvertToStatModifier((PowerUp)null);

        Assert.IsNull(modifier);
    }

    #endregion

    // === TESTS APPLICATION SUR SHIP ===
    #region Tests Application Ship

    [TestMethod]
    public void PowerUpHelper_ApplyPowerUpToShip_ShouldApplyModifierAndRemoveFromInventory()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("Test", ShipType.NORMAL, stats);
        var inventory = Inventory.Instance;
        var powerUp = new PermanentPowerUp("Attack Boost", "Test", PowerUpStat.ATTACK, 10);
        
        inventory.AddItem(powerUp);
        int initialAttack = ship.GetEffectiveAttack();

        bool result = PowerUpHelper.ApplyPowerUpToShip(ship, powerUp, inventory);

        Assert.IsTrue(result);
        Assert.AreEqual(initialAttack + 10, ship.GetEffectiveAttack());
        Assert.IsFalse(inventory.HasItem(powerUp));
    }

    [TestMethod]
    public void PowerUpHelper_ApplyPowerUpToShip_WithTemporary_ShouldWork()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("Test", ShipType.NORMAL, stats);
        var inventory = Inventory.Instance;
        var powerUp = new TemporaryPowerUp("Speed Boost", "Test", PowerUpStat.SPEED, 10, 3);
        
        inventory.AddItem(powerUp, 2);
        int initialSpeed = ship.Stats.EffectiveSpeed;

        bool result = PowerUpHelper.ApplyPowerUpToShip(ship, powerUp, inventory);

        Assert.IsTrue(result);
        Assert.AreEqual(initialSpeed + 10, ship.Stats.EffectiveSpeed);
        Assert.AreEqual(1, inventory.GetItemQuantity(powerUp));
    }

    [TestMethod]
    public void PowerUpHelper_ApplyPowerUpToShip_NotInInventory_ShouldReturnFalse()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("Test", ShipType.NORMAL, stats);
        var inventory = Inventory.Instance;
        var powerUp = new PermanentPowerUp("Attack Boost", "Test", PowerUpStat.ATTACK, 10);

        bool result = PowerUpHelper.ApplyPowerUpToShip(ship, powerUp, inventory);

        Assert.IsFalse(result);
    }

    #endregion

    // === TESTS MÉTHODES UTILITAIRES ===
    #region Tests Utilitaires

    [TestMethod]
    public void PowerUpHelper_IsPowerUp_WithPowerUp_ShouldReturnTrue()
    {
        Item item = new PermanentPowerUp("Test", "Test", PowerUpStat.ATTACK, 10);

        bool result = PowerUpHelper.IsPowerUp(item);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PowerUpHelper_IsPowerUp_WithCapacity_ShouldReturnFalse()
    {
        Item item = new Capacity("Test", "Test", CapacityType.DASH, 3);

        bool result = PowerUpHelper.IsPowerUp(item);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void PowerUpHelper_TryGetPowerUp_WithPowerUp_ShouldReturnPowerUp()
    {
        Item item = new PermanentPowerUp("Test", "Test", PowerUpStat.ATTACK, 10);

        PowerUp powerUp = PowerUpHelper.TryGetPowerUp(item);

        Assert.IsNotNull(powerUp);
        Assert.AreEqual("Test", powerUp.Name);
    }

    [TestMethod]
    public void PowerUpHelper_TryGetPowerUp_WithCapacity_ShouldReturnNull()
    {
        Item item = new Capacity("Test", "Test", CapacityType.DASH, 3);

        PowerUp powerUp = PowerUpHelper.TryGetPowerUp(item);

        Assert.IsNull(powerUp);
    }

    #endregion

    // === TEST INTÉGRATION COMPLÈTE ===
    #region Test Intégration

    [TestMethod]
    public void Integration_UseMultiplePowerUps_ShouldStackCorrectly()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("Hero", ShipType.NORMAL, stats);
        var inventory = Inventory.Instance;

        var attackPerm = new PermanentPowerUp("Weapon", "Test", PowerUpStat.ATTACK, 5);
        var attackTemp = new TemporaryPowerUp("Rage", "Test", PowerUpStat.ATTACK, 10, 3);
        var defensePerm = new PermanentPowerUp("Armor", "Test", PowerUpStat.DEFENSE, 3);

        inventory.AddItem(attackPerm);
        inventory.AddItem(attackTemp, 2);
        inventory.AddItem(defensePerm);

        // Application
        PowerUpHelper.ApplyPowerUpToShip(ship, attackPerm, inventory);
        PowerUpHelper.ApplyPowerUpToShip(ship, attackTemp, inventory);
        PowerUpHelper.ApplyPowerUpToShip(ship, defensePerm, inventory);

        // Vérifications
        Assert.AreEqual(20 + 5 + 10, ship.GetEffectiveAttack());
        Assert.AreEqual(10 + 3, ship.Stats.EffectiveDefense);
        Assert.AreEqual(2, ship.Stats.GetActiveModifiers().Count);

        // Après expiration du temporaire
        ship.Update();
        ship.Update();
        ship.Update();

        Assert.AreEqual(20 + 5, ship.GetEffectiveAttack());
        Assert.AreEqual(1, ship.Stats.GetActiveModifiers().Count);
    }

    #endregion
}
