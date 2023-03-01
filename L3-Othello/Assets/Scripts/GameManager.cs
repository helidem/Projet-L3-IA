using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

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

    private void AfficherCoupsLegaux()
    {
        foreach(Position position in gameState.MouvementsLegaux.Keys)
        {
            Vector2 scenePos = PlateauToScenePos(position) + Vector2.up * 0.01f;
            GameObject hl = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(hl);
        }
    }

    private void CacherCoupsLegaux()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }




    private void OnPlateauClicked(Position plateauPos)
    {
        if(!peutJouer) 
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
        foreach (Position plateauPos in gameState.PositionsOccupées())
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
        RetournerPieces(moveInfo.Capturés);
        yield return new WaitForSeconds(0.83f);
    }

}
