using System;

public abstract class Item
{
    // === CHAMPS PRIVÉS ===
    private string _name;
    private string _description;
    private char _symbol;
    private ItemCategory _category;

    // === PROPRIÉTÉS PUBLIQUES ===
    public string Name 
    { 
        get => _name; 
        protected set => _name = value; 
    }
    
    public string Description 
    { 
        get => _description; 
        protected set => _description = value; 
    }
    
    public char Symbol 
    { 
        get => _symbol; 
        protected set => _symbol = value; 
    }
    
    public ItemCategory Category 
    { 
        get => _category; 
        protected set => _category = value; 
    }

    // === CONSTRUCTEUR ===
    protected Item(string name, string description, char symbol, ItemCategory category)
    {
        _name = name;
        _description = description;
        _symbol = symbol;
        _category = category;
    }

    // === MÉTHODE ===
    public abstract void Use();

    public virtual string GetDisplayInfo()
    {
        return $"[{_symbol}] {_name} - {_description}";
    }

    public override string ToString()
    {
        return $"{_category}: {_name}";
    }
}

public enum ItemCategory
{
    CAPACITY,   
    TEMPORARY, 
    PERMANENT   
}