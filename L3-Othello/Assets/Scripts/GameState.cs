using System.Collections.Generic;
using UnityEngine;

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
    public AIPlayer ai { get; private set; }

    public GameState()
    {
        Plateau = new Joueur[NB_LIGNES, NB_COLONNES];
        Plateau[3, 3] = Joueur.Blanc;
        Plateau[4, 4] = Joueur.Blanc;
        Plateau[3, 4] = Joueur.Noir;
        Plateau[4, 3] = Joueur.Noir;

        ai = new AIPlayer(this, 1);

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
        List<Position> captures = MouvementsLegaux[position];

        Plateau[position.Ligne, position.Colonne] = joueur;
        RetournerPiece(captures, Plateau);
        NombrePieces(joueur, captures.Count);
        TourSuivant();

        moveInfo = new MoveInfo { Joueur = joueur, Position = position, Captures = captures };
        return true;
    }

    public IEnumerable<Position> PositionsOccupees()
    {
        for (int l = 0; l < NB_LIGNES; l++)
        {
            for (int c = 0; c < NB_COLONNES; c++)
            {
                if (Plateau[l, c] != Joueur.Vide)
                {
                    yield return new Position(l, c);
                }
            }
        }
    }


    private void RetournerPiece(List<Position> postitions, Joueur[,] plateau)
    {
        foreach (Position pos in postitions)
        {
            plateau[pos.Ligne, pos.Colonne] = plateau[pos.Ligne, pos.Colonne].AutreJoueur();
        }
    }

    private void NombrePieces(Joueur joueur, int NbCapture)
    {
        NbPiece[joueur] += NbCapture + 1;
        NbPiece[joueur.AutreJoueur()] -= NbCapture;
    }

    public void JoueurSuivant()
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

    private List<Position> CaptureDansDirection(Position position, Joueur joueur, int directionLigne, int directionColonne, Joueur[,] plateau)
    {
        List<Position> captures = new List<Position>();
        int ligne = position.Ligne + directionLigne;
        int colonne = position.Colonne + directionColonne;

        while (EstDansPlateau(ligne, colonne) && plateau[ligne, colonne] != Joueur.Vide)
        {
            if (plateau[ligne, colonne] == joueur.AutreJoueur())
            {
                captures.Add(new Position(ligne, colonne));
                ligne += directionLigne;
                colonne += directionColonne;
            }
            else if (plateau[ligne, colonne] == joueur)
            {
                return captures;
            }
        }

        return new List<Position>();
    }

    private List<Position> Captures(Position position, Joueur joueur, Joueur[,] plateau)
    {
        List<Position> captures = new List<Position>();

        for (int directionLigne = -1; directionLigne <= 1; directionLigne++)
        {
            for (int directionColonne = -1; directionColonne <= 1; directionColonne++)
            {
                if (directionColonne == 0 && directionLigne == 0) continue;
                if (JoueurActuel == Joueur.Noir)
                {
                    captures.AddRange(CaptureDansDirection(position, joueur, directionLigne, directionColonne, Plateau));
                }
                else
                {
                    captures.AddRange(CaptureDansDirection(position, joueur, directionLigne, directionColonne, plateau));
                }
            }
        }

        return captures;
    }

    private bool EstLegal(Joueur joueur, Position position, Joueur[,] plateau, out List<Position> captures)
    {
        if (plateau[position.Ligne, position.Colonne] != Joueur.Vide)
        {
            captures = null; return false;
        }

        // captures = Captures(position, joueur);
        if (JoueurActuel == Joueur.Noir)
        {
            captures = Captures(position, joueur, Plateau);
        }
        else
        {
            captures = Captures(position, joueur, plateau);
        }

        return captures.Count > 0;
    }

    public Dictionary<Position, List<Position>> ListeMouvementsLegaux(Joueur joueur)
    {
        Dictionary<Position, List<Position>> mouvementsLegaux = new Dictionary<Position, List<Position>>();

        for (int ligne = 0; ligne < NB_LIGNES; ligne++)
        {
            for (int colonne = 0; colonne < NB_COLONNES; colonne++)
            {
                Position position = new Position(ligne, colonne);

                if (EstLegal(joueur, position, Plateau, out List<Position> captures))
                {
                    mouvementsLegaux.Add(position, captures); // ou mouvementsLegaux[position] = captures;
                }
            }
        }
        return mouvementsLegaux;
    }

    /**
    * Retourne le nombre de pièces capturables par le joueur si il joue sur la position donnée.
    * @param pos La position où le joueur veut jouer.
    * @return Le nombre de pièces capturables.
    */
    public int getNombreDePiecesCapturables(Position pos)
    {
        int nbPiecesCapturables = 0;
        List<Position> captures = new List<Position>();
        if (EstLegal(JoueurActuel, pos, Plateau, out captures))
        {
            nbPiecesCapturables = captures.Count;
        }
        return nbPiecesCapturables;
    }

    public Joueur[,] getNewPlateauAfterMove(Joueur[,] plateau, Position pos)
    {
        // clone le plateau
        Joueur[,] newPlateau = new Joueur[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                newPlateau[i, j] = plateau[i, j];
            }
        }

        // joue le coup
        newPlateau[pos.Ligne, pos.Colonne] = JoueurActuel;

        // retourne les pièces
        List<Position> captures = new List<Position>();
        if (EstLegal(JoueurActuel, pos, newPlateau, out captures))
        {
            RetournerPiece(captures, newPlateau);
        }

        // retourne le nouveau plateau
        return newPlateau;
    }
}
