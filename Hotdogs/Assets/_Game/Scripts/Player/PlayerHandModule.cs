using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerHandModule : MonoBehaviour
{
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("handType")] private HandType hand;
    [SerializeField] private float handMovementSpeed = 10f;
    [SerializeField] private float defaultHandDistance = 2f;
    [SerializeField] private GameObject handObject;
    [SerializeField] private AnimationCurve handDistanceOffset;

    private LineRenderer _handLine;
    private Sequence handMoveSequence;
    private Vector3 goalMovePosition;
    private bool slappy = false;
    private float contactDistance = 0.5f;

    public HandType Hand => hand;
    public Vector3 HandPosition => goalMovePosition;
    public bool WithinContactDistance(Vector3 position) => Vector3.Distance(handObject.transform.position, position) < contactDistance;


    private bool isHolding => Player.Main.Holding.isHolding(Hand);
    private LineRenderer handLine => _handLine ??= handObject.GetComponent<LineRenderer>();


    #region Unity Lifecycle

    private void OnEnable()
    {
        CoroutineHelper.PerformAfterCondition(this, AssignListeners, () => InputManager.Exists);
    }

    private void OnDisable()
    {
        UnassignListeners();
    }

    #endregion

    #region Listeners

    private void AssignListeners()
    {
        if (hand == HandType.Left)
        {
            InputManager.Instance.onLeftClickPressed += OnInputPressed;
            InputManager.Instance.onLeftClickReleased += OnInputReleased;
            InputManager.Instance.onLeftClickHeld += OnInputHeld;
        }
        if (hand == HandType.Right)
        {
            InputManager.Instance.onRightClickPressed += OnInputPressed;
            InputManager.Instance.onRightClickReleased += OnInputReleased;
            InputManager.Instance.onRightClickHeld += OnInputHeld;
        }
    }

    private void UnassignListeners()
    {
        if (hand == HandType.Left)
        {
            InputManager.Instance.onLeftClickPressed -= OnInputPressed;
            InputManager.Instance.onLeftClickReleased -= OnInputReleased;
            InputManager.Instance.onLeftClickHeld -= OnInputHeld;
        }
        if (hand == HandType.Right)
        {
            InputManager.Instance.onRightClickPressed -= OnInputPressed;
            InputManager.Instance.onRightClickReleased -= OnInputReleased;
            InputManager.Instance.onRightClickHeld -= OnInputHeld;
        }
    }

    #endregion


    private void Update()
    {
        handObject.transform.position = Vector3.Lerp(handObject.transform.position, goalMovePosition, handMovementSpeed * Time.deltaTime);
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

    private void OnInputHeld(InputManager.InputContext context)
    {
        if (handMoveSequence != null && !handMoveSequence.IsActive())
            return;

        if(!isHolding)
        {
            TryPlaySlap(context);

            if(Player.Main.Holding.TryHold(Hand, context))
            {
                PlaySlap();
                handMoveSequence.Kill();
            }
        }

        bool defaultHandPos = !context.Valid || isHolding;

        goalMovePosition = defaultHandPos ? GetDefaultHandEndPosition(defaultHandDistance) : context.First.point;
    }

    private void OnInputReleased(InputManager.InputContext context)
    {
        Player.Main.Holding.TryDrop(Hand);

        ResetSlapTrigger();
        AnimateHandOut();
    }

    private void OnInputPressed(InputManager.InputContext context)
    {
        if(Player.Main.Holding.TryHold(Hand, context))
        {
            PlaySlap();
        }

        bool defaultHandPos = !context.Valid || isHolding;

        AnimateHandIn(defaultHandPos ? GetDefaultHandEndPosition(defaultHandDistance) : context.First.point);
    }

    private void TryPlaySlap(InputManager.InputContext context)
    {
        if (context.Valid && WithinContactDistance(context.First.point) && slappy == false)
        {
            slappy = true;
            PlaySlap();
        }

        if (!context.Valid)
            slappy = false;
    }

    private void PlaySlap()
    {
        SoundManager.PlayInteractionSFX();
    }

    private void ResetSlapTrigger()
    {
        slappy = false;
    }

    private void AnimateHandOut()
    {
        if (handMoveSequence != null)
            handMoveSequence.Kill();

        handMoveSequence = DOTween.Sequence();
        handMoveSequence.Append(DOTween.To(
            () => handObject.transform.position,
            (x) => goalMovePosition = x,
            GetHandStartPos(), 0.2f).SetEase(Ease.OutSine));

        handMoveSequence.OnKill(() =>
        {
            handObject.gameObject.SetActive(false);
            handMoveSequence = null;
        });
    }

    private void AnimateHandIn(Vector3 movePosition)
    {
        if (handMoveSequence != null)
            handMoveSequence.Kill();

        handMoveSequence = DOTween.Sequence();
        handMoveSequence.OnStart(() =>
        {
            handObject.gameObject.SetActive(true);
            goalMovePosition = GetHandStartPos();
        });

        handMoveSequence.Append(DOTween.To(
            () => handObject.transform.position,
            (x) => goalMovePosition = x,
            movePosition, 0.1f).SetEase(Ease.OutSine));

        handMoveSequence.OnKill(() => handMoveSequence = null);
    }

    private Vector3 GetHandStartPos()
    {
        Vector3 offset = hand switch
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
        Vector3 forwardOffset = Camera.main.transform.forward * distance;
        Vector3 verticalOffset = Vector3.up;
        Vector3 perpendicularOffset = hand switch
        {
            HandType.Left => -transform.right,
            HandType.Right => transform.right,
            HandType.None => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException()
        };

        perpendicularOffset = perpendicularOffset * 0.5f;

        return transform.position + perpendicularOffset + forwardOffset + verticalOffset;
    }

    public enum HandType
    {
        None,
        Left,
        Right
    }
}
