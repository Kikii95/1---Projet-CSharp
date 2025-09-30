using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    // === SINGLETON PATTERN ===
    private static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Inventory();
            return _instance;
        }
    }

    // === CHAMPS PRIVÉS ===
    private Dictionary<Item, int> _items;          
    private List<TemporaryPowerUp> _activeEffects;  

    public int TotalItems => _items.Values.Sum();
    public int UniqueItems => _items.Count;

    public event Action<Item, int> OnItemAdded;     
    public event Action<Item, int> OnItemRemoved;  
    public event Action<Item> OnItemUsed;           

    // === CONSTRUCTEUR PRIVÉ ===
    private Inventory()
    {
        _items = new Dictionary<Item, int>();
        _activeEffects = new List<TemporaryPowerUp>();
    }

    // === MÉTHODES PUBLIQUES DE GESTION ===
    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            Console.WriteLine("❌ Item invalide ou quantité négative");
            return;
        }

        if (_items.ContainsKey(item))
        {
            _items[item] += quantity;
        }
        else
        {
            _items[item] = quantity;
        }

        OnItemAdded?.Invoke(item, quantity);
        Console.WriteLine($"📦 Ajouté: {quantity}x {item.Name}");
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (!_items.ContainsKey(item))
        {
            Console.WriteLine($"❌ {item.Name} non trouvé dans l'inventaire");
            return false;
        }

        if (_items[item] < quantity)
        {
            Console.WriteLine($"❌ Quantité insuffisante de {item.Name} ({_items[item]} disponibles)");
            return false;
        }

        _items[item] -= quantity;
        
        if (_items[item] <= 0)
        {
            _items.Remove(item);
        }

        OnItemRemoved?.Invoke(item, quantity);
        Console.WriteLine($"📤 Retiré: {quantity}x {item.Name}");
        return true;
    }

    public bool UseItem(Item item)
    {
        if (!_items.ContainsKey(item))
        {
            Console.WriteLine($"❌ {item.Name} non disponible");
            return false;
        }

        item.Use();
        OnItemUsed?.Invoke(item);

        if (item is TemporaryPowerUp tempPowerUp && tempPowerUp.IsActive)
        {
            if (!_activeEffects.Contains(tempPowerUp))
            {
                _activeEffects.Add(tempPowerUp);
            }
        }

        if (item is PermanentPowerUp permPowerUp && permPowerUp.IsUsed)
        {
            RemoveItem(item);
        }

        return true;
    }

    public void UpdateTemporaryEffects()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            _activeEffects[i].ReduceDuration();
            
            if (!_activeEffects[i].IsActive)
            {
                _activeEffects.RemoveAt(i);
            }
        }

        foreach (var itemPair in _items)
        {
            if (itemPair.Key is Capacity capacity)
            {
                capacity.ReduceCooldown();
            }
        }
    }

    // === MÉTHODES DE CONSULTATION ===
    public bool HasItem(Item item)
    {
        return _items.ContainsKey(item);
    }

    public int GetItemQuantity(Item item)
    {
        return _items.ContainsKey(item) ? _items[item] : 0;
    }

    public List<Item> GetItemsByCategory(ItemCategory category)
    {
        return _items.Keys.Where(item => item.Category == category).ToList();
    }

    public Dictionary<Item, int> GetAllItems()
    {
        return new Dictionary<Item, int>(_items); 
    }

    // === MÉTHODES D'AFFICHAGE ===
    public void DisplayInventory()
    {
        Console.WriteLine("\n=== 🎒 INVENTAIRE ===");
        
        if (_items.Count == 0)
        {
            Console.WriteLine("Inventaire vide");
            return;
        }

        foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
        {
            var categoryItems = GetItemsByCategory(category);
            if (categoryItems.Count > 0)
            {
                Console.WriteLine($"\n--- {category} ---");
                foreach (var item in categoryItems)
                {
                    int quantity = _items[item];
                    string quantityText = quantity > 1 ? $" x{quantity}" : "";
                    Console.WriteLine($"  {item.GetDisplayInfo()}{quantityText}");
                }
            }
        }

        if (_activeEffects.Count > 0)
        {
            Console.WriteLine("\n--- EFFETS ACTIFS ---");
            foreach (var effect in _activeEffects)
            {
                Console.WriteLine($"  ✨ {effect.Name} ({effect.RemainingDuration} tours)");
            }
        }

        Console.WriteLine($"\nTotal: {TotalItems} items ({UniqueItems} uniques)");
    }

    public void DisplaySummary()
    {
        Console.WriteLine($"🎒 Inventaire: {TotalItems} items | Actifs: {_activeEffects.Count}");
    }

    // === MÉTHODES DE DEBUG ===
    public void ClearInventory()
    {
        _items.Clear();
        _activeEffects.Clear();
        Console.WriteLine("🗑️ Inventaire vidé");
    }

    public void AddDebugItems()
    {
        var dash = new Capacity("Super Dash", "Déplacement ultra-rapide", CapacityType.DASH, 3);
        var shield = new Capacity("Energy Shield", "Bouclier énergétique", CapacityType.SHIELD, 5);
        var missiles = new Capacity("Homing Missiles", "Missiles à tête chercheuse", CapacityType.MISSILES, 4);

        var speedBoost = new TemporaryPowerUp("Speed Boost", "Vitesse x2", PowerUpStat.SPEED, 5, 10);
        var attackBoost = new TemporaryPowerUp("Attack Boost", "Attaque doublée", PowerUpStat.ATTACK, 10, 5);

        var hpUpgrade = new PermanentPowerUp("HP Upgrade", "PV Max +20", PowerUpStat.MAX_HP, 20);
        var attackUpgrade = new PermanentPowerUp("Attack Upgrade", "Attaque +5", PowerUpStat.ATTACK, 5);

        AddItem(dash);
        AddItem(shield);
        AddItem(missiles);
        AddItem(speedBoost, 3);
        AddItem(attackBoost, 2);
        AddItem(hpUpgrade);
        AddItem(attackUpgrade);

        Console.WriteLine("🧪 Items de debug ajoutés");
    }
}