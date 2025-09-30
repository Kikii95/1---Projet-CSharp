# 🧪 Guide Tests Unitaires - Système Inventory

## 📋 Setup Tests Unitaires

### 1. Prérequis Visual Studio
- **Visual Studio 2022** avec module "Développement .NET Desktop"
- **MSTest Framework** (intégré par défaut)

### 2. Création Projet Test
```bash
# Dans Visual Studio :
# Fichier > Nouveau > Projet > "Projet de test unitaire (.NET Framework)"
# Nom : "InventoryTestProject"
```

### 3. Références Nécessaires
- Ajouter références vers vos fichiers source :
  - `Item.cs`
  - `Capacity.cs` 
  - `PowerUp.cs`
  - `Inventory.cs`

## 🚀 Exécution des Tests

### Via Visual Studio
1. **Menu Test** > **Fenêtre** > **Explorateur de tests**
2. **Clic droit** sur `InventoryTests.cs` > **Exécuter les tests**
3. **Tous les tests** : `Ctrl + R, A`

### Via Console
```bash
# Naviguer vers le dossier du projet
cd "C:\Users\killi\source\Année 3\1 - Projet C#"

# Compiler et exécuter tests
dotnet test
```

## 📊 Tests Implémentés

### ✅ Tests de Base (Item Creation)
- `Item_CreationCapacity_ShouldHaveCorrectProperties`
- `Item_CreationTemporaryPowerUp_ShouldHaveCorrectProperties`
- `Item_CreationPermanentPowerUp_ShouldHaveCorrectProperties`

**Valide** : Constructeurs, propriétés, valeurs par défaut

### ✅ Tests Capacity (Cooldown System)
- `Capacity_Use_ShouldSetCooldown`
- `Capacity_UseInCooldown_ShouldNotWork`
- `Capacity_ReduceCooldown_ShouldWorkCorrectly`
- `Capacity_ResetCooldown_ShouldWork`

**Valide** : Système de cooldown, usage multiple, réduction temporelle

### ✅ Tests PowerUp Temporaire
- `TemporaryPowerUp_Use_ShouldActivate`
- `TemporaryPowerUp_UseWhileActive_ShouldNotRestart`
- `TemporaryPowerUp_ReduceDuration_ShouldWorkCorrectly`

**Valide** : Activation, durée, fin d'effet

### ✅ Tests PowerUp Permanent
- `PermanentPowerUp_Use_ShouldMarkAsUsed`
- `PermanentPowerUp_UseAgain_ShouldNotWorkTwice`

**Valide** : Usage unique, protection contre double usage

### ✅ Tests Inventory Singleton
- `Inventory_Singleton_ShouldReturnSameInstance`
- `Inventory_AddItem_ShouldIncreaseQuantity`
- `Inventory_AddSameItem_ShouldStack`
- `Inventory_RemoveItem_ShouldDecreaseQuantity`
- `Inventory_RemoveAllItems_ShouldRemoveFromInventory`

**Valide** : Pattern singleton, gestion quantités, stack items

### ✅ Tests Edge Cases (Cas Limite)
- `Inventory_AddNullItem_ShouldNotCrash`
- `Inventory_AddNegativeQuantity_ShouldNotAdd`
- `Inventory_RemoveNonExistentItem_ShouldReturnFalse`
- `Inventory_RemoveMoreThanAvailable_ShouldReturnFalse`
- `Inventory_UseNonExistentItem_ShouldReturnFalse`

**Valide** : Robustesse code, gestion erreurs, sécurité

### ✅ Tests Intégration
- `Inventory_UsePermanentPowerUp_ShouldRemoveFromInventory`
- `Inventory_GetItemsByCategory_ShouldFilterCorrectly`
- `Inventory_UpdateTemporaryEffects_ShouldReduceAllCooldowns`
- `Inventory_CompleteGameFlow_ShouldWorkCorrectly`

**Valide** : Workflow complet, interactions entre composants

## 🎯 Interprétation Résultats

### ✅ Test RÉUSSI
```
✅ Item_CreationCapacity_ShouldHaveCorrectProperties - PASSED
```
➡️ **Fonctionnalité validée** : Le code fonctionne comme attendu

### ❌ Test ÉCHOUÉ
```
❌ Inventory_AddItem_ShouldIncreaseQuantity - FAILED
Expected: 3, Actual: 0
```
➡️ **Bug détecté** : Problème dans la logique d'ajout d'items

### ⚠️ Test IGNORÉ
```
⚠️ SomeTest - IGNORED  
```
➡️ **Test désactivé** : Fonctionnalité pas encore implémentée

## 🔧 Debugging Tests

### 1. Test qui Échoue
- **Clic droit** sur le test > **Déboguer**
- **Points d'arrêt** sur les `Assert`
- **Variables locales** pour voir les valeurs

### 2. Console Output
```csharp
// Ajouter dans vos tests pour debug
Console.WriteLine($"Debug: Quantity = {inventory.GetItemQuantity(item)}");
```

### 3. Test Explorer
- **Détails du test** : double-clic sur test échoué
- **Stack trace** : voir où l'erreur s'est produite

## 🎮 Tests de Régression

### Avant Modifications
1. **Exécuter tous les tests** : s'assurer qu'ils passent
2. **Commit Git** : sauvegarder l'état stable

### Après Modifications
1. **Réexécuter tous les tests**
2. **Si échec** : analyser et corriger
3. **Si succès** : modification validée

## 📈 Couverture de Code

### Ce qui EST testé
- ✅ Création d'objets
- ✅ Propriétés et getters/setters
- ✅ Méthodes publiques principales
- ✅ Logique métier (cooldown, durée, usage)
- ✅ Cas d'erreur et edge cases
- ✅ Pattern Singleton
- ✅ Intégrations complexes

### Ce qui N'EST PAS testé
- ❌ Affichage Console (difficile à tester)
- ❌ Events (nécessiterait mocks)
- ❌ Performance (pas nécessaire ici)

## 🚨 Règles de Qualité

### Tests DOIVENT
- ✅ Tester **UN** concept par test
- ✅ Avoir des noms **explicites**
- ✅ Utiliser **Arrange/Act/Assert**
- ✅ Être **indépendants** (pas d'ordre)
- ✅ Être **reproductibles**

### Tests NE DOIVENT PAS
- ❌ Dépendre les uns des autres
- ❌ Tester plusieurs fonctionnalités
- ❌ Avoir de la logique complexe
- ❌ Être lents à exécuter

## 🎯 Objectif Pédagogique

Ces tests prouvent que ton système d'inventaire est **robuste** et **prêt pour la production**. Ils documentent le comportement attendu et détectent les régressions lors des modifications futures.

**Comme dit dans le cours POO** : "Les tests unitaires sont une preuve que l'on peut avoir confiance dans votre production."