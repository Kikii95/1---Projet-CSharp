# 🎒 Système d'Inventaire - Projet C# POO

## ✅ Projet Livré et Testé

**Status**: ✅ **PRÊT POUR INTÉGRATION**
- **18/18 tests unitaires** passent ✅
- **Compilation** sans erreurs ✅  
- **Démonstration interactive** fonctionnelle ✅
- **Architecture POO** selon cours GT3 ✅

## 🚀 Utilisation Rapide

### 1. Ouvrir dans Visual Studio
```
Fichier > Ouvrir > Projet/Solution
Sélectionner: InventorySystem.sln
```

### 2. Exécuter la Démonstration
```
Clic droit sur "InventorySystemTests" > Définir comme projet de démarrage
F5 ou Démarrer
```

### 3. Lancer les Tests Unitaires
```
Test > Explorateur de tests > Exécuter tous les tests
Ou : Ctrl + R, A
```

## 📋 Architecture Implémentée

### Classes Principales
```
Item (abstraite)
├── Capacity (cooldown system)
└── PowerUp (abstraite)
    ├── TemporaryPowerUp (durée limitée)
    └── PermanentPowerUp (usage unique)

Inventory (singleton)
```

### Fonctionnalités Validées
- ✅ **Pattern Singleton** : `Inventory.Instance`
- ✅ **Héritage POO** : Item → Capacity/PowerUp → Temp/Perm
- ✅ **Events** : `OnItemAdded`, `OnItemUsed`, `OnCapacityUsed`
- ✅ **Cooldown System** : capacités avec recharge
- ✅ **Durée Temporaire** : PowerUps limités dans le temps
- ✅ **Usage Unique** : PowerUps permanents consommés
- ✅ **Stack Items** : quantités multiples d'items identiques
- ✅ **Catégories** : CAPACITY, TEMPORARY, PERMANENT
- ✅ **Edge Cases** : protection contre erreurs/null

## 🎮 Exemples d'Usage

### Créer des Items
```csharp
// Capacités
var dash = new Capacity("Super Dash", "Déplacement rapide", CapacityType.DASH, 3);
var shield = new Capacity("Energy Shield", "Protection", CapacityType.SHIELD, 5);

// PowerUps temporaires
var speedBoost = new TemporaryPowerUp("Speed Boost", "Vitesse x2", PowerUpStat.SPEED, 5, 10);
var attackBoost = new TemporaryPowerUp("Attack Boost", "Attaque x2", PowerUpStat.ATTACK, 10, 5);

// PowerUps permanents  
var hpUpgrade = new PermanentPowerUp("HP Upgrade", "PV Max +20", PowerUpStat.MAX_HP, 20);
var attackUpgrade = new PermanentPowerUp("Attack Upgrade", "Attaque +5", PowerUpStat.ATTACK, 5);
```

### Utiliser l'Inventaire
```csharp
var inventory = Inventory.Instance;

// Ajouter items
inventory.AddItem(dash);
inventory.AddItem(speedBoost, 3); // 3 exemplaires

// Utiliser items
inventory.UseItem(dash);        // Active cooldown
inventory.UseItem(speedBoost);  // Active effet temporaire
inventory.UseItem(hpUpgrade);   // Consomme l'item

// Gérer le temps (à appeler chaque tour de jeu)
inventory.UpdateTemporaryEffects(); // Réduit cooldowns + durées

// Consulter inventaire
inventory.DisplayInventory();           // Affichage complet
inventory.DisplaySummary();             // Résumé
bool hasItem = inventory.HasItem(dash); // Vérification
int quantity = inventory.GetItemQuantity(speedBoost); // Quantité
```

### Intégration avec Votre Code
```csharp
// Dans votre Ship ou GameObject
public class PlayerShip 
{
    private Inventory _inventory;
    
    public void Start()
    {
        _inventory = Inventory.Instance;
        
        // S'abonner aux événements
        _inventory.OnItemUsed += OnPlayerUsedItem;
    }
    
    private void OnPlayerUsedItem(Item item)
    {
        if (item is Capacity capacity)
        {
            // Appliquer effet de capacité sur le ship
            ApplyCapacityEffect(capacity);
        }
        else if (item is TemporaryPowerUp tempPowerUp)
        {
            // Appliquer buff temporaire sur stats
            ApplyTemporaryBonus(tempPowerUp);
        }
        // etc.
    }
    
    public void Update() // Chaque tour/frame
    {
        _inventory.UpdateTemporaryEffects();
    }
}
```

## 🧪 Tests Unitaires

### 18 Tests Automatisés
- **3 tests** création items (propriétés, constructeurs)
- **3 tests** capacités (cooldown, usage, réduction)
- **2 tests** PowerUps temporaires (activation, durée)
- **2 tests** PowerUps permanents (usage unique, protection)
- **3 tests** Inventory singleton (instance, ajout, stack)
- **3 tests** edge cases (null, quantités, erreurs)
- **2 tests** intégration (workflow complet, catégories)

### Comment Ajouter des Tests
```csharp
[TestMethod]
public void MonNouveauTest_ShouldWork()
{
    // Arrange
    var inventory = Inventory.Instance;
    var item = new Capacity("Test", "Test", CapacityType.DASH, 2);
    
    // Act
    inventory.AddItem(item);
    bool result = inventory.UseItem(item);
    
    // Assert
    Assert.IsTrue(result);
    Assert.IsFalse(item.IsReady); // En cooldown
}
```

## 🔗 Points d'Intégration Équipe

### TODO pour Connexion avec Autres Composants

1. **Ship Integration** (`Ship.cs`)
```csharp
// Remplacer TODO dans Capacity.cs
private void ExecuteDash() 
{
    // TODO: Intégration avec système de mouvement du Ship
    ship.ApplySpeedBoost(2.0f, 3); // exemple
}
```

2. **Stats System** (`PlayerStats.cs`)
```csharp
// Remplacer TODO dans PowerUp.cs  
private void ApplyTemporaryEffect()
{
    // TODO: Intégration avec stats du joueur
    PlayerStats.Instance.ModifyStat(_stat, _value, _duration);
}
```

3. **UI Integration** (`Canvas.cs`)
```csharp
// Affichage inventaire in-game
inventory.OnItemAdded += (item, quantity) => RefreshUI();
inventory.OnItemUsed += (item) => ShowItemUsedEffect(item);
```

## 📁 Structure des Fichiers

```
InventorySystem/
├── InventorySystem.sln           # Solution Visual Studio
├── README.md                     # Ce guide
├── Source/                       # Code source
│   ├── InventorySystem.csproj   # Projet library
│   ├── Item.cs                  # Classe abstraite de base
│   ├── Capacity.cs              # Capacités avec cooldown
│   ├── PowerUp.cs               # PowerUps temporaires/permanents
│   └── Inventory.cs             # Singleton de gestion
└── Tests/                       # Tests et démos
    ├── InventorySystemTests.csproj # Projet de tests
    ├── InventorySystemTests.cs     # Tests unitaires fusionnés
    └── Program.cs                  # Point d'entrée démo
```

## 🎯 Prochaines Étapes

Votre système d'inventaire est **100% fonctionnel et testé**. Vous pouvez maintenant :

1. **Intégrer avec vos Ship/GameObject** existants
2. **Connecter aux stats** du joueur  
3. **Ajouter UI** pour affichage in-game
4. **Étendre avec nouveaux types** d'items si besoin

**Le code respecte strictement votre cours POO** : classes, héritage, polymorphisme, encapsulation, events, singleton, tests unitaires.

---
**Développé selon cours GT3 - Kévin Roussel**  
**Architecture POO + Tests Unitaires**