using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerHandModule : MonoBehaviour
{
    [SerializeField] private HandType handType;
    [SerializeField] private GameObject handObject;
    [SerializeField] private AnimationCurve handDistanceOffset;

    private LineRenderer handLine => _handLine ??= handObject.GetComponent<LineRenderer>();
    private LineRenderer _handLine;

    private Sequence handMoveSequence;
    private bool slappy = false;
    private float stickDistance = 0.5f;

    private bool InStickingDistance(Vector3 position) => Vector3.Distance(handObject.transform.position, position) < stickDistance;

    private void OnEnable()
    {
        CoroutineHelper.PerformAfterCondition(this, AssignListeners, () => InputManager.Exists);
    }

    private void OnDisable()
    {
        UnassignListeners();
    }

    private void LateUpdate()
    {
        if(handLine)
        {
            handLine.SetPositions(new Vector3[]
            {
                GetHandStartPos(),
                handObject.transform.position
            });
        }
    }
    private void AssignListeners()
    {
        if (handType == HandType.Left)
        {
            InputManager.Instance.onLeftClickPressed += OnInputPressed;
            InputManager.Instance.onLeftClickReleased += OnInputReleased;
            InputManager.Instance.onLeftClickHeld += OnInputHeld;
        }
        if (handType == HandType.Right)
        {
            InputManager.Instance.onRightClickPressed += OnInputPressed;
            InputManager.Instance.onRightClickReleased += OnInputReleased;
            InputManager.Instance.onRightClickHeld += OnInputHeld;
        }
    }

    private void UnassignListeners()
    {
        if (handType == HandType.Left)
        {
            InputManager.Instance.onLeftClickPressed -= OnInputPressed;
            InputManager.Instance.onLeftClickReleased -= OnInputReleased;
            InputManager.Instance.onLeftClickHeld -= OnInputHeld;
        }
        if (handType == HandType.Right)
        {
            InputManager.Instance.onRightClickPressed -= OnInputPressed;
            InputManager.Instance.onRightClickReleased -= OnInputReleased;
            InputManager.Instance.onRightClickHeld -= OnInputHeld;
        }
    }

    private void OnInputHeld(InputManager.InputContext context)
    {
        if (handMoveSequence != null && !handMoveSequence.IsActive())
            return;

        if (context.Valid && InStickingDistance(context.First.point) && slappy == false)
        {
            slappy = true;
            SoundManager.PlayInteractionSFX();
        }

        if(!context.Valid)
        {
            slappy = false;
        }

        Vector3 movePosition = context.Valid ? context.First.point : GetDefaultHandEndPosition(2f);

        handObject.transform.position = Vector3.Lerp(handObject.transform.position, movePosition, 10 * Time.deltaTime);
    }

    private void OnInputReleased(InputManager.InputContext context)
    {
        slappy = false;

        if (handMoveSequence != null)
        {
            handMoveSequence.Kill();
        }

        handMoveSequence = DOTween.Sequence();
        handMoveSequence.Append(DOTween.To(
            () => handObject.transform.position,
            (x) => handObject.transform.position = x,
            GetHandStartPos(), 0.2f).SetEase(Ease.OutSine));

        handMoveSequence.OnKill(() =>
        {
            handObject.gameObject.SetActive(false);
            handMoveSequence = null;
        });
    }

    private void OnInputPressed(InputManager.InputContext context)
    {
        if(handMoveSequence != null)
        {
            handMoveSequence.Kill();
        }

        Vector3 movePosition = context.Valid ? context.First.point : GetDefaultHandEndPosition(2f);

        handMoveSequence = DOTween.Sequence();
        handMoveSequence.OnStart(() =>
        {
            handObject.gameObject.SetActive(true);
            handObject.transform.position = GetHandStartPos();
        });

        handMoveSequence.Append(DOTween.To(
            () => handObject.transform.position,
            (x) => handObject.transform.position = x,
            movePosition, 0.1f).SetEase(Ease.OutSine));

        handMoveSequence.OnKill(() => handMoveSequence = null);
    }

    private Vector3 GetHandStartPos()
    {
        Vector3 offset = handType switch
        {
            HandType.Left => -transform.right,
            HandType.Right => transform.right,
            HandType.None => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException()
        };

        return transform.position + offset;
    }

    private Vector3 GetDefaultHandEndPosition(float distance)
    {
        Vector3 forwardOffset = transform.forward * distance;
        Vector3 perpendicularOffset = handType switch
        {
            HandType.Left => -transform.right,
            HandType.Right => transform.right,
            HandType.None => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException()
        };

        perpendicularOffset = handDistanceOffset.Clerp(perpendicularOffset, Vector3.zero, distance);

        return transform.position + perpendicularOffset + forwardOffset;
    }

    public enum HandType
    {
        None,
        Left,
        Right
    }
}
