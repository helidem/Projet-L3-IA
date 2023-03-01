using System.Collections.Generic;
using Unity.Burst;

public class GameState
{
    public const int NB_LIGNES = 8;
    public const int NB_COLONNES = 8;

    public Joueur[,] Plateau { get; }
    public Dictionary<Joueur, int> NbPiece { get; }

    public Joueur JoueurActuel { get; private set; }
    public bool FinPartie { get; private set; }
    public Joueur Gagnant { get; private set; }
    public Dictionary<Position, List<Position>> MouvementsLegaux { get; private set; }

    public GameState()
    {
        Plateau = new Joueur[NB_LIGNES, NB_COLONNES];
        Plateau[3, 3] = Joueur.Blanc;
        Plateau[4, 4] = Joueur.Blanc;
        Plateau[3, 4] = Joueur.Noir;
        Plateau[4, 3] = Joueur.Noir;

        NbPiece = new Dictionary<Joueur, int>() { { Joueur.Noir, 2 }, { Joueur.Blanc, 2 } };

        JoueurActuel = Joueur.Noir;

        MouvementsLegaux = ListeMouvementsLegaux(JoueurActuel);

    }

    public bool Jouer(Position position, out MoveInfo moveInfo)
    {
        if (!MouvementsLegaux.ContainsKey(position))
        {
            moveInfo = null;
            return false;
        }

        Joueur joueur = JoueurActuel;
        List<Position> capturés = MouvementsLegaux[position];

        Plateau[position.Ligne, position.Colonne] = joueur;
        RetournerPiece(capturés);
        NombrePieces(joueur, capturés.Count);
        TourSuivant();

        moveInfo = new MoveInfo { Joueur = joueur, Position = position, Capturés = capturés };
        return true;
    }

    public IEnumerable<Position> PositionsOccupées()
    {
        for (int l = 0; l< NB_LIGNES;  l++)
        {
            for (int c = 0;  c < NB_COLONNES; c++)
            {
                if (Plateau[l, c] != Joueur.Vide)
                {
                    yield return new Position(l, c);
                }
            }
        }
    }


    private void RetournerPiece(List<Position> postitions)
    {
        foreach (Position pos in postitions)
        {
            Plateau[pos.Ligne, pos.Colonne] = Plateau[pos.Ligne, pos.Colonne].AutreJoueur();
        }
    }

    private void NombrePieces(Joueur joueur, int NbCapture)
    {
        NbPiece[joueur] += NbCapture + 1;
        NbPiece[joueur.AutreJoueur()] -= NbCapture;
    }

    private void JoueurSuivant()
    {
        JoueurActuel = JoueurActuel.AutreJoueur();
        MouvementsLegaux = ListeMouvementsLegaux(JoueurActuel);
    }

    private Joueur GetGagnant()
    {
        if (NbPiece[Joueur.Noir] > NbPiece[Joueur.Blanc])
        {
            return Joueur.Noir;
        }
        if (NbPiece[Joueur.Noir] < NbPiece[Joueur.Blanc])
        {
            return Joueur.Blanc;
        }
        return Joueur.Vide;
    }

    private void TourSuivant()
    {
        JoueurSuivant();
        if (MouvementsLegaux.Count > 0)
        {
            return;
        }
        JoueurSuivant();
        if (MouvementsLegaux.Count == 0)
        {
            JoueurActuel = Joueur.Vide;
            FinPartie = true;
            Gagnant = GetGagnant();
        }
    }


    private bool EstDansPlateau(int ligne, int colonne)
    {
        return ligne >= 0 && ligne < NB_LIGNES && colonne >= 0 && colonne < NB_COLONNES;
    }

    private List<Position> CaptureDansDirection(Position position, Joueur joueur, int directionLigne, int directionColonne)
    {
        List<Position> capturés = new List<Position>();
        int ligne = position.Ligne + directionLigne;
        int colonne = position.Colonne + directionColonne;

        while (EstDansPlateau(ligne, colonne) && Plateau[ligne, colonne] != Joueur.Vide)
        {
            if (Plateau[ligne, colonne] == joueur.AutreJoueur())
            {
                capturés.Add(new Position(ligne, colonne));
                ligne += directionLigne;
                colonne += directionColonne;
            }
            else if (Plateau[ligne, colonne] == joueur)
            {
                return capturés;
            }
        }

        return new List<Position>();
    }

    private List<Position> Capturés(Position position, Joueur joueur)
    {
        List<Position> capturés = new List<Position>();

        for (int directionLigne = -1; directionLigne <= 1; directionLigne++)
        {
            for (int directionColonne = -1; directionColonne <= 1; directionColonne++)
            {
                if (directionColonne == 0 && directionLigne == 0) continue;
                capturés.AddRange(CaptureDansDirection(position, joueur, directionLigne, directionColonne));
            }
        }

        return capturés;
    }

    private bool EstLegal(Joueur joueur, Position position, out List<Position> capturés)
    {
        if (Plateau[position.Ligne, position.Colonne] != Joueur.Vide)
        {
            capturés = null; return false;
        }

        capturés = Capturés(position, joueur);
        return capturés.Count > 0;
    }

    private Dictionary<Position, List<Position>> ListeMouvementsLegaux(Joueur joueur)
    {
        Dictionary<Position, List<Position>> mouvementsLegaux = new Dictionary<Position, List<Position>>();

        for (int ligne = 0; ligne < NB_LIGNES; ligne++)
        {
            for (int colonne = 0; colonne < NB_COLONNES; colonne++)
            {
                Position position = new Position(ligne, colonne);

                if (EstLegal(joueur, position, out List<Position> capturés))
                {
                    mouvementsLegaux.Add(position, capturés); // ou mouvementsLegaux[position] = capturés;
                }
            }
        }
        return mouvementsLegaux;
    }

}
