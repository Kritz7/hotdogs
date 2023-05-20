using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotDogs
{
    public interface IHoldable
    {
        public delegate void OnHeld();
        public delegate void OnDropped();

        public void Hold(Action onHold = null, Action onDrop = null);
        public void OnHold();

        public void Drop(Action onDropSuccess = null);
        public void OnDrop();
    }
}