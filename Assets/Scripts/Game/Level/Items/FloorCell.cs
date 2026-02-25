using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCell : MonoBehaviour
{
    public event Action OnFilled;

    [SerializeField] private SpriteRenderer cellSprite;

    public bool Filled { get; private set; } = false;

    private Color fillColor;

    public void Init(Color fillColor)
    {
        this.fillColor = fillColor;
    }

    public void SetFilled()
    {
        if (!Filled)
        {
            Filled = true;
            cellSprite.DOColor(fillColor, 0.3f);
            OnFilled?.Invoke();
        }
    }
}