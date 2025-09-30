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
