using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField]
    private Joueur top;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangerCouleur()
    {
        if (top == Joueur.Noir)
        {
            animator.Play("NoirToBlanc");
            top = Joueur.Blanc;
        }
        else
        {
            animator.Play("BlancToNoir");
            top = Joueur.Noir;
        }
    }

    public void Compter()
    {
        animator.Play("CompterPieces");
    }

}
