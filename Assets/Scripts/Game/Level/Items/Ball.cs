using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private MeshRenderer ballMesh;

    [Space]
    [ReadOnly]
    public Vector2Int GridPosition = new Vector2Int();

    [SerializeField] private AnimationCurve moveScaleEase = new AnimationCurve();

    private bool isMoving = false;

    public void Init(Color ballColor)
    {
        ballMesh.material.color = ballColor;
    }

    public void MoveTo(Vector2Int target, float speed)
    {
        if (isMoving) { return; }
        isMoving = true;

        float duration = speed * Vector2.Distance(GridPosition, target);
        
        //Vector3 direction = (GridPosition - target).normalized;
        GridPosition = target;

        //ballMesh.transform.DOScale();
        transform.DOLocalMove(new Vector3(GridPosition.x, 0, -GridPosition.y), duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => isMoving = false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<FloorCell>(out var cell))
        {
            cell.SetFilled();
        }
    }

    
}