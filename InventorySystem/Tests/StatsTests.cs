using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests unitaires pour le système de statistiques complet
/// Couvre: TypeEffectiveness, StatModifier, Stats, Ship (Player/Enemy)
/// </summary>
[TestClass]
public class StatsTests
{
    // === TESTS TYPE EFFECTIVENESS ===
    #region Tests Type Effectiveness

    [TestMethod]
    public void TypeEffectiveness_SuperEffective_ShouldReturnCorrectMultiplier()
    {
        float result = TypeEffectiveness.GetEffectiveness(AttackType.FEU, ShipType.GLACE);
        
        Assert.AreEqual(2.0f, result);
    }

    [TestMethod]
    public void TypeEffectiveness_NotVeryEffective_ShouldReturnCorrectMultiplier()
    {
        float result = TypeEffectiveness.GetEffectiveness(AttackType.FEU, ShipType.EAU);
        
        Assert.AreEqual(0.5f, result);
    }

    [TestMethod]
    public void TypeEffectiveness_NormalEffectiveness_ShouldReturnOne()
    {
        float result = TypeEffectiveness.GetEffectiveness(AttackType.NORMAL, ShipType.FEU);
        
        Assert.AreEqual(1.0f, result);
    }

    [TestMethod]
    public void TypeEffectiveness_STAB_ShouldApplyBonus()
    {
        bool hasStab = TypeEffectiveness.HasSTAB(AttackType.FEU, ShipType.FEU);
        
        Assert.IsTrue(hasStab);
    }

    [TestMethod]
    public void TypeEffectiveness_NoSTAB_ShouldNotApplyBonus()
    {
        bool hasStab = TypeEffectiveness.HasSTAB(AttackType.FEU, ShipType.GLACE);
        
        Assert.IsFalse(hasStab);
    }

    [TestMethod]
    public void TypeEffectiveness_TotalMultiplier_WithSTAB_ShouldBeCorrect()
    {
        float result = TypeEffectiveness.GetTotalMultiplier(AttackType.FEU, ShipType.FEU, ShipType.GLACE);
        
        Assert.AreEqual(3.0f, result);
    }

    [TestMethod]
    public void TypeEffectiveness_TotalMultiplier_WithoutSTAB_ShouldBeCorrect()
    {
        float result = TypeEffectiveness.GetTotalMultiplier(AttackType.FEU, ShipType.NORMAL, ShipType.GLACE);
        
        Assert.AreEqual(2.0f, result);
    }

    #endregion

    // === TESTS STAT MODIFIER ===
    #region Tests StatModifier

    [TestMethod]
    public void StatModifier_TemporaryCreation_ShouldHaveCorrectProperties()
    {
        var modifier = new StatModifier("Attack Boost", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 3);

        Assert.AreEqual("Attack Boost", modifier.Name);
        Assert.AreEqual(ModifiableStat.ATTACK, modifier.Stat);
        Assert.AreEqual(ModifierType.FLAT, modifier.Type);
        Assert.AreEqual(10, modifier.Value);
        Assert.AreEqual(3, modifier.Duration);
        Assert.AreEqual(3, modifier.RemainingDuration);
        Assert.IsFalse(modifier.IsPermanent);
        Assert.IsTrue(modifier.IsActive);
    }

    [TestMethod]
    public void StatModifier_PermanentCreation_ShouldHaveCorrectProperties()
    {
        var modifier = new StatModifier("Permanent Attack", ModifiableStat.ATTACK, ModifierType.FLAT, 5);

        Assert.IsTrue(modifier.IsPermanent);
        Assert.IsTrue(modifier.IsActive);
        Assert.AreEqual(-1, modifier.Duration);
    }

    [TestMethod]
    public void StatModifier_ReduceDuration_ShouldDecreaseCorrectly()
    {
        var modifier = new StatModifier("Test", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 3);

        modifier.ReduceDuration();
        Assert.AreEqual(2, modifier.RemainingDuration);
        Assert.IsTrue(modifier.IsActive);

        modifier.ReduceDuration();
        Assert.AreEqual(1, modifier.RemainingDuration);

        modifier.ReduceDuration();
        Assert.AreEqual(0, modifier.RemainingDuration);
        Assert.IsFalse(modifier.IsActive);
    }

    [TestMethod]
    public void StatModifier_ApplyFlat_ShouldAddValue()
    {
        var modifier = new StatModifier("Test", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 3);

        float result = modifier.Apply(50);

        Assert.AreEqual(60, result);
    }

    [TestMethod]
    public void StatModifier_ApplyPercentage_ShouldMultiplyValue()
    {
        var modifier = new StatModifier("Test", ModifiableStat.ATTACK, ModifierType.PERCENTAGE, 20, 3);

        float result = modifier.Apply(100);

        Assert.AreEqual(120, result);
    }

    [TestMethod]
    public void StatModifier_ApplyNegativeFlat_ShouldSubtractValue()
    {
        var modifier = new StatModifier("Debuff", ModifiableStat.ATTACK, ModifierType.FLAT, -10, 3);

        float result = modifier.Apply(50);

        Assert.AreEqual(40, result);
    }

    #endregion

    // === TESTS STATS CLASS ===
    #region Tests Stats

    [TestMethod]
    public void Stats_Creation_ShouldHaveCorrectDefaults()
    {
        var stats = new Stats(100, 20, 10, 15);

        Assert.AreEqual(100, stats.BaseMaxHP);
        Assert.AreEqual(20, stats.BaseAttack);
        Assert.AreEqual(10, stats.BaseDefense);
        Assert.AreEqual(15, stats.BaseSpeed);
        Assert.AreEqual(1, stats.Level);
        Assert.AreEqual(0, stats.CurrentXP);
    }

    [TestMethod]
    public void Stats_EffectiveStats_WithoutModifiers_ShouldEqualBase()
    {
        var stats = new Stats(100, 20, 10, 15);

        Assert.AreEqual(100, stats.EffectiveMaxHP);
        Assert.AreEqual(20, stats.EffectiveAttack);
        Assert.AreEqual(10, stats.EffectiveDefense);
        Assert.AreEqual(15, stats.EffectiveSpeed);
    }

    [TestMethod]
    public void Stats_AddModifier_ShouldAffectEffectiveStat()
    {
        var stats = new Stats(100, 20, 10, 15);
        var modifier = new StatModifier("Buff", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 3);

        stats.AddModifier(modifier);

        Assert.AreEqual(30, stats.EffectiveAttack);
    }

    [TestMethod]
    public void Stats_MultipleModifiers_ShouldStack()
    {
        var stats = new Stats(100, 20, 10, 15);
        var mod1 = new StatModifier("Buff1", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 3);
        var mod2 = new StatModifier("Buff2", ModifiableStat.ATTACK, ModifierType.FLAT, 5, 3);

        stats.AddModifier(mod1);
        stats.AddModifier(mod2);

        Assert.AreEqual(35, stats.EffectiveAttack);
    }

    [TestMethod]
    public void Stats_PercentageModifier_ShouldApplyAfterFlat()
    {
        var stats = new Stats(100, 20, 10, 15);
        var flatMod = new StatModifier("Flat", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 3);
        var percentMod = new StatModifier("Percent", ModifiableStat.ATTACK, ModifierType.PERCENTAGE, 50, 3);

        stats.AddModifier(flatMod);
        stats.AddModifier(percentMod);

        Assert.AreEqual(45, stats.EffectiveAttack);
    }

    [TestMethod]
    public void Stats_GainXP_ShouldIncrease()
    {
        var stats = new Stats();
        
        stats.GainXP(50);

        Assert.AreEqual(50, stats.CurrentXP);
    }

    [TestMethod]
    public void Stats_GainXP_ShouldLevelUp()
    {
        var stats = new Stats();
        int initialAttack = stats.BaseAttack;

        stats.GainXP(200);

        Assert.AreEqual(2, stats.Level);
        Assert.AreEqual(initialAttack + 2, stats.BaseAttack);
    }

    [TestMethod]
    public void Stats_UpdateModifiers_ShouldReduceDurations()
    {
        var stats = new Stats();
        var modifier = new StatModifier("Temp", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 2);
        stats.AddModifier(modifier);

        stats.UpdateModifiers();
        Assert.AreEqual(1, modifier.RemainingDuration);

        stats.UpdateModifiers();
        Assert.AreEqual(0, modifier.RemainingDuration);
        Assert.AreEqual(0, stats.GetActiveModifiers().Count);
    }

    [TestMethod]
    public void Stats_RemoveModifier_ShouldWork()
    {
        var stats = new Stats(100, 20, 10, 15);
        var modifier = new StatModifier("Buff", ModifiableStat.ATTACK, ModifierType.FLAT, 10, 3);
        stats.AddModifier(modifier);

        stats.RemoveModifier(modifier);

        Assert.AreEqual(20, stats.EffectiveAttack);
        Assert.AreEqual(0, stats.GetActiveModifiers().Count);
    }

    #endregion

    // === TESTS SHIP ===
    #region Tests Ship

    [TestMethod]
    public void Ship_Creation_ShouldHaveCorrectProperties()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.FEU, stats);

        Assert.AreEqual("TestShip", ship.Name);
        Assert.AreEqual(ShipType.FEU, ship.Type);
        Assert.AreEqual(100, ship.CurrentHP);
        Assert.AreEqual(100, ship.MaxHP);
        Assert.IsTrue(ship.IsAlive);
    }

    [TestMethod]
    public void Ship_TakeDamage_ShouldReduceHP()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.FEU, stats);

        ship.TakeDamage(30);

        Assert.AreEqual(70, ship.CurrentHP);
        Assert.IsTrue(ship.IsAlive);
    }

    [TestMethod]
    public void Ship_TakeDamage_ShouldDieWhenHPZero()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.FEU, stats);

        ship.TakeDamage(150);

        Assert.AreEqual(0, ship.CurrentHP);
        Assert.IsFalse(ship.IsAlive);
    }

    [TestMethod]
    public void Ship_Heal_ShouldIncreaseHP()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.FEU, stats);
        ship.TakeDamage(40);

        ship.Heal(20);

        Assert.AreEqual(80, ship.CurrentHP);
    }

    [TestMethod]
    public void Ship_Heal_ShouldNotExceedMaxHP()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.FEU, stats);
        ship.TakeDamage(30);

        ship.Heal(100);

        Assert.AreEqual(100, ship.CurrentHP);
    }

    [TestMethod]
    public void Ship_TakeDamageWithType_SuperEffective_ShouldDealMoreDamage()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.GLACE, stats);

        ship.TakeDamage(20, AttackType.FEU, ShipType.NORMAL);

        Assert.IsTrue(ship.CurrentHP < 80);
    }

    [TestMethod]
    public void Ship_Revive_ShouldRestoreHP()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.FEU, stats);
        ship.TakeDamage(200);

        ship.Revive(50);

        Assert.AreEqual(50, ship.CurrentHP);
        Assert.IsTrue(ship.IsAlive);
    }

    [TestMethod]
    public void Ship_GetEffectiveAttack_ShouldReturnCorrectValue()
    {
        var stats = new Stats(100, 20, 10, 15);
        var ship = new PlayerShip("TestShip", ShipType.FEU, stats);

        int attack = ship.GetEffectiveAttack();

        Assert.AreEqual(20, attack);
    }

    #endregion

    // === TESTS PLAYER SHIP ===
    #region Tests PlayerShip

    [TestMethod]
    public void PlayerShip_GainGold_ShouldIncrease()
    {
        var stats = new Stats();
        var player = new PlayerShip("Player", ShipType.NORMAL, stats);

        player.GainGold(100);

        Assert.AreEqual(100, player.Gold);
    }

    [TestMethod]
    public void PlayerShip_SpendGold_ShouldDecrease()
    {
        var stats = new Stats();
        var player = new PlayerShip("Player", ShipType.NORMAL, stats);
        player.GainGold(100);

        bool result = player.SpendGold(40);

        Assert.IsTrue(result);
        Assert.AreEqual(60, player.Gold);
    }

    [TestMethod]
    public void PlayerShip_SpendGold_InsufficientFunds_ShouldReturnFalse()
    {
        var stats = new Stats();
        var player = new PlayerShip("Player", ShipType.NORMAL, stats);
        player.GainGold(50);

        bool result = player.SpendGold(100);

        Assert.IsFalse(result);
        Assert.AreEqual(50, player.Gold);
    }

    #endregion

    // === TESTS ENEMY SHIP ===
    #region Tests EnemyShip

    [TestMethod]
    public void EnemyShip_Creation_ShouldHaveRewards()
    {
        var stats = new Stats();
        var enemy = new EnemyShip("Enemy", ShipType.FEU, stats, 100, 50);

        Assert.AreEqual(100, enemy.XPReward);
        Assert.AreEqual(50, enemy.GoldReward);
    }

    [TestMethod]
    public void EnemyShip_DecideAction_ShouldReturnAttack()
    {
        var stats = new Stats();
        var enemy = new EnemyShip("Enemy", ShipType.FEU, stats);

        string action = enemy.DecideAction();

        Assert.AreEqual("ATTACK", action);
    }

    #endregion

    // === TEST INTÉGRATION COMPLÈTE ===
    #region Tests Intégration

    [TestMethod]
    public void Integration_CompleteCombatScenario_ShouldWork()
    {
        var playerStats = new Stats(100, 25, 10, 20);
        var player = new PlayerShip("Hero", ShipType.FEU, playerStats);

        var enemyStats = new Stats(80, 15, 5, 15);
        var enemy = new EnemyShip("Goblin", ShipType.GLACE, enemyStats, 50, 30);

        // Tour 1: Joueur applique un buff
        var attackBuff = new StatModifier("Rage", ModifiableStat.ATTACK, ModifierType.PERCENTAGE, 50, 3);
        player.AddStatModifier(attackBuff);
        Assert.IsTrue(player.GetEffectiveAttack() > 25);

        // Tour 2: Ennemi attaque le joueur
        enemy.RollHitCheck(player);
        player.TakeDamage(15, AttackType.GLACE, ShipType.GLACE);
        Assert.IsTrue(player.IsAlive);

        // Tour 3: Joueur attaque avec STAB
        if (player.RollHitCheck(enemy))
        {
            int damage = player.GetEffectiveAttack();
            if (player.RollCriticalCheck())
            {
                damage = player.ApplyCriticalDamage(damage);
            }
            enemy.TakeDamage(damage, AttackType.FEU, ShipType.FEU);
        }

        // Tour 4-7: Combat continue et buffs expirent
        for (int i = 0; i < 4; i++)
        {
            player.Update();
        }

        Assert.AreEqual(0, player.Stats.GetActiveModifiers().Count);
    }

    #endregion
}
