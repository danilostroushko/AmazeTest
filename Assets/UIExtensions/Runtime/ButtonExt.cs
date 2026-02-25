using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonExt : Button
{
    public GameObject enableState;
    public GameObject disableState;

    public bool animateOnDisable = false;
    public float duration = 0.1f;
    public float upScale = 1f;
    public float hoverScale = 1f;
    
    public float downScale = 0.97f;

    public Ease hoverEase = Ease.Linear;
    public Ease downEase = Ease.Linear;

    public AnimationCurve hoverScaleEase = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
    public AnimationCurve downScaleEase = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    public bool autoSelectAO = false;
    public Transform animationObject;

    private RectTransform _rectTransform;
    public RectTransform rectTransform 
    {
        get 
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    private CanvasGroup _canvasGroup;
    public CanvasGroup canvasGroup
    {
        get
        {
            if (TryGetComponent<CanvasGroup>(out var comp))
            {
                _canvasGroup = comp;
            }
            else
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            return _canvasGroup;
        }
    }

    protected override void OnDestroy()
    {
        if (animationObject != null) { animationObject.DOKill(); }
        base.OnDestroy();
    }

    public override bool IsInteractable()
    {
        if (enableState != null) { enableState.SetActive(interactable); }
        if (disableState != null) { disableState.SetActive(!interactable); }
        return base.IsInteractable();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (!interactable && !animateOnDisable) { return; }
        animationObject = GetTransform();

        animationObject.DOScale(hoverScale, duration).SetEase(hoverEase).SetUpdate(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (!interactable && !animateOnDisable) { return; }
        animationObject = GetTransform();

        animationObject.DOScale(1f, duration).SetEase(Ease.Linear).SetUpdate(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!interactable && !animateOnDisable) { return; }
        animationObject = GetTransform();

        animationObject.DOScale(downScale, duration).SetEase(downEase).SetUpdate(true);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (!interactable && !animateOnDisable) { return; }
        animationObject = GetTransform();

        animationObject.DOScale(upScale, duration).SetEase(Ease.Linear).SetUpdate(true);
    }

    private Transform GetTransform()
    {
        if (autoSelectAO && enableState != null && disableState != null)
        {
            return IsInteractable() ? enableState.transform : disableState.transform;
        }

        return animationObject == null ? transform : animationObject;
    }

    public List<TextMeshProUGUI> GetLabels() => GetComponentsInChildren<TextMeshProUGUI>(true).ToList();
}