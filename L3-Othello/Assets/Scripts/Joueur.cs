public enum Joueur
{
    Vide, Noir, Blanc
}

public static class JoueurExtensions
{
    public static Joueur AutreJoueur(this Joueur joueur)
    {
        if (joueur == Joueur.Noir)
        {
            return Joueur.Blanc;
        }
        else if (joueur == Joueur.Blanc)
        {
            return Joueur.Noir;
        }

        return Joueur.Vide;
    }
}