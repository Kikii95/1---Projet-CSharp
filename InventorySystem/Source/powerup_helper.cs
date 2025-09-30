using System;

public static class PowerUpHelper
{
    // === CONVERSION PERMANENT POWERUP ===
    public static StatModifier ConvertToStatModifier(PermanentPowerUp powerUp)
    {
        if (powerUp == null)
        {
            Console.WriteLine("⚠️ PowerUp null, impossible de convertir");
            return null;
        }

        ModifiableStat targetStat = ConvertPowerUpStatToModifiableStat(powerUp.Stat);
        
        return new StatModifier(
            powerUp.Name,
            targetStat,
            ModifierType.FLAT,
            powerUp.Value
        );
    }

    // === CONVERSION TEMPORARY POWERUP ===
    public static StatModifier ConvertToStatModifier(TemporaryPowerUp powerUp)
    {
        if (powerUp == null)
        {
            Console.WriteLine("⚠️ PowerUp null, impossible de convertir");
            return null;
        }

        ModifiableStat targetStat = ConvertPowerUpStatToModifiableStat(powerUp.Stat);
        
        return new StatModifier(
            powerUp.Name,
            targetStat,
            ModifierType.FLAT,
            powerUp.Value,
            powerUp.Duration
        );
    }

    // === CONVERSION AUTOMATIQUE (POLYMORPHISME) ===
    public static StatModifier ConvertToStatModifier(PowerUp powerUp)
    {
        if (powerUp == null)
        {
            Console.WriteLine("⚠️ PowerUp null, impossible de convertir");
            return null;
        }

        if (powerUp is PermanentPowerUp permPowerUp)
        {
            return ConvertToStatModifier(permPowerUp);
        }
        else if (powerUp is TemporaryPowerUp tempPowerUp)
        {
            return ConvertToStatModifier(tempPowerUp);
        }

        Console.WriteLine($"⚠️ Type de PowerUp non reconnu: {powerUp.GetType().Name}");
        return null;
    }

    // === MÉTHODE PRIVÉE DE CONVERSION ENUM ===
    private static ModifiableStat ConvertPowerUpStatToModifiableStat(PowerUpStat powerUpStat)
    {
        switch (powerUpStat)
        {
            case PowerUpStat.ATTACK:
                return ModifiableStat.ATTACK;
            
            case PowerUpStat.DEFENSE:
                return ModifiableStat.DEFENSE;
            
            case PowerUpStat.SPEED:
                return ModifiableStat.SPEED;
            
            case PowerUpStat.MAX_HP:
                return ModifiableStat.MAX_HP;
            
            default:
                Console.WriteLine($"⚠️ PowerUpStat non mappé: {powerUpStat}, utilisation de ATTACK par défaut");
                return ModifiableStat.ATTACK;
        }
    }

    // === MÉTHODES UTILITAIRES ===
    public static bool ApplyPowerUpToShip(Ship ship, PowerUp powerUp, Inventory inventory)
    {
        if (ship == null || powerUp == null || inventory == null)
        {
            Console.WriteLine("⚠️ Paramètre null détecté");
            return false;
        }

        if (!inventory.UseItem(powerUp))
        {
            return false;
        }

        StatModifier modifier = ConvertToStatModifier(powerUp);
        
        if (modifier != null)
        {
            ship.AddStatModifier(modifier);
            Console.WriteLine($"✅ {powerUp.Name} appliqué sur {ship.Name}");
            return true;
        }

        Console.WriteLine($"❌ Échec de la conversion du PowerUp {powerUp.Name}");
        return false;
    }

    public static bool IsPowerUp(Item item)
    {
        return item is PowerUp;
    }

    public static PowerUp TryGetPowerUp(Item item)
    {
        return item as PowerUp;
    }
}
