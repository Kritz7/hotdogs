using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using HandType = PlayerHandModule.HandType;

public class PlayerHoldingModule : MonoBehaviour
{
    private Dictionary<HandType, IHoldable> heldCollection;

    public IHoldable Holding(HandType hand) => heldCollection[hand];
    public bool isHolding(HandType hand) => heldCollection[hand] != null;

    private void Awake()
    {
        heldCollection = new Dictionary<HandType, IHoldable>
        {
            { HandType.Left, null },
            { HandType.Right, null }
        };
    }

    private void Update()
    {
        foreach(var hand in heldCollection)
        {
            if (hand.Value == null)
                continue;

            UpdatePosition(hand.Key, heldCollection[hand.Key]);
        }
    }

    private void UpdatePosition(HandType hand, IHoldable holdable)
    {
        Item item = (Item)holdable;
        PlayerHandModule handModule = Player.Main.GetHand(hand);

        item.transform.position = handModule.HandPosition;
    }

    public bool TryHold(HandType hand, PlayerHandModule.InputContext context, Action onHeld = null, Action onDropped = null)
    {
        if (!context.Valid)
            return false;

        if (heldCollection[hand] != null)
            return false;

        foreach (var hit in context.Hits)
        {
            if (hit.collider.GetComponentInParent<IHoldable>() is not IHoldable holdable)
                continue;

            PlayerHandModule handModule = Player.Main.GetHand(hand);

            if (!handModule.WithinContactDistance(((Item)holdable).transform.position))
                continue;

            Hold(hand, holdable, onHeld, onDropped);
            break;
        }

        return heldCollection[hand] != null;
    }

    public void Hold(HandType hand, IHoldable holdable, Action onHeld = null, Action onDrop = null)
    {
        if (heldCollection[hand] != null)
            Debug.LogError($"{hand} cannot hold {holdable}! Already holding {heldCollection[hand]}!");

        heldCollection[hand] = holdable;
        holdable.Hold(onHeld, onDrop);

        onHeld?.Invoke();
    }

    public bool TryDrop(HandType hand, Action onDropSuccess = null)
    {
        if (heldCollection[hand] == null)
            return false;

        Drop(hand, onDropSuccess);

        return true;
    }

    public void Drop(HandType hand, Action onDropSuccess = null)
    {
        heldCollection[hand].Drop(onDropSuccess);
        heldCollection[hand] = null;
    }
}
