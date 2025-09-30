using System;

public class Capacity : Item
{
    // === CHAMPS PRIVÉS ===
    private int _cooldown;          
    private int _currentCooldown;    
    private CapacityType _type;    

    // === PROPRIÉTÉS ===
    public int Cooldown 
    { 
        get => _cooldown; 
        private set => _cooldown = value; 
    }
    
    public int CurrentCooldown 
    { 
        get => _currentCooldown; 
        private set => _currentCooldown = value; 
    }
    
    public CapacityType Type 
    { 
        get => _type; 
        private set => _type = value; 
    }

    public bool IsReady => _currentCooldown <= 0;

    // === ÉVÉNEMENT ===
    public event Action<Capacity> OnCapacityUsed;

    // === CONSTRUCTEUR ===
    public Capacity(string name, string description, CapacityType type, int cooldown) 
        : base(name, description, 'C', ItemCategory.CAPACITY)
    {
        _type = type;
        _cooldown = cooldown;
        _currentCooldown = 0;
    }

    // === MÉTHODES PUBLIQUES ===
    public override void Use()
    {
        if (!IsReady)
        {
            Console.WriteLine($"❌ {Name} en cooldown ({_currentCooldown} tours restants)");
            return;
        }

        switch (_type)
        {
            case CapacityType.DASH:
                ExecuteDash();
                break;
            case CapacityType.SHIELD:
                ExecuteShield();
                break;
            case CapacityType.MISSILES:
                ExecuteMissiles();
                break;
            default:
                Console.WriteLine($"⚠️ Capacité {Name} non implémentée");
                break;
        }

        _currentCooldown = _cooldown;
        OnCapacityUsed?.Invoke(this);
        
        Console.WriteLine($"✅ {Name} utilisée ! Cooldown: {_cooldown} tours");
    }

    public void ReduceCooldown()
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown--;
        }
    }

    public void ResetCooldown()
    {
        _currentCooldown = 0;
    }

    // === MÉTHODES PRIVÉES D'EXÉCUTION ===
    private void ExecuteDash()
    {
        Console.WriteLine("💨 Dash activé ! Déplacement rapide disponible");
        // TODO: Intégration avec système de mouvement du Ship
    }

    private void ExecuteShield()
    {
        Console.WriteLine("🛡️ Shield activé ! Protection temporaire");
        // TODO: Intégration avec système de santé IHealth
    }

    private void ExecuteMissiles()
    {
        Console.WriteLine("🚀 Missiles lancés ! Attaque en zone");
        // TODO: Intégration avec système de projectiles
    }

    // === OVERRIDE AFFICHAGE ===
    public override string GetDisplayInfo()
    {
        string cooldownInfo = IsReady ? "PRÊT" : $"CD:{_currentCooldown}";
        return $"[{Symbol}] {Name} - {Description} ({cooldownInfo})";
    }
}

public enum CapacityType
{
    DASH,      
    SHIELD,    
    MISSILES   
}