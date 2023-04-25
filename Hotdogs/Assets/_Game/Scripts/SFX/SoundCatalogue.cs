using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SFX Catalogue", menuName = "HotDogs/SFX/SFX Catalogue")]
public class SoundCatalogue : ScriptableObject
{
    [SerializeField] private List<SFXScriptable> InteractSFX;

    public AudioClip GetRandomClip => InteractSFX.GetRandom().Clip;
}
