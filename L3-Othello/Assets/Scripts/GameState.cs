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

        MouvementsLegaux = ListeMouvementsLegaux(JoueurActuel, Plateau);

    }

    /// <summary>
    /// Retourne vrai si le coup est valide et correctement joué.
    /// </summary>
    /// <param name="position"> Position du coup. </param>
    /// <param name="moveInfo"> Informations sur le coup. </param>
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


    /// <summary>
    /// Méthode qui retourne les pieces capturées par un joueur.
    /// </summary>
    /// <param name="positions"> Liste des positions des pieces capturées. </param>
    /// <param name="plateau"> Plateau de jeu. </param>
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
        MouvementsLegaux = ListeMouvementsLegaux(JoueurActuel, Plateau);
    }

    /// <summary>
    /// Retourne le gagnant de la partie.
    /// </summary>
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

    /// <summary>
    /// Retourne vrai si la position est dans le plateau.
    /// </summary>
    /// <param name="ligne">Ligne de la position.</param>
    /// <param name="colonne">Colonne de la position.</param>
    /// <returns>Vrai si la position est dans le plateau.</returns>
    private bool EstDansPlateau(int ligne, int colonne)
    {
        return ligne >= 0 && ligne < NB_LIGNES && colonne >= 0 && colonne < NB_COLONNES;
    }

    /// <summary>
    /// Retourne la liste des positions capturées dans une direction.
    /// </summary>
    /// <param name="position">Position de la pièce jouée.</param>
    /// <param name="joueur">Joueur qui joue.</param>
    /// <param name="directionLigne">Direction de la ligne.</param>
    /// <param name="directionColonne">Direction de la colonne.</param>
    /// <param name="plateau">Plateau de jeu.</param> 
    /// <returns>Liste des positions capturées.</returns>
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

    /// <summary>
    /// Retourne la liste des positions capturées dans toutes les directions.
    /// </summary>
    /// <param name="position">Position de la pièce jouée.</param>
    /// <param name="joueur">Joueur qui joue.</param>
    /// <param name="plateau">Plateau de jeu.</param>
    /// <returns>Liste des positions capturées.</returns>
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

    /// <summary>
    /// Retourne vrai si le mouvement est légal et retourne la liste des positions capturées.
    /// </summary>
    /// <param name="joueur">Joueur pour lequel on veut savoir si le mouvement est légal.</param>
    /// <param name="position">Position de la pièce à jouer.</param>
    /// <param name="captures">Liste des positions capturées.</param>
    /// <param name="plateau">Le plateau</param>
    /// <returns>Vrai si le mouvement est légal.</returns>
    private bool EstLegal(Joueur joueur, Position position, Joueur[,] plateau, out List<Position> captures)
    {
        if (plateau[position.Ligne, position.Colonne] != Joueur.Vide)
        {
            captures = null; return false;
        }

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

    /// <summary>
    /// Retourne le dictonnaire des mouvements légaux avec la position de la pièce et la liste des positions capturées.
    /// </summary>
    /// <param name="joueur">Joueur pour lequel on veut la liste des mouvements légaux.</param>
    /// <param name="plateau">Plateau sur lequel on veut la liste des mouvements légaux.</param>
    /// <returns>La liste des mouvements légaux pour le joueur donné et le plateau donné.</returns>
    public Dictionary<Position, List<Position>> ListeMouvementsLegaux(Joueur joueur, Joueur[,] plateau)
    {
        Dictionary<Position, List<Position>> mouvementsLegaux = new Dictionary<Position, List<Position>>();

        for (int ligne = 0; ligne < NB_LIGNES; ligne++)
        {
            for (int colonne = 0; colonne < NB_COLONNES; colonne++)
            {
                Position position = new Position(ligne, colonne);

                if (EstLegal(joueur, position, plateau, out List<Position> captures))
                {
                    mouvementsLegaux.Add(position, captures); // ou mouvementsLegaux[position] = captures;
                }
            }
        }
        return mouvementsLegaux;
    }

    /// <summary>
    /// Retourne le nombre de pièces capturables par le joueur si il joue sur la position donnée.
    /// </summary>
    /// <param name="pos">Position où le joueur veut jouer</param>
    /// <returns>Nombre de pièces capturables</returns>
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


    /// <summary>
    /// Retourne le nouveau plateau après avoir joué le coup sur la position donnée (sans changer le plateau actuel).
    /// </summary>
    /// <param name="plateau">Plateau actuel</param>
    /// <param name="pos">Position où le joueur veut jouer</param>
    /// <returns>Nouveau plateau après avoir joué le coup</returns>
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
