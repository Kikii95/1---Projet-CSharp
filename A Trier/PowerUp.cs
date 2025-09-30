using System;

public abstract class PowerUp : Item
{
    // === CHAMPS PROTÉGÉS ===
    protected int _value;            
    protected PowerUpStat _stat;    

    // === PROPRIÉTÉS ===
    public int Value 
    { 
        get => _value; 
        protected set => _value = value; 
    }
    
    public PowerUpStat Stat 
    { 
        get => _stat; 
        protected set => _stat = value; 
    }

    // === CONSTRUCTEUR PROTÉGÉ ===
    protected PowerUp(string name, string description, char symbol, ItemCategory category, 
                     PowerUpStat stat, int value) 
        : base(name, description, symbol, category)
    {
        _stat = stat;
        _value = value;
    }

    // === ÉVÉNEMENT COMMUN ===
    public event Action<PowerUp> OnPowerUpUsed;

    // === MÉTHODE PROTÉGÉE ===
    protected void TriggerUsedEvent()
    {
        OnPowerUpUsed?.Invoke(this);
    }
}

public class TemporaryPowerUp : PowerUp
{
    // === CHAMPS PRIVÉS ===
    private int _duration;           
    private int _remainingDuration;  

    // === PROPRIÉTÉS ===
    public int Duration 
    { 
        get => _duration; 
        private set => _duration = value; 
    }
    
    public int RemainingDuration 
    { 
        get => _remainingDuration; 
        private set => _remainingDuration = value; 
    }

    public bool IsActive => _remainingDuration > 0;

    // === CONSTRUCTEUR ===
    public TemporaryPowerUp(string name, string description, PowerUpStat stat, 
                           int value, int duration) 
        : base(name, description, 'T', ItemCategory.TEMPORARY, stat, value)
    {
        _duration = duration;
        _remainingDuration = 0; 
    }

    // === IMPLÉMENTATION ABSTRAITE ===
    public override void Use()
    {
        if (IsActive)
        {
            Console.WriteLine($"⚠️ {Name} déjà actif ({_remainingDuration} tours restants)");
            return;
        }

        _remainingDuration = _duration;
        ApplyTemporaryEffect();
        
        TriggerUsedEvent();
        Console.WriteLine($"✨ {Name} activé ! +{_value} {_stat} pour {_duration} tours");
    }

    public void ReduceDuration()
    {
        if (_remainingDuration > 0)
        {
            _remainingDuration--;
            
            if (_remainingDuration <= 0)
            {
                RemoveTemporaryEffect();
                Console.WriteLine($"⏰ Effet de {Name} terminé");
            }
        }
    }

    // === MÉTHODES PRIVÉES ===
    private void ApplyTemporaryEffect()
    {
        //TODO: Intégration avec stats du joueur
        switch (_stat)
        {
            case PowerUpStat.SPEED:
                Console.WriteLine($"🏃 Vitesse augmentée de {_value}");
                break;
            case PowerUpStat.ATTACK:
                Console.WriteLine($"⚔️ Attaque augmentée de {_value}");
                break;
            case PowerUpStat.DEFENSE:
                Console.WriteLine($"🛡️ Défense augmentée de {_value}");
                break;
        }
    }

    private void RemoveTemporaryEffect()
    {
        // TODO: Retirer l'effet des stats du joueur
        Console.WriteLine($"📉 Bonus {_stat} retiré");
    }

    // === OVERRIDE AFFICHAGE ===
    public override string GetDisplayInfo()
    {
        string statusInfo = IsActive ? $"ACTIF:{_remainingDuration}" : "PRÊT";
        return $"[{Symbol}] {Name} - +{_value} {_stat} ({_duration}t) ({statusInfo})";
    }
}

public class PermanentPowerUp : PowerUp
{
    // === CHAMPS PRIVÉS ===
    private bool _isUsed;

    // === PROPRIÉTÉS ===
    public bool IsUsed 
    { 
        get => _isUsed; 
        private set => _isUsed = value; 
    }

    // === CONSTRUCTEUR ===
    public PermanentPowerUp(string name, string description, PowerUpStat stat, int value) 
        : base(name, description, 'P', ItemCategory.PERMANENT, stat, value)
    {
        _isUsed = false;
    }

    // === IMPLÉMENTATION ABSTRAITE ===
    public override void Use()
    {
        if (_isUsed)
        {
            Console.WriteLine($"⚠️ {Name} déjà utilisé définitivement");
            return;
        }

        ApplyPermanentEffect();
        _isUsed = true;
        
        TriggerUsedEvent();
        Console.WriteLine($"⭐ {Name} utilisé ! +{_value} {_stat} PERMANENT");
    }

    // === MÉTHODE PRIVÉE ===
    private void ApplyPermanentEffect()
    {
        // TODO: Intégration avec stats permanentes du joueur
        switch (_stat)
        {
            case PowerUpStat.MAX_HP:
                Console.WriteLine($"❤️ PV Max augmentés de {_value}");
                break;
            case PowerUpStat.ATTACK:
                Console.WriteLine($"⚔️ Attaque augmentée de {_value}");
                break;
            case PowerUpStat.DEFENSE:
                Console.WriteLine($"🛡️ Défense augmentée de {_value}");
                break;
            case PowerUpStat.SPEED:
                Console.WriteLine($"🏃 Vitesse augmentée de {_value}");
                break;
        }
    }

    // === OVERRIDE AFFICHAGE ===
    public override string GetDisplayInfo()
    {
        string statusInfo = _isUsed ? "UTILISÉ" : "PRÊT";
        return $"[{Symbol}] {Name} - +{_value} {_stat} PERMANENT ({statusInfo})";
    }
}

public enum PowerUpStat
{
    ATTACK,   
    DEFENSE,   
    SPEED,    
    MAX_HP   
}