using UnityEngine;

namespace Mechanics.Character
{
    public class Audio : MonoBehaviour
    {
        [Header("Explosion")] public AudioClip jumpClip;
        public AudioClip boomClip;

        [Header("Movement")] public AudioClip[] stepClips;
        public AudioClip stepClip;

        private AudioSource _audio;
        [SerializeField] private AudioSource stepsAudio;


        public static Audio Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _audio = GetComponent<AudioSource>();
        }

        public void PlaySfx(AudioClip clip)
        {
            _audio.PlayOneShot(clip);
        }

        public void MoveAudio(bool needPLay)
        {
            if (!stepsAudio.isPlaying && needPLay)
            {
                // if (stepsAudio.clip == stepClips[0])
                // {
                //     stepsAudio.PlayOneShot(stepClips[1]);
                //     stepsAudio.clip = stepClips[1];
                // }
                // else
                // {
                //     stepsAudio.PlayOneShot(stepClips[0]);
                //     stepsAudio.clip = stepClips[0];
                // }

                stepsAudio.PlayOneShot(stepClip);
                // stepClip.
            }
            else if (stepsAudio.isPlaying && !needPLay)
                stepsAudio.Stop();
        }
    }
}