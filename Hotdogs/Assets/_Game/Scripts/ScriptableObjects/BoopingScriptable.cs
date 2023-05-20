using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable, CreateAssetMenu(fileName = "New Boop Style", menuName = "HotDogs/Booping/Boop Style")]
public class BoopingScriptable : ScriptableObject
{
    [Header("Position Bopping")]
    [SerializeField] private Vector3 minPositionOffset;
    [SerializeField] private Vector3 maxPositionOffset;
    [SerializeField] private AnimationCurve xPositionBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);
    [SerializeField] private AnimationCurve yPositionBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);
    [SerializeField] private AnimationCurve zPositionBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);

    [Header("Scale Bopping")]
    [SerializeField] private Vector3 minScaleOffset;
    [SerializeField] private Vector3 maxScaleOffset;
    [SerializeField] private AnimationCurve xScaleBop = AnimationCurve.EaseInOut(0, 0.5f, 1f, 0.5f);
    [SerializeField] private AnimationCurve yScaleBop = AnimationCurve.EaseInOut(0, 0.5f, 1f, 0.5f);
    [SerializeField] private AnimationCurve zScaleBop = AnimationCurve.EaseInOut(0, 0.5f, 1f, 0.5f);

    public Vector3 MinPositionOffset => minPositionOffset;
    public Vector3 MaxPositionOffset => maxPositionOffset;
    public AnimationCurve XPositionBop => xPositionBop;
    public AnimationCurve YPositionBop => yPositionBop;
    public AnimationCurve ZPositionBop => zPositionBop;


    public Vector3 MinScaleOffset => minScaleOffset;
    public Vector3 MaxScaleOffset => maxScaleOffset;
    public AnimationCurve XScaleBop => xScaleBop;
    public AnimationCurve YScaleBop => yScaleBop;
    public AnimationCurve ZScaleBop => zScaleBop;

#if UNITY_EDITOR
    [ButtonMethod]
    public void ResetCurvesToLinear()
    {
        EditorUtility.SetDirty(this);
        Undo.RecordObject(this, "Reset Animation Curves");

        xPositionBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);
        yPositionBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);
        zPositionBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);
        xScaleBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);
        yScaleBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);
        zScaleBop = AnimationCurve.Linear(0, 0.5f, 1f, 0.5f);

        xPositionBop.postWrapMode = WrapMode.PingPong;
        yPositionBop.postWrapMode = WrapMode.PingPong;
        zPositionBop.postWrapMode = WrapMode.PingPong;
        xScaleBop.postWrapMode = WrapMode.PingPong;
        yScaleBop.postWrapMode = WrapMode.PingPong;
        zScaleBop.postWrapMode = WrapMode.PingPong;

        xPositionBop.preWrapMode = WrapMode.PingPong;
        yPositionBop.preWrapMode = WrapMode.PingPong;
        zPositionBop.preWrapMode = WrapMode.PingPong;
        xScaleBop.preWrapMode = WrapMode.PingPong;
        yScaleBop.preWrapMode = WrapMode.PingPong;
        zScaleBop.preWrapMode = WrapMode.PingPong;

        AssetDatabase.SaveAssets();
        EditorUtility.ClearDirty(this);
    }
#endif
}
