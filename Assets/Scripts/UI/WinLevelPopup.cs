using AssetKits.ParticleImage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WinLevelPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private ParticleImage confetti;
    [SerializeField] private TextMeshProUGUI rewardLabel;

    private void ResetPopup()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        rewardLabel.rectTransform.anchoredPosition = Vector2.zero;
        rewardLabel.text = "";
    }

    public async UniTask PlayAnimation()
    {
        rewardLabel.text = $"+{Random.Range(50, 500)}";

        canvasGroup.blocksRaycasts = true;
        confetti.Play();

        Sequence winAnimation = DOTween.Sequence();

        winAnimation.Append(canvasGroup.DOFade(1f, 0.2f));
        winAnimation.Append(rewardLabel.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2));
        winAnimation.Join(rewardLabel.DOFade(1f, 0.3f).From(0f));
        winAnimation.Append(rewardLabel.rectTransform.DOAnchorPosY(250f, 1.2f));
        winAnimation.Join(rewardLabel.DOFade(0f, 1.2f));
        winAnimation.Append(canvasGroup.DOFade(0f, 0.2f));
        winAnimation.AppendCallback(ResetPopup);

        await winAnimation.Play().AsyncWaitForCompletion();
    }
}
