using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Extensions/CanvasGroupRect"), RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
public class CanvasGroupRect : UIBehaviour
{
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    private Sequence animSequence;

    public CanvasGroup CanvasGroup 
    {
        get
        {
            if (_canvasGroup == null) { _canvasGroup = GetComponent<CanvasGroup>(); }
            return _canvasGroup;
        }
    }
    public RectTransform RectTransform {
        get
        {
            if (_rectTransform == null) { _rectTransform = GetComponent<RectTransform>(); }
            return _rectTransform;
        }
    }

    public Tween DOAnchorPosAndFade(Vector2 pos, float fade, float fadeFrom = 0f, float duration = 0.25f)
    {
        animSequence?.Kill();

        animSequence = DOTween.Sequence();
        animSequence.Append(RectTransform.DOAnchorPos(pos, duration));
        animSequence.Join(CanvasGroup.DOFade(fade, duration).From(fadeFrom));

        return animSequence.Play();
    }

    public Tween DOPunchScaleAndFade(float scale, float fade, float fadeFrom = 0f, float duration = 0.25f)
    {
        animSequence?.Kill();

        animSequence = DOTween.Sequence();
        animSequence.Append(RectTransform.DOPunchScale(Vector3.one * scale, duration, 1));
        animSequence.Join(CanvasGroup.DOFade(fade, duration).From(fadeFrom));

        return animSequence.Play();
    }
}