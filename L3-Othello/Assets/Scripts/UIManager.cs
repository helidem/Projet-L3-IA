using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI blackScoreText;
    [SerializeField] private TextMeshProUGUI whiteScoreText;
    [SerializeField] private TextMeshProUGUI winnerText;

    [SerializeField] private Image blackOverlay;

    [SerializeField] private RectTransform playAgainButton;

    public void SetPlayerText(Joueur courant) {
        if (courant == Joueur.Blanc) {
            topText.text = "Tour des Blancs <sprite name=DiscWhiteUp>";
        } else if (courant == Joueur.Noir) {
            topText.text = "Tour des Noirs <sprite name=DiscBlackUp>";
        }
    }

    public void SetSkippedText(Joueur skipped)
    {
        if (skipped == Joueur.Blanc)
        {
            topText.text = "Blancs peuvent pas jouer ! <sprite name=DiscWhiteUp>";
        } else if (skipped == Joueur.Noir)
        {
            topText.text = "Noirs peuvent pas jouer ! <sprite name=DiscBlackUp>";
        }
    }

    public void SetTopText(string text) {
        topText.text = text;
    }

    public IEnumerator AnimateTopText() {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);
    }

    private IEnumerator ScaleDown(RectTransform rectTransform) {
        rectTransform.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rectTransform.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rectTransform) {
        rectTransform.gameObject.SetActive(true);
        rectTransform.localScale = Vector3.zero;
        rectTransform.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText(){
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreText(int score) {
        blackScoreText.text = $"<sprite name=DiscBlackUp> {score}";
    }

    public void SetWhiteScoreText(int score) {
        whiteScoreText.text = $"<sprite name=DiscWhiteUp> {score}";
    }

    private IEnumerator ShowOverlay() {
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1f);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator HideOverlay() {
        blackOverlay.rectTransform.LeanAlpha(0f, 1f);
        yield return new WaitForSeconds(1f);
        blackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoreDown(){
        blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(Joueur winner) {
        switch (winner)
        {
            case Joueur.Blanc:
                winnerText.text = "Les Blancs ont gagné !>";
                break;
            case Joueur.Noir:
                winnerText.text = "Les Noirs ont gagné !>";
                break;
            case Joueur.Vide:
                winnerText.text = "Match nul !";
                break;
        }
    }

    public IEnumerator ShowEndScreen() {
        yield return ShowOverlay();
        yield return MoveScoreDown();
        yield return ScaleUp(winnerText.rectTransform);
        yield return ScaleUp(playAgainButton);
    }

    public IEnumerator HideEndScreen() {
        yield return ScaleDown(winnerText.rectTransform);
        yield return ScaleDown(blackScoreText.rectTransform);
        yield return ScaleDown(whiteScoreText.rectTransform);
        yield return ScaleDown(playAgainButton);

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }

}
