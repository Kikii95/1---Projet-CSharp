using System;

public class EnemyShip : Ship
{
    // === CHAMPS PRIVÉS SPÉCIFIQUES AUX ENNEMIS ===
    private int _xpReward;
    private int _goldReward;

    // === PROPRIÉTÉS ===
    public int XPReward { get => _xpReward; }
    public int GoldReward { get => _goldReward; }

    // === CONSTRUCTEUR ===
    public EnemyShip(string name, ShipType type, Stats stats, int xpReward = 50, int goldReward = 20) 
        : base(name, type, stats)
    {
        _xpReward = xpReward;
        _goldReward = goldReward;
    }

    // === MÉTHODES SPÉCIFIQUES AUX ENNEMIS ===
    public virtual string DecideAction()
    {
        // TODO: Ajouter plus de stratégies
        return "ATTACK";
    }

    // === OVERRIDE ===
    public override void Die()
    {
        base.Die();
        Console.WriteLine($"🎁 Récompenses: {_xpReward} XP, {_goldReward} Or");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"🎁 Récompenses: {_xpReward} XP | {_goldReward} Or");
    }
}
