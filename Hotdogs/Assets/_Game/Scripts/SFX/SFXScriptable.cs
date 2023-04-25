using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New SFX Item", menuName = "HotDogs/SFX/SFX Item")]
public class SFXScriptable : ScriptableObject
{
    [SerializeField] private AudioClip clip;

    public AudioClip Clip => clip;
}
