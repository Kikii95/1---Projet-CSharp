using System;

public enum ModifierType
{
    FLAT,      
    PERCENTAGE  
}

public enum ModifiableStat
{
    ATTACK,
    DEFENSE,
    SPEED,
    MAX_HP,
    PRECISION,
    EVASION,
    CRITICAL_RATE,
    CRITICAL_DAMAGE
}

public class StatModifier
{
    // === CHAMPS PRIVÉS ===
    private string _name;
    private ModifiableStat _stat;
    private ModifierType _type;
    private float _value;
    private int _duration;
    private int _remainingDuration;
    private bool _isPermanent;

    // === PROPRIÉTÉS ===
    public string Name { get => _name; }
    public ModifiableStat Stat { get => _stat; }
    public ModifierType Type { get => _type; }
    public float Value { get => _value; }
    public int Duration { get => _duration; }
    public int RemainingDuration { get => _remainingDuration; }
    public bool IsPermanent { get => _isPermanent; }
    public bool IsActive => _isPermanent || _remainingDuration > 0;

    // === ÉVÉNEMENT ===
    public event Action<StatModifier> OnExpired;

    // === CONSTRUCTEUR TEMPORAIRE ===
    public StatModifier(string name, ModifiableStat stat, ModifierType type, float value, int duration)
    {
        _name = name;
        _stat = stat;
        _type = type;
        _value = value;
        _duration = duration;
        _remainingDuration = duration;
        _isPermanent = false;
    }

    // === CONSTRUCTEUR PERMANENT ===
    public StatModifier(string name, ModifiableStat stat, ModifierType type, float value)
    {
        _name = name;
        _stat = stat;
        _type = type;
        _value = value;
        _duration = -1;
        _remainingDuration = -1;
        _isPermanent = true;
    }

    // === MÉTHODES PUBLIQUES ===
    public void ReduceDuration()
    {
        if (_isPermanent)
        {
            return;
        }

        if (_remainingDuration > 0)
        {
            _remainingDuration--;

            if (_remainingDuration <= 0)
            {
                OnExpired?.Invoke(this);
                Console.WriteLine($"⏰ Modificateur {_name} expiré");
            }
        }
    }

    public void ResetDuration()
    {
        if (!_isPermanent)
        {
            _remainingDuration = _duration;
        }
    }

    public float Apply(float baseValue)
    {
        if (!IsActive)
        {
            return baseValue;
        }

        if (_type == ModifierType.FLAT)
        {
            return baseValue + _value;
        }
        else 
        {
            return baseValue * (1 + _value / 100f);
        }
    }

    public override string ToString()
    {
        string modifierSign = _value >= 0 ? "+" : "";
        string modifierValue = _type == ModifierType.FLAT 
            ? $"{modifierSign}{_value}" 
            : $"{modifierSign}{_value}%";

        string durationInfo = _isPermanent 
            ? "PERMANENT" 
            : $"{_remainingDuration}t";

        return $"{_name}: {modifierValue} {_stat} ({durationInfo})";
    }
}
