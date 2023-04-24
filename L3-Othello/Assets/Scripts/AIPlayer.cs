using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AIPlayer
{
    private GameState gameState;
    private Joueur joueur;
    private int nodesExplored = 0;
    private Evaluator evaluator = new Evaluator();

    public int difficulte { get; private set; }

    public AIPlayer(GameState gameState, Joueur joueur, int difficulte)
    {
        this.gameState = gameState;
        this.joueur = joueur;
        this.difficulte = difficulte;
    }

    public Position Jouer(Joueur[,] plateau, Joueur joueur)
    {
        return solve(plateau, joueur, 4, evaluator);
    }

    private Position JouerDifficile()
    {
        return null;
    }

    private Position JouerMoyen()
    {
        return null;
    }

    private Position JouerFacile()
    {
        Position pos = new Position(2, 2);
        gameState.getNewPlateauAfterMove(gameState.Plateau, pos);
        return pos;
    }

    public Position solve(Joueur[,] plateau, Joueur joueur, int profondeur, Evaluator e)
    {
        nodesExplored = 0;
        int bestScore = int.MinValue;
        Position bestMove = null;
        foreach (Position move in gameState.MouvementsLegaux.Keys)
        {
            Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
            int score = MMAB(newPlateau, joueur, profondeur - 1, false, int.MinValue, int.MaxValue, e);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }

    private int MMAB(Joueur[,] plateau, Joueur joueur, int profondeur, bool maximizingPlayer, int alpha, int beta, Evaluator e)
    {
        nodesExplored++;
        if (profondeur == 0 || gameState.FinPartie)
        {
            return e.eval(plateau, joueur, gameState);
        }
        if (maximizingPlayer)
        {
            int bestScore = int.MinValue;
            foreach (Position move in gameState.MouvementsLegaux.Keys)
            {
                Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
                int score = MMAB(newPlateau, joueur, profondeur - 1, false, alpha, beta, e);
                bestScore = Mathf.Max(score, bestScore);
                alpha = Mathf.Max(alpha, bestScore);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            foreach (Position move in gameState.MouvementsLegaux.Keys)
            {
                Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
                int score = MMAB(newPlateau, joueur, profondeur - 1, true, alpha, beta, e);
                bestScore = Mathf.Min(score, bestScore);
                beta = Mathf.Min(beta, bestScore);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return bestScore;
        }
    }

    internal Position Glouton(Joueur[,] plateau)
    {
        int captures = 0;
        int bestCaptures = 0;
        Position bestPosition = null;
        foreach (Position coup in gameState.MouvementsLegaux.Keys)
        {
            captures = gameState.getNombreDePiecesCapturables(coup);
            if (captures > bestCaptures)
            {
                bestCaptures = captures;
                bestPosition = coup;
            } else if (captures == bestCaptures)
            {
                int rand = UnityEngine.Random.Range(0, 2);
                if (rand == 0)
                {
                    bestPosition = coup;
                }
            }
        }
        return bestPosition;
    }
}