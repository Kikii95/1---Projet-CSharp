using System;
using System.Collections.Generic;

public enum ShipType
{
    FEU,      
    GLACE,    
    ROCHE,    
    ELECTRIQUE, 
    EAU,     
    NORMAL    
}

public enum AttackType
{
    FEU,
    GLACE,
    ROCHE,
    ELECTRIQUE,
    EAU,
    NORMAL
}

public static class TypeEffectiveness
{
    // === CONSTANTES DE MULTIPLICATEURS ===
    public const float SUPER_EFFECTIVE = 2.0f;   
    public const float EFFECTIVE = 1.0f;          
    public const float NOT_VERY_EFFECTIVE = 0.5f; 
    public const float STAB_BONUS = 1.5f;         

    // === TABLEAU DES EFFICACITÉS ===
    private static Dictionary<(AttackType, ShipType), float> _effectivenessTable;

    // === INITIALISATION STATIQUE ===
    static TypeEffectiveness()
    {
        InitializeEffectivenessTable();
    }

    private static void InitializeEffectivenessTable()
    {
        _effectivenessTable = new Dictionary<(AttackType, ShipType), float>();

        // FEU
        _effectivenessTable[(AttackType.FEU, ShipType.GLACE)] = SUPER_EFFECTIVE;
        _effectivenessTable[(AttackType.FEU, ShipType.ROCHE)] = NOT_VERY_EFFECTIVE;
        _effectivenessTable[(AttackType.FEU, ShipType.EAU)] = NOT_VERY_EFFECTIVE;
        _effectivenessTable[(AttackType.FEU, ShipType.FEU)] = NOT_VERY_EFFECTIVE;

        // GLACE
        _effectivenessTable[(AttackType.GLACE, ShipType.ROCHE)] = SUPER_EFFECTIVE;
        _effectivenessTable[(AttackType.GLACE, ShipType.FEU)] = NOT_VERY_EFFECTIVE;
        _effectivenessTable[(AttackType.GLACE, ShipType.EAU)] = NOT_VERY_EFFECTIVE;
        _effectivenessTable[(AttackType.GLACE, ShipType.GLACE)] = NOT_VERY_EFFECTIVE;

        // ROCHE
        _effectivenessTable[(AttackType.ROCHE, ShipType.ELECTRIQUE)] = SUPER_EFFECTIVE;
        _effectivenessTable[(AttackType.ROCHE, ShipType.FEU)] = SUPER_EFFECTIVE;
        _effectivenessTable[(AttackType.ROCHE, ShipType.GLACE)] = NOT_VERY_EFFECTIVE;

        // ELECTRIQUE
        _effectivenessTable[(AttackType.ELECTRIQUE, ShipType.EAU)] = SUPER_EFFECTIVE;
        _effectivenessTable[(AttackType.ELECTRIQUE, ShipType.ROCHE)] = NOT_VERY_EFFECTIVE;
        _effectivenessTable[(AttackType.ELECTRIQUE, ShipType.ELECTRIQUE)] = NOT_VERY_EFFECTIVE;

        // EAU
        _effectivenessTable[(AttackType.EAU, ShipType.FEU)] = SUPER_EFFECTIVE;
        _effectivenessTable[(AttackType.EAU, ShipType.ROCHE)] = SUPER_EFFECTIVE;
        _effectivenessTable[(AttackType.EAU, ShipType.GLACE)] = NOT_VERY_EFFECTIVE;
        _effectivenessTable[(AttackType.EAU, ShipType.EAU)] = NOT_VERY_EFFECTIVE;

        // NORMAL - toujours efficacité normale (1.0) par défaut
    }

    public static float GetEffectiveness(AttackType attackType, ShipType defenderType)
    {
        if (_effectivenessTable.ContainsKey((attackType, defenderType)))
        {
            return _effectivenessTable[(attackType, defenderType)];
        }

        return EFFECTIVE; 
    }

    public static bool HasSTAB(AttackType attackType, ShipType attackerType)
    {
        return (int)attackType == (int)attackerType;
    }

    public static float GetTotalMultiplier(AttackType attackType, ShipType attackerType, ShipType defenderType)
    {
        float effectiveness = GetEffectiveness(attackType, defenderType);
        float stabBonus = HasSTAB(attackType, attackerType) ? STAB_BONUS : 1.0f;

        return effectiveness * stabBonus;
    }

    public static string GetEffectivenessMessage(float multiplier)
    {
        if (multiplier >= SUPER_EFFECTIVE * STAB_BONUS)
            return "C'est EXTRÊMEMENT efficace !";
        else if (multiplier >= SUPER_EFFECTIVE)
            return "C'est super efficace !";
        else if (multiplier > EFFECTIVE)
            return "C'est plutôt efficace.";
        else if (multiplier < EFFECTIVE)
            return "Ce n'est pas très efficace...";
        else
            return "";
    }
}
