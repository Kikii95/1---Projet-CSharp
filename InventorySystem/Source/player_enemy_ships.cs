using System;

public class PlayerShip : Ship
{
    // === CHAMPS PRIVÉS SPÉCIFIQUES AU JOUEUR ===
    private int _gold;

    // === PROPRIÉTÉS ===
    public int Gold 
    { 
        get => _gold; 
        private set => _gold = value >= 0 ? value : 0; 
    }

    // === ÉVÉNEMENTS SPÉCIFIQUES ===
    public event Action<int> OnGoldGained;
    public event Action<int> OnGoldSpent;

    // === CONSTRUCTEUR ===
    public PlayerShip(string name, ShipType type, Stats stats) 
        : base(name, type, stats)
    {
        _gold = 0;
    }

    // === MÉTHODES SPÉCIFIQUES AU JOUEUR ===
    public void GainGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _gold += amount;
        OnGoldGained?.Invoke(amount);
        Console.WriteLine($"💰 +{amount} Or (Total: {_gold})");
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("⚠️ Montant invalide");
            return false;
        }

        if (_gold < amount)
        {
            Console.WriteLine($"⚠️ Or insuffisant (Besoin: {amount}, Disponible: {_gold})");
            return false;
        }

        _gold -= amount;
        OnGoldSpent?.Invoke(amount);
        Console.WriteLine($"💸 -{amount} Or (Reste: {_gold})");
        return true;
    }

    // === OVERRIDE ===
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"💰 Or: {_gold}");
    }
}

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
