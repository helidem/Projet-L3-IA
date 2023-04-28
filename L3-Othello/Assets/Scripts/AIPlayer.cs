using UnityEngine;

public class AIPlayer
{
    private GameState gameState;

    private Evaluator evaluator = new Evaluator();

    public int difficulte { get; private set; }

    public AIPlayer(GameState gameState, int difficulte)
    {
        this.gameState = gameState;
        this.difficulte = difficulte;
    }

    public Position Jouer(Joueur[,] plateau, Joueur joueur, int difficulte)
    {
        switch (difficulte)
        {
            case 1:
                return Glouton(plateau); // glouton
            case 2:
                return solve(plateau, joueur, 3, evaluator, false); // minmax
            case 3:
                return solve(plateau, joueur, 5, evaluator, true); // minmax avec alpha beta
            default:
                return Glouton(plateau);
        }
    }


    public Position solve(Joueur[,] plateau, Joueur joueur, int profondeur, Evaluator e, bool ab)
    {
        int bestScore = int.MinValue;
        int score;
        Position bestMove = null;
        foreach (Position move in gameState.MouvementsLegaux.Keys)
        {
            Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
            if (ab) // alpha beta
            {
                score = MMAB(newPlateau, joueur, profondeur - 1, false, int.MinValue, int.MaxValue, e);
            }
            else // minmax
            {
                score = MM(newPlateau, joueur, profondeur - 1, false, e);
            }
            if (score >= bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }


    private int MMAB(Joueur[,] plateau, Joueur joueur, int profondeur, bool maximizingPlayer, int alpha, int beta, Evaluator e)
    {
        if (profondeur == 0 || gameState.FinPartie)
        {
            return e.eval(plateau, joueur, gameState); // calcul de la valeur de la feuille (heuristique)
        }
        if (maximizingPlayer) // joueur max
        {
            int bestScore = int.MinValue;
            foreach (Position move in gameState.ListeMouvementsLegaux(joueur, plateau).Keys)
            {
                Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
                int score = MMAB(newPlateau, joueur, profondeur - 1, false, alpha, beta, e); // appel recursif (joueur min)
                bestScore = Mathf.Max(score, bestScore); // max entre le score et le bestScore
                alpha = Mathf.Max(alpha, bestScore); // max entre alpha et bestScore
                if (beta <= alpha) // coupure alpha beta
                {
                    break;
                }
            }
            return bestScore;
        }
        else // joueur min
        {
            int bestScore = int.MaxValue;
            foreach (Position move in gameState.ListeMouvementsLegaux(joueur.AutreJoueur(), plateau).Keys)
            {
                Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
                int score = MMAB(newPlateau, joueur, profondeur - 1, true, alpha, beta, e); // appel recursif (joueur max)
                bestScore = Mathf.Min(score, bestScore); // min entre le score et le bestScore
                beta = Mathf.Min(beta, bestScore);
                if (beta <= alpha) // coupure alpha beta
                {
                    break;
                }
            }
            return bestScore;
        }
    }

    private int MM(Joueur[,] plateau, Joueur joueur, int profondeur, bool max, Evaluator e)
    {
        if (profondeur == 0 || gameState.FinPartie)
        {
            return e.eval(plateau, joueur, gameState);
        }

        int score;
        if (max)
        {
            score = int.MinValue;
            foreach (Position move in gameState.ListeMouvementsLegaux(joueur, plateau).Keys)
            {
                Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
                score = Mathf.Max(score, MM(newPlateau, joueur, profondeur - 1, false, e));
            }
        }
        else
        {
            score = int.MaxValue;
            foreach (Position move in gameState.ListeMouvementsLegaux(joueur.AutreJoueur(), plateau).Keys)
            {
                Joueur[,] newPlateau = gameState.getNewPlateauAfterMove(plateau, move);
                score = Mathf.Min(score, MM(newPlateau, joueur, profondeur - 1, true, e));
            }

        }
        return score;
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
            }
            else if (captures == bestCaptures)
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