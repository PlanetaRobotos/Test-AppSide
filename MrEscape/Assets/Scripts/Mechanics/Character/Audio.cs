using System;
using UnityEngine;

namespace Mechanics.Character
{
    /// <summary>
    /// Audio Class (Player? explosions, etg)
    /// </summary>
    public class Audio : MonoBehaviour
    {
        [Header("Explosion")] public AudioClip jumpClip;
        public AudioClip boomClip;

        public AudioClip stepClip;

        private AudioSource _audio;
        [SerializeField] private AudioSource stepsAudio;
        public static Audio Instance;

        private void Awake() => Instance = this;

        private void Start() => _audio = GetComponent<AudioSource>();

        public void PlaySfx(AudioClip clip) => _audio.PlayOneShot(clip);

        /// <summary>
        /// Changing audio settings
        /// </summary>
        /// <param name="needPLay">if need to play now clip</param>
        public void MoveAudio(bool needPLay)
        {
            if (!stepsAudio.isPlaying && needPLay)
            {
                stepsAudio.PlayOneShot(stepClip);
            }
            else if (stepsAudio.isPlaying && !needPLay)
                stepsAudio.Stop();
        }
    }
}