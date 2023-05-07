using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerHandModule : MonoBehaviour
{
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("handType")] private HandType hand;
    [SerializeField] private float handMovementSpeed = 10f;
    [SerializeField] private float defaultHandDistance = 1f;
    [SerializeField] private GameObject handObject;
    [SerializeField] private AnimationCurve handDistanceOffset;

    private LineRenderer _handLine;
    private Sequence handMoveSequence;
    private Vector3 goalMovePosition;
    private bool slappy = false;
    private float contactDistance = 0.4f;

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

    private void OnInputHeld()
    {
        InputContext context = new InputContext(this);

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

    private void OnInputReleased()
    {
        InputContext context = new InputContext(this);
        Player.Main.Holding.TryDrop(Hand);

        ResetSlapTrigger();
        AnimateHandOut();
    }

    private void OnInputPressed()
    {
        InputContext context = new InputContext(this);

        if (Player.Main.Holding.TryHold(Hand, context))
        {
            PlaySlap();
        }

        bool defaultHandPos = !context.Valid || isHolding;

        AnimateHandIn(defaultHandPos ? GetDefaultHandEndPosition(defaultHandDistance) : context.First.point);
    }

    private void TryPlaySlap(InputContext context)
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

    protected Vector3 GetHandStartPos()
    {
        Vector3 offset = hand switch
        {
            HandType.Left => -transform.right,
            HandType.Right => transform.right,
            HandType.None => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException()
        };

        offset *= 0.8f;
        Vector3 verticalOffset = Vector3.up;

        return transform.position + offset + verticalOffset;
    }

    protected Vector3 GetDefaultHandEndPosition(float distance)
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
    public class InputContext
    {
        public List<RaycastHit> Hits = new List<RaycastHit>();
        public bool Valid => Hits != null && Hits.Count > 0;
        public RaycastHit First => Valid ? Hits[0] : new RaycastHit();

        public InputContext(Vector3 origin, Vector3 direction, float distance)
        {
            Hits = CameraUtilities.SteppedSpherecast(origin, direction, distance).ToList();
        }

        public InputContext(PlayerHandModule hand, bool allowDebugging = false)
        {
            Vector3 handDirection = (hand.GetDefaultHandEndPosition(hand.defaultHandDistance) - hand.GetHandStartPos()).normalized;
            float screenOffsetPercent = hand.hand == HandType.Left ? 0.4f : 0.6f;
            float screenCloseOffsetPercent = hand.hand == HandType.Left ? 0.45f : 0.55f;
            float screenFarOffsetPercent = hand.hand == HandType.Left ? 0.3333f : 0.6667f;

            Hits = new List<RaycastHit>();
            Hits.AddRange(CameraUtilities.SteppedSpherecastFromScreenCentre(hand.defaultHandDistance * 0.8f));
            Hits.AddRange(CameraUtilities.SteppedSpherecastFromScreen(hand.defaultHandDistance, screenOffsetPercent));
            Hits.AddRange(CameraUtilities.SteppedSpherecastFromScreen(hand.defaultHandDistance, screenCloseOffsetPercent));

            if(Hits.Count == 0)
                Hits.AddRange(CameraUtilities.SteppedSpherecastFromScreen(hand.defaultHandDistance, screenFarOffsetPercent));

            if (Hits.Count == 0)
                Hits.AddRange(CameraUtilities.SteppedSpherecast(hand.GetHandStartPos(), handDirection, hand.defaultHandDistance, screenOffsetPercent));

            Hits = Hits.Where(x => x.distance > 0.01f).ToList();

            // Sort the hits based on distance then by item
            int itemsHit = Hits.Count((x) => x.transform.GetComponentInParent<IHoldable>() != null);

            if(itemsHit > 0)
            {
                Hits = Hits.OrderBy(x => x.transform.GetComponentInParent<IHoldable>() == null).ToList();
            }
            else
            {
                Hits = Hits.OrderBy(x => x.distance).ToList();
            }

            if (allowDebugging)
            {
                Color randColor = Random.ColorHSV();
                string output = "";
                for (int i = 0; i < Hits.Count; i++)
                {
                    output += $"{i}_{Hits[i].transform.name} {Hits[i].distance}, ";
                    WorldDebugHelper.ShowText(Hits[i].point, $"{i}_{Hits[i].transform.name}", 5f, randColor);
                }

                Debug.Log(output);
            }
        }
    }

    public enum HandType
    {
        None,
        Left,
        Right
    }
}
