using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Cinemachine
{
    public class CinemachineEffects : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        [SerializeField] private float bloomIntensity;
        [SerializeField] private float vignetteIntensity;
        [SerializeField] private float shakeIntensity;
        private Bloom _bloom;
        private Vignette _vignette;

        private CinemachineVirtualCamera _cinemachineVirtualCamera;
        private CinemachineBasicMultiChannelPerlin _channelPerlin;

        public static CinemachineEffects Instance;
        [SerializeField] private float speed = 1f;

        private void Awake()
        {
            Instance = this;

            _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
            _channelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Start()
        {
            if (volume.profile.TryGet(out _bloom))
            {
            }

            if (volume.profile.TryGet(out _vignette))
            {
            }
        }

        public void ShakeScreen()
        {
            StartCoroutine(BloomEffectIe());
            StartCoroutine(VignetteEffectIe());
            StartCoroutine(ShakeEffectIe());
        }

        public void BloomEffect() => StartCoroutine(BloomEffectIe());

        private IEnumerator BloomEffectIe()
        {
            _bloom.intensity.value = bloomIntensity;

            while (_bloom.intensity.value > 0.02f)
            {
                _bloom.intensity.value = Mathf.Lerp(_bloom.intensity.value, 0f, speed * Time.deltaTime);
                yield return null;
            }
        }
        
        private IEnumerator VignetteEffectIe()
        {
            _vignette.intensity.value = vignetteIntensity;

            while (_vignette.intensity.value > 0.02f)
            {
                _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0f, speed * Time.deltaTime);
                yield return null;
            }
        }
        
        private IEnumerator ShakeEffectIe()
        {
            _channelPerlin.m_AmplitudeGain = shakeIntensity;

            while (_channelPerlin.m_AmplitudeGain > 0.02f)
            {
                _channelPerlin.m_AmplitudeGain = Mathf.Lerp(_channelPerlin.m_AmplitudeGain, 0f, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}