using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum GamePhase
{
    EARLY,
    MID,
    END
}
public class Evaluator
{

    public int eval(Joueur[,] plateau, Joueur joueur, GameState gameState)
    {
        if (gameState.FinPartie)
        {
            return 1000 * evalDiffPieces(plateau, joueur);
        }

        switch (GetGamePhase(plateau, gameState))
        {
            case GamePhase.EARLY:
                return 1000 * evalCorners(plateau, joueur) + 50 * evalMobility(plateau, joueur, gameState);
            case GamePhase.MID:
                return 1000 * evalCorners(plateau, joueur) + 20 * evalMobility(plateau, joueur, gameState) + 10 * evalDiffPieces(plateau, joueur) + 100 * evalBords(plateau, joueur);
            case GamePhase.END:
                return 1000 * evalCorners(plateau, joueur) + 100 * evalMobility(plateau, joueur, gameState) + 500 * evalDiffPieces(plateau, joueur) + 500 * evalBords(plateau, joueur);
            default:
                return 0;
        }
    }

    private GamePhase GetGamePhase(Joueur[,] plateau, GameState gameState)
    {
        int nbPieces = gameState.NbPiece[Joueur.Noir] + gameState.NbPiece[Joueur.Blanc];
        if (nbPieces < 20)
        {
            return GamePhase.EARLY;
        }
        else if (nbPieces < 58) // on a testé plusieurs valeurs et 58 est la meilleure
        {
            return GamePhase.MID;
        }
        else
        {
            return GamePhase.END;
        }
    }


    /// <summary>
    /// Evalue la différence de pièces entre le joueur et son adversaire
    /// </summary>
    /// <param name="plateau">Le plateau.</param>
    /// <param name="joueur">Le joueur actuel.</param>
    /// <returns>La différence de pièces.</returns>
    public int evalDiffPieces(Joueur[,] plateau, Joueur joueur)
    {
        int nbPieces = 0;
        int ennemiNbPieces = 0;
        for (int l = 0; l < GameState.NB_LIGNES; l++)
        {
            for (int c = 0; c < GameState.NB_COLONNES; c++)
            {
                if (plateau[l, c] == joueur)
                {
                    nbPieces++;
                }
                else if (plateau[l, c] != Joueur.Vide)
                {
                    ennemiNbPieces++;
                }
            }
        }
        return 100 * (nbPieces - ennemiNbPieces) / (nbPieces + ennemiNbPieces);
    }

    /// <summary>
    /// Evalue la mobilité du joueur.
    /// </summary>
    /// <param name="plateau">Le plateau.</param>
    /// <param name="joueur">Le joueur actuel.</param>
    /// <param name="gameState">L'état du jeu.</param>
    public int evalMobility(Joueur[,] plateau, Joueur joueur, GameState gameState)
    {
        int nbCoupsPossibles = gameState.ListeMouvementsLegaux(joueur, plateau).Count;
        int ennemiNbCoupPossibles = gameState.ListeMouvementsLegaux(joueur == Joueur.Noir ? Joueur.Blanc : Joueur.Noir, plateau).Count;

        return 100 * (nbCoupsPossibles - ennemiNbCoupPossibles) / (nbCoupsPossibles + ennemiNbCoupPossibles + 1);
    }

    /// <summary>
    /// Evalue la capacité à capturer les coins du plateau.
    /// </summary>
    /// <param name="plateau">Le plateau.</param>
    /// <param name="joueur">Le joueur actuel.</param>
    public int evalCorners(Joueur[,] plateau, Joueur joueur)
    {
        int nbCoins = 0;
        int ennemiNbCoins = 0;

        if (plateau[0, 0] == joueur)
        {
            nbCoins++;
        }
        else if (plateau[0, 0] != Joueur.Vide)
        {
            ennemiNbCoins++;
        }

        if (plateau[0, 7] == joueur)
        {
            nbCoins++;
        }
        else if (plateau[0, 7] != Joueur.Vide)
        {
            ennemiNbCoins++;
        }

        if (plateau[7, 0] == joueur)
        {
            nbCoins++;
        }
        else if (plateau[7, 0] != Joueur.Vide)
        {

            ennemiNbCoins++;
        }
        if (plateau[7, 7] == joueur)
        {
            nbCoins++;
        }
        else if (plateau[7, 7] != Joueur.Vide)
        {
            ennemiNbCoins++;
        }

        return 100 * (nbCoins - ennemiNbCoins) / (nbCoins + ennemiNbCoins + 1);
    }


    /// <summary>
    /// Evalue la capacité à capturer les bords du plateau.
    /// </summary>
    /// <param name="plateau">Le plateau.</param>
    /// <param name="joueur">Le joueur actuel.</param>
    public int evalBords(Joueur[,] plateau, Joueur joueur)
    {
        Joueur ennemi = joueur.AutreJoueur();
        int[][] poids =
        {
            new int[] {200 , -100, 100,  50,  50, 100, -100,  200},
            new int[] {-100, -200, -50, -50, -50, -50, -200, -100},
            new int[] {100 ,  -50, 100,   0,   0, 100,  -50,  100},
            new int[] {50  ,  -50,   0,   0,   0,   0,  -50,   50},
            new int[] {50  ,  -50,   0,   0,   0,   0,  -50,   50},
            new int[] {100 ,  -50, 100,   0,   0, 100,  -50,  100},
            new int[] {-100, -200, -50, -50, -50, -50, -200, -100},
            new int[] {200 , -100, 100,  50,  50, 100, -100,  200}
        };

        if (plateau[0, 0] != Joueur.Vide)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    poids[i][j] = 0;
                }
            }
        }
        if (plateau[0, 7] != Joueur.Vide)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 4; j <= 7; j++)
                {
                    poids[i][j] = 0;
                }
            }
        }
        if (plateau[7, 0] != Joueur.Vide)
        {
            for (int i = 5; i < 8; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    poids[i][j] = 0;
                }
            }
        }
        if (plateau[7, 7] != Joueur.Vide)
        {
            for (int i = 5; i < 8; i++)
            {
                for (int j = 4; j <= 7; j++)
                {
                    poids[i][j] = 0;
                }
            }
        }

        int score = 0;
        int ennemiScore = 0;

        for (int l = 0; l < GameState.NB_LIGNES; l++)
        {
            for (int c = 0; c < GameState.NB_COLONNES; c++)
            {
                if (plateau[l, c] == joueur)
                {
                    score += poids[l][c];
                }
                else if (plateau[l, c] == ennemi)
                {
                    ennemiScore += poids[l][c];
                }
            }
        }

        return (score - ennemiScore) / (score + ennemiScore + 1);
    }
}
