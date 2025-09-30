using System;

public abstract class Ship : IHealth
{
    // === CHAMPS PRIVÉS ===
    private string _name;
    private ShipType _type;
    private Stats _stats;
    private int _currentHP;

    // === PROPRIÉTÉS ===
    public string Name { get => _name; protected set => _name = value; }
    public ShipType Type { get => _type; protected set => _type = value; }
    public Stats Stats { get => _stats; }

    // === IMPLÉMENTATION IHealth ===
    public int CurrentHP { get => _currentHP; }
    public int MaxHP => _stats.EffectiveMaxHP;
    public bool IsAlive => _currentHP > 0;
    public float HPPercentage => MaxHP > 0 ? (float)_currentHP / MaxHP : 0;

    // === ÉVÉNEMENTS IHealth ===
    public event Action<int> OnDamageTaken;
    public event Action<int> OnHealed;
    public event Action OnDeath;
    public event Action<int, int> OnHPChanged;

    // === ÉVÉNEMENTS SUPPLÉMENTAIRES ===
    public event Action<int> OnAttackPerformed;
    public event Action<bool> OnAttackResult; 

    // === CONSTRUCTEUR PROTÉGÉ ===
    protected Ship(string name, ShipType type, Stats stats)
    {
        _name = name;
        _type = type;
        _stats = stats;
        _currentHP = _stats.EffectiveMaxHP;

        _stats.OnLevelUp += OnLevelUpHandler;
    }

    // === MÉTHODES IHealth ===
    public virtual void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0)
        {
            return;
        }

        int oldHP = _currentHP;
        _currentHP -= amount;

        if (_currentHP < 0)
        {
            _currentHP = 0;
        }

        OnDamageTaken?.Invoke(amount);
        OnHPChanged?.Invoke(oldHP, _currentHP);
        Console.WriteLine($"💥 {_name} prend {amount} dégâts ! ({_currentHP}/{MaxHP} HP)");

        if (!IsAlive)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        if (!IsAlive || amount <= 0)
        {
            return;
        }

        int oldHP = _currentHP;
        _currentHP += amount;

        if (_currentHP > MaxHP)
        {
            _currentHP = MaxHP;
        }

        OnHealed?.Invoke(amount);
        OnHPChanged?.Invoke(oldHP, _currentHP);
        Console.WriteLine($"💚 {_name} récupère {amount} HP ! ({_currentHP}/{MaxHP} HP)");
    }

    public virtual void Die()
    {
        if (!IsAlive)
        {
            return;
        }

        _currentHP = 0;
        OnDeath?.Invoke();
        Console.WriteLine($"💀 {_name} est K.O. !");
    }

    public virtual void Revive(int hpAmount)
    {
        if (IsAlive)
        {
            Console.WriteLine($"⚠️ {_name} est déjà en vie");
            return;
        }

        _currentHP = Math.Min(hpAmount, MaxHP);
        Console.WriteLine($"✨ {_name} est réanimé avec {_currentHP} HP !");
    }

    // === MÉTHODES PUBLIQUES - COMBAT ===
    public void TakeDamage(int baseDamage, AttackType attackType, ShipType attackerType)
    {
        if (!IsAlive)
        {
            return;
        }

        float typeMultiplier = TypeEffectiveness.GetTotalMultiplier(attackType, attackerType, _type);
        
        int finalDamage = CalculateDamageWithDefense(baseDamage, typeMultiplier);

        string effectivenessMsg = TypeEffectiveness.GetEffectivenessMessage(typeMultiplier);
        if (!string.IsNullOrEmpty(effectivenessMsg))
        {
            Console.WriteLine(effectivenessMsg);
        }

        TakeDamage(finalDamage);
    }

    public int GetEffectiveAttack()
    {
        return _stats.EffectiveAttack;
    }

    public bool RollHitCheck(Ship target)
    {
        Random random = new Random();
        int hitRoll = random.Next(0, 100);
        int hitChance = _stats.EffectivePrecision - target.Stats.EffectiveEvasion;
        hitChance = Math.Clamp(hitChance, 5, 100); 

        bool hit = hitRoll < hitChance;
        OnAttackResult?.Invoke(hit);

        if (hit)
        {
            Console.WriteLine($"🎯 Attaque réussie ! ({hitChance}% de chances)");
        }
        else
        {
            Console.WriteLine($"💨 Attaque esquivée ! ({hitChance}% de chances)");
        }

        return hit;
    }

    public bool RollCriticalCheck()
    {
        Random random = new Random();
        int critRoll = random.Next(0, 100);
        bool isCritical = critRoll < _stats.EffectiveCriticalRate;

        if (isCritical)
        {
            Console.WriteLine($"⚡ COUP CRITIQUE !");
        }

        return isCritical;
    }

    public int ApplyCriticalDamage(int baseDamage)
    {
        return (int)(baseDamage * (_stats.EffectiveCriticalDamage / 100f));
    }

    // === MÉTHODES PUBLIQUES - GESTION ===
    public virtual void Update()
    {
        _stats.UpdateModifiers();
    }

    public void GainXP(int amount)
    {
        _stats.GainXP(amount);
    }

    public void AddStatModifier(StatModifier modifier)
    {
        _stats.AddModifier(modifier);
    }

    public void RemoveStatModifier(StatModifier modifier)
    {
        _stats.RemoveModifier(modifier);
    }

    // === MÉTHODES PRIVÉES ===
    private int CalculateDamageWithDefense(int baseDamage, float typeMultiplier)
    {
        float rawDamage = baseDamage * typeMultiplier;
        int finalDamage = (int)(rawDamage - (_stats.EffectiveDefense / 2f));

        return Math.Max(1, finalDamage);
    }

    private void OnLevelUpHandler(int newLevel)
    {
        int oldMaxHP = MaxHP;
        int hpGained = MaxHP - _currentHP;
        _currentHP = MaxHP;
        Console.WriteLine($"💚 HP restaurés au maximum ! +{hpGained} HP");
    }

    // === MÉTHODES VIRTUELLES POUR LES CLASSES ENFANTS ===
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"\n=== 🚀 {_name} ===");
        Console.WriteLine($"Type: {_type}");
        Console.WriteLine($"HP: {_currentHP}/{MaxHP} ({HPPercentage:P0})");
        Console.WriteLine($"État: {(IsAlive ? "En vie" : "K.O.")}");
        _stats.DisplayStats();
    }

    public override string ToString()
    {
        return $"{_name} ({_type}) - Niv.{_stats.Level} - {_currentHP}/{MaxHP} HP";
    }
}
