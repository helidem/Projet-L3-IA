using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask plateauLayer;

    [SerializeField]
    private Piece pieceNoir;

    [SerializeField]
    private Piece pieceBlanc;

    [SerializeField]
    private GameObject highlightPrefab;

    [SerializeField]
    private UIManager uiManager;

    private Dictionary<Joueur, Piece> prefabPieces = new Dictionary<Joueur, Piece>();
    private GameState gameState = new GameState();
    private Piece[,] pieces = new Piece[8, 8];
    private bool peutJouer = true;
    private List<GameObject> highlights = new List<GameObject>();




    // Start is called before the first frame update
    void Start()
    {
        prefabPieces[Joueur.Noir] = pieceNoir;
        prefabPieces[Joueur.Blanc] = pieceBlanc;

        PiecesDepart();
        AfficherCoupsLegaux();
        uiManager.SetPlayerText(gameState.JoueurActuel);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (gameState.JoueurActuel == Joueur.Blanc) // joueur AI
        {
            OnPlateauClicked(gameState.ai.Jouer(gameState.Plateau));
        }
        else if (gameState.JoueurActuel == Joueur.Noir) // joueur humain
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, plateauLayer))
                {
                    Vector2 impact = hitInfo.point;
                    Position plateauPos = SceneToPlateauPos(impact);
                    OnPlateauClicked(plateauPos);
                }
            }
        }
    }

    //void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Escape))
    //    {
    //        Application.Quit();
    //    }

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, plateauLayer))
    //        {
    //            Vector2 impact = hitInfo.point;
    //            Position plateauPos = SceneToPlateauPos(impact);
    //            OnPlateauClicked(plateauPos);
    //        }
    //    }
    //}



    private void AfficherCoupsLegaux()
    {
        foreach (Position position in gameState.MouvementsLegaux.Keys)
        {
            Vector2 scenePos = PlateauToScenePos(position) + Vector2.up * 0.01f;
            GameObject hl = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(hl);
            gameState.getNombreDePiecesCapturables(position);
        }
    }

    private void CacherCoupsLegaux()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }




    private void OnPlateauClicked(Position plateauPos)
    {
        if (!peutJouer)
        {
            return;
        }

        if (gameState.Jouer(plateauPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnJouer(moveInfo));
        }
    }

    private IEnumerator OnJouer(MoveInfo moveInfo)
    {
        peutJouer = false;
        CacherCoupsLegaux();
        yield return AfficherMouvement(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        AfficherCoupsLegaux();
        peutJouer = true;
    }


    private Position SceneToPlateauPos(Vector2 scenePos)
    {
        int col = (int)(scenePos.x - 0.25f);
        int ligne = 7 - (int)(scenePos.y - 0.25f);
        return new Position(ligne, col);
    }

    private Vector2 PlateauToScenePos(Position plateauPos)
    {
        return new Vector2(plateauPos.Colonne + 0.75f, 7 - plateauPos.Ligne + 0.75f);
    }

    private void PlacerPiece(Piece prefab, Position plateauPos)
    {
        Vector2 scenePos = PlateauToScenePos(plateauPos) + Vector2.up * 0.1f;
        pieces[plateauPos.Ligne, plateauPos.Colonne] = Instantiate(prefab, scenePos, Quaternion.identity);
    }

    private void PiecesDepart()
    {
        foreach (Position plateauPos in gameState.PositionsOccupees())
        {
            Joueur j = gameState.Plateau[plateauPos.Ligne, plateauPos.Colonne];
            PlacerPiece(prefabPieces[j], plateauPos);
        }
    }

    private void RetournerPieces(List<Position> positions)
    {
        foreach (Position piece in positions)
        {
            pieces[piece.Ligne, piece.Colonne].ChangerCouleur();
        }
    }

    private IEnumerator AfficherMouvement(MoveInfo moveInfo)
    {
        PlacerPiece(prefabPieces[moveInfo.Joueur], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        RetournerPieces(moveInfo.Captures);
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator ShowTurnSkipped(Joueur skipped)
    {
        uiManager.SetSkippedText(skipped);
        yield return uiManager.AnimateTopText();
    }

    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (gameState.FinPartie)
        {
            yield return ShowGameOver(gameState.Gagnant);
            yield break;
        }
        Joueur courant = gameState.JoueurActuel;

        if (courant == moveInfo.Joueur)
        {
            // montrer le joueur qui a joué
            yield return ShowTurnSkipped(courant.AutreJoueur());
        }

        uiManager.SetPlayerText(courant);
    }

    private IEnumerator ShowGameOver(Joueur winner)
    {
        uiManager.SetTopText("Partie terminée!");
        yield return uiManager.AnimateTopText();

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);
        yield return ShowCounting();

        uiManager.SetWinnerText(winner);
        yield return uiManager.ShowEndScreen();
    }

    private IEnumerator ShowCounting()
    {
        int noir = 0, blanc = 0;

        foreach (Position pos in gameState.PositionsOccupees())
        {
            Joueur j = gameState.Plateau[pos.Ligne, pos.Colonne];
            if (j == Joueur.Noir)
            {
                noir++;
                uiManager.SetBlackScoreText(noir);
            }
            else
            {
                blanc++;
                uiManager.SetWhiteScoreText(blanc);
            }

            pieces[pos.Ligne, pos.Colonne].Compter();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator RestartGame()
    {
        yield return uiManager.HideEndScreen();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void OnRestartButtonClicked()
    {
        StartCoroutine(RestartGame());
    }

}
