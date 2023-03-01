using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField]
    private Color normal;

    [SerializeField]
    private Color hover;

    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        material.color = normal;
    }

    private void OnMouseEnter()
    {
        material.color = hover;
    }

    private void OnMouseExit()
    {
        material.color = normal;
    }

    private void OnDestroy()
    {
        Destroy(material);
    }
}
