using System.Collections.Generic;
using UnityEngine;

public class AIPlayer 
{
    private GameState gameState;
    private Joueur joueur;

    public int difficulte { get; private set;}

    public AIPlayer(GameState gameState, Joueur joueur, int difficulte)
    {
        this.gameState = gameState;
        this.joueur = joueur;
        this.difficulte = difficulte;
    }

    public Position Jouer(int difficulte)
    {
        if (difficulte == 1)
        {
            return JouerFacile();
        }
        else if (difficulte == 2)
        {
            return JouerMoyen();
        }
        else
        {
            return JouerDifficile();
        }
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
        List<Position> positions = new List<Position>(gameState.MouvementsLegaux.Keys);
        int index = Random.Range(0, positions.Count);
        return positions[index];
    }
}