using System;
public interface IHealth
{
    // === PROPRIÉTÉS ===
    int CurrentHP { get; }
    int MaxHP { get; }
    bool IsAlive { get; }
    float HPPercentage { get; }

    // === ÉVÉNEMENTS ===
    event Action<int> OnDamageTaken;     
    event Action<int> OnHealed;          
    event Action OnDeath;               
    event Action<int, int> OnHPChanged; 

    // === MÉTHODES ===
    void TakeDamage(int amount);
    void Heal(int amount);
    void Die();
    void Revive(int hpAmount);
}
