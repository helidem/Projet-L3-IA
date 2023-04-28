public class Position
{
    public int Ligne { get; set; }
    public int Colonne { get; set; }

    public Position(int ligne, int colonne)
    {
        Ligne = ligne;
        Colonne = colonne;
    }


    public override bool Equals(object obj)
    {
        if (obj is Position autre)
        {
            return Ligne == autre.Ligne && Colonne == autre.Colonne;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return 8 * Ligne + Colonne;
    }
}
