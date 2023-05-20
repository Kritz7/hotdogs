using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotDogs.HDSound
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private SoundCatalogue InteractionCatalogue;

        private AudioSource Source = null;

        private void OnEnable()
        {
            Source ??= Camera.main.GetComponent<AudioSource>();
        }
        public static void PlayInteractionSFX()
        {
            Instance.Source.PlayOneShot(Instance.InteractionCatalogue.GetRandomClip);
        }

        public void PlayRandomAudioFromCatalogue(SoundCatalogue catalogue)
        {
            Source.PlayOneShot(catalogue.GetRandomClip);
        }
    }
}