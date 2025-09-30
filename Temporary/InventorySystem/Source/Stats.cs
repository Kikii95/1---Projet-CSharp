using System;
using System.Collections.Generic;
using System.Linq;

public class Stats
{
    // === CHAMPS PRIVÉS - STATS DE BASE ===
    private int _baseMaxHP;
    private int _baseAttack;
    private int _baseDefense;
    private int _baseSpeed;
    private int _basePrecision;      
    private int _baseEvasion;      
    private int _baseCriticalRate;   
    private int _baseCriticalDamage; 

    // === CHAMPS PRIVÉS - NIVEAU ET XP ===
    private int _level;
    private int _currentXP;
    private int _xpToNextLevel;

    // === LISTE DES MODIFICATEURS ACTIFS ===
    private List<StatModifier> _modifiers;

    // === PROPRIÉTÉS - STATS DE BASE ===
    public int BaseMaxHP 
    { 
        get => _baseMaxHP; 
        set => _baseMaxHP = value > 0 ? value : 1; 
    }
    
    public int BaseAttack 
    { 
        get => _baseAttack; 
        set => _baseAttack = value >= 0 ? value : 0; 
    }
    
    public int BaseDefense 
    { 
        get => _baseDefense; 
        set => _baseDefense = value >= 0 ? value : 0; 
    }
    
    public int BaseSpeed 
    { 
        get => _baseSpeed; 
        set => _baseSpeed = value >= 0 ? value : 0; 
    }
    
    public int BasePrecision 
    { 
        get => _basePrecision; 
        set => _basePrecision = Math.Clamp(value, 0, 100); 
    }
    
    public int BaseEvasion 
    { 
        get => _baseEvasion; 
        set => _baseEvasion = Math.Clamp(value, 0, 100); 
    }
    
    public int BaseCriticalRate 
    { 
        get => _baseCriticalRate; 
        set => _baseCriticalRate = Math.Clamp(value, 0, 100); 
    }
    
    public int BaseCriticalDamage 
    { 
        get => _baseCriticalDamage; 
        set => _baseCriticalDamage = value >= 100 ? value : 100; 
    }

    // === PROPRIÉTÉS - NIVEAU ET XP ===
    public int Level { get => _level; }
    public int CurrentXP { get => _currentXP; }
    public int XPToNextLevel { get => _xpToNextLevel; }

    // === PROPRIÉTÉS - STATS EFFECTIVES (avec modificateurs) ===
    public int EffectiveMaxHP => CalculateEffectiveStat(_baseMaxHP, ModifiableStat.MAX_HP);
    public int EffectiveAttack => CalculateEffectiveStat(_baseAttack, ModifiableStat.ATTACK);
    public int EffectiveDefense => CalculateEffectiveStat(_baseDefense, ModifiableStat.DEFENSE);
    public int EffectiveSpeed => CalculateEffectiveStat(_baseSpeed, ModifiableStat.SPEED);
    public int EffectivePrecision => Math.Clamp(CalculateEffectiveStat(_basePrecision, ModifiableStat.PRECISION), 0, 100);
    public int EffectiveEvasion => Math.Clamp(CalculateEffectiveStat(_baseEvasion, ModifiableStat.EVASION), 0, 100);
    public int EffectiveCriticalRate => Math.Clamp(CalculateEffectiveStat(_baseCriticalRate, ModifiableStat.CRITICAL_RATE), 0, 100);
    public int EffectiveCriticalDamage => Math.Max(100, CalculateEffectiveStat(_baseCriticalDamage, ModifiableStat.CRITICAL_DAMAGE));

    // === ÉVÉNEMENTS ===
    public event Action<int> OnLevelUp;
    public event Action<int> OnXPGained;
    public event Action<StatModifier> OnModifierAdded;
    public event Action<StatModifier> OnModifierRemoved;

    // === CONSTRUCTEUR ===
    public Stats(int baseMaxHP = 100, int baseAttack = 10, int baseDefense = 5, int baseSpeed = 10)
    {
        _baseMaxHP = baseMaxHP;
        _baseAttack = baseAttack;
        _baseDefense = baseDefense;
        _baseSpeed = baseSpeed;
        _basePrecision = 95;
        _baseEvasion = 5;
        _baseCriticalRate = 5;
        _baseCriticalDamage = 150;

        _level = 1;
        _currentXP = 0;
        _xpToNextLevel = CalculateXPForNextLevel();

        _modifiers = new List<StatModifier>();
    }

    // === MÉTHODES PUBLIQUES - GESTION XP ET NIVEAU ===
    public void GainXP(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _currentXP += amount;
        OnXPGained?.Invoke(amount);
        Console.WriteLine($"💎 +{amount} XP (Total: {_currentXP}/{_xpToNextLevel})");

        while (_currentXP >= _xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _currentXP -= _xpToNextLevel;
        _level++;

        _baseMaxHP += 10;
        _baseAttack += 2;
        _baseDefense += 1;
        _baseSpeed += 1;

        _xpToNextLevel = CalculateXPForNextLevel();

        OnLevelUp?.Invoke(_level);
        Console.WriteLine($"⭐ NIVEAU SUPÉRIEUR ! Niveau {_level}");
        Console.WriteLine($"   HP Max: +10 | Attaque: +2 | Défense: +1 | Vitesse: +1");
    }

    private int CalculateXPForNextLevel()
    {
        return (int)(100 * Math.Pow(_level, 1.5f));
    }

    // === MÉTHODES PUBLIQUES - GESTION DES MODIFICATEURS ===
    public void AddModifier(StatModifier modifier)
    {
        if (modifier == null)
        {
            return;
        }

        _modifiers.Add(modifier);
        OnModifierAdded?.Invoke(modifier);

        string sign = modifier.Value >= 0 ? "+" : "";
        string typeStr = modifier.Type == ModifierType.FLAT ? "" : "%";
        Console.WriteLine($"✨ Modificateur ajouté: {modifier.Name} ({sign}{modifier.Value}{typeStr} {modifier.Stat})");
    }
    public void RemoveModifier(StatModifier modifier)
    {
        if (_modifiers.Remove(modifier))
        {
            OnModifierRemoved?.Invoke(modifier);
            Console.WriteLine($"🔻 Modificateur retiré: {modifier.Name}");
        }
    }
    public void RemoveModifiersOfStat(ModifiableStat stat)
    {
        _modifiers.RemoveAll(m => m.Stat == stat);
    }

    public void ClearAllModifiers()
    {
        _modifiers.Clear();
        Console.WriteLine("🗑️ Tous les modificateurs retirés");
    }

    public void UpdateModifiers()
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            _modifiers[i].ReduceDuration();

            if (!_modifiers[i].IsActive)
            {
                _modifiers.RemoveAt(i);
            }
        }
    }

    public List<StatModifier> GetActiveModifiers()
    {
        return new List<StatModifier>(_modifiers);
    }

    public List<StatModifier> GetModifiersForStat(ModifiableStat stat)
    {
        return _modifiers.Where(m => m.Stat == stat && m.IsActive).ToList();
    }

    // === MÉTHODES PRIVÉES - CALCUL DES STATS EFFECTIVES ===
    private int CalculateEffectiveStat(int baseStat, ModifiableStat statType)
    {
        float effectiveStat = baseStat;

        var modifiers = GetModifiersForStat(statType);
        
        foreach (var mod in modifiers.Where(m => m.Type == ModifierType.FLAT))
        {
            effectiveStat = mod.Apply(effectiveStat);
        }

        foreach (var mod in modifiers.Where(m => m.Type == ModifierType.PERCENTAGE))
        {
            effectiveStat = mod.Apply(effectiveStat);
        }

        return (int)effectiveStat;
    }

    // === MÉTHODES D'AFFICHAGE ===
    public void DisplayStats()
    {
        Console.WriteLine($"\n=== 📊 STATISTIQUES - NIVEAU {_level} ===");
        Console.WriteLine($"XP: {_currentXP}/{_xpToNextLevel}");
        Console.WriteLine($"HP Max:     {_baseMaxHP} → {EffectiveMaxHP}");
        Console.WriteLine($"Attaque:    {_baseAttack} → {EffectiveAttack}");
        Console.WriteLine($"Défense:    {_baseDefense} → {EffectiveDefense}");
        Console.WriteLine($"Vitesse:    {_baseSpeed} → {EffectiveSpeed}");
        Console.WriteLine($"Précision:  {_basePrecision}% → {EffectivePrecision}%");
        Console.WriteLine($"Esquive:    {_baseEvasion}% → {EffectiveEvasion}%");
        Console.WriteLine($"Taux Crit:  {_baseCriticalRate}% → {EffectiveCriticalRate}%");
        Console.WriteLine($"Dégât Crit: {_baseCriticalDamage}% → {EffectiveCriticalDamage}%");

        if (_modifiers.Count > 0)
        {
            Console.WriteLine($"\n--- MODIFICATEURS ACTIFS ({_modifiers.Count}) ---");
            foreach (var mod in _modifiers)
            {
                Console.WriteLine($"  {mod}");
            }
        }
    }
}
