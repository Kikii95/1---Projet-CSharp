using System;

/// <summary>
/// Point d'entrée pour la démonstration interactive
/// Exécute les demos + donne option de lancer les tests unitaires
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🎮 === SYSTÈME INVENTAIRE - C# POO ===");
        Console.WriteLine("Projet selon cours GT3 - Kévin Roussel");
        Console.WriteLine("Architecture: Singleton + Héritage + Events + Tests Unitaires\n");

        try
        {
            // Démonstration interactive complète
            InventorySystemTests.RunInteractiveDemo();

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("🧪 TESTS UNITAIRES AUTOMATISÉS");
            Console.WriteLine("Pour exécuter les tests automatisés :");
            Console.WriteLine("• Visual Studio: Test > Exécuter tous les tests");
            Console.WriteLine("• Console: dotnet test");
            Console.WriteLine("• Test Explorer: Clic droit > Exécuter les tests");
            Console.WriteLine("" + new string('=', 60));

            Console.WriteLine("\n✅ Système d'inventaire prêt pour intégration !");
            Console.WriteLine("📦 Classes disponibles: Item, Capacity, PowerUp, Inventory");
            Console.WriteLine("🔗 Prêt pour connexion avec: Ship, GameObject, Canvas, Scene");
            
            Console.WriteLine("\nAppuyez sur Entrée pour quitter...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur: {ex.Message}");
            Console.WriteLine("Vérifiez que tous les fichiers source sont présents.");
            Console.ReadLine();
        }
    }
}