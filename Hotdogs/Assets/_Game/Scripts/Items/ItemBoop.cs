using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using HotDogs.HDUtils;

namespace HotDogs
{
#if UNITY_EDITOR
    // Allow preview and editing outside of playmode.
    [ExecuteAlways]
#endif
    /// <summary>
    /// always be booping
    /// </summary>
    public class ItemBoop : MonoBehaviour
    {
        [Header("Always be Booping")]
        [SerializeField] private float bopSpeed = 10;
        [SerializeField,Range(0f, 100f), Tooltip("How fast will this bop match the lerp curve? faster = more responsive")] private float lerpMatchSpeed = 20;
        [SerializeField] private bool startImmediately = true;

        [Header("Looping")]
        [SerializeField] private bool loopForever = true;
        [SerializeField, ConditionalField(nameof(loopForever), true)] private int loops = 1;
        [SerializeField, ConditionalField(nameof(loopForever), true)]
        [Tooltip("Note: this does not change the curve length, you are (manually) telling the script how long a bop goes for based on the curves.")]
        private float loopDuration = 2f; // could work this out from the max time value of all curves, but, that might be more confusing?
        [SerializeField, ConditionalField(nameof(loopForever), true)]
        [Tooltip("If the looping gets stopped, how long should this item take to return to initial pos+scale? 0 = does not reset automatically")]
        private float resetDuration = 1;

        [Header("Booping")]
        [SerializeField] private BoopingScriptable boop;
        [SerializeField] private bool useLocalPosition = true;

        private bool bopping = false;
        private Sequence resetSequence = null;
        private Vector3 initialPosition, initialScale;
        private Vector3 goalPosition, goalScale;
        private float currentLifetime;
        private int currentLoopsCompleted;

#if UNITY_EDITOR
        private bool PreviewInEditor = false;

        // TODO: would love to use ConditionalFields here but they don't seem to work on ButtonMethods in this version of MyBox
        [ButtonMethod]
        public void StartPreview()
        {
            if (boop == null)
            {
                Debug.LogError("Please assign a boop style!");
                return;
            }

            ResetInitialPositions();
            PreviewInEditor = true;
        }

        [ButtonMethod]
        public void StopPreview()
        {
            PreviewInEditor = false;
            ResetToInitialPositions();
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying || !PreviewInEditor) return;
            if (boop == null) return;

            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
#endif

        private void Awake()
        {
            ResetInitialPositions();
        }

        private void OnEnable()
        {
            if (startImmediately)
                StartBopping();
        }

        private void Update()
        {
            currentLifetime += Time.deltaTime;
            
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if(PreviewInEditor)
                    BopUpdate();
                return;
            }
            #endif
            
            if (!bopping)
                return;

            if (!loopForever && currentLoopsCompleted >= loops)
                return;

            BopUpdate();

            if (!loopForever && loopDuration != 0)
            {
                currentLoopsCompleted = Mathf.FloorToInt(currentLifetime % loopDuration);
            }

            if(currentLoopsCompleted >= loops)
            {
                StopBopping();
            }
        }

        private void BopUpdate()
        {
            float xOffset = GetOffset(boop.XPositionBop, boop.MinPositionOffset.x, boop.MaxPositionOffset.x);
            float yOffset = GetOffset(boop.YPositionBop, boop.MinPositionOffset.y, boop.MaxPositionOffset.y);
            float zOffset = GetOffset(boop.ZPositionBop, boop.MinPositionOffset.z, boop.MaxPositionOffset.z);

            float xScale = GetOffset(boop.XScaleBop, boop.MinScaleOffset.x, boop.MaxScaleOffset.x);
            float yScale = GetOffset(boop.YScaleBop, boop.MinScaleOffset.y, boop.MaxScaleOffset.y);
            float zScale = GetOffset(boop.ZScaleBop, boop.MinScaleOffset.z, boop.MaxScaleOffset.z);

            goalScale = new Vector3(xScale, yScale, zScale);
            goalPosition = new Vector3(xOffset, yOffset, zOffset);
            goalPosition = useLocalPosition ? transform.InverseTransformDirection(goalPosition) : goalPosition;

            transform.position = Vector3.Lerp(transform.position, initialPosition + goalPosition, lerpMatchSpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale + goalScale, lerpMatchSpeed * Time.deltaTime);
        }
        
        public void ResetInitialPositions()
        {
            initialPosition = transform.position;
            initialScale = transform.localScale;
        }

        public void ResetToInitialPositions()
        {
            transform.position = initialPosition;
            transform.localScale = initialScale;
        }

        public void StartBopping()
        {
            bopping = true;
            currentLifetime = 0;
            currentLoopsCompleted = 0;
        }

        public void StopBopping()
        {
//          throw new NotImplementedException("Always be bopping");
            bopping = false;
        }

        private void ResetToInitialPositionSequence()
        {
            if (resetDuration == 0)
                return;

            if(resetSequence != null)
            {
                resetSequence.Kill();
                resetSequence = null;
            }

            resetSequence = DOTween.Sequence();

            resetSequence.Join(transform.DOMove(initialPosition, resetDuration));
            resetSequence.Join(transform.DOScale(initialScale, resetDuration));
            resetSequence.OnComplete(ResetToInitialPositions);
            resetSequence.Play();
        }

        private float GetOffset(AnimationCurve curve, float min, float max) =>
            Utilities.Clerp(curve, min, max, currentLifetime * bopSpeed);
    }
}
