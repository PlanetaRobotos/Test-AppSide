using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using Logic;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Mechanics.Character
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Collision : MonoBehaviour
    {
        public static event Action OnChangeScore;
        public static event Action OnVictoryUI;

        [SerializeField] private float minRadius;
        [SerializeField] private float maxRadius;

        [Header("Effects")] [SerializeField] private ParticleSystem[] particles;

        private CapsuleCollider _collider;
        private Audio _audio;
        private Transform _player;
        private Animator _playerAnimator;
        private static readonly int Victory = Animator.StringToHash("Victory");
        [SerializeField] private AudioClip victoryClip;

        private void OnEnable()
        {
            MovementInput.OnSetColliderRadius += SetRadius;
        }

        private void OnDisable()
        {
            MovementInput.OnSetColliderRadius -= SetRadius;
        }

        private void SetRadius(bool setBig) =>
            _collider.radius = setBig ? maxRadius : minRadius;

        private void Start()
        {
            _collider = GetComponent<CapsuleCollider>();
            _collider.radius = minRadius;

            _player = GameObject.FindWithTag(Tags.Player).transform;
            _playerAnimator = _player.GetComponent<Animator>();
            _audio = _player.GetComponent<Audio>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tags.Interactable))
            {
                other.tag = Tags.Untagged;

                Transform objectTransform = other.transform;

                Sequence mySequence = DOTween.Sequence();

                mySequence.Append(objectTransform.DOMoveY(objectTransform.position.y + 0.7f, 0.4f)
                    .SetEase(Ease.OutElastic));
                mySequence.Append(objectTransform.DOScale(0.3f, 1f).SetEase(Ease.InElastic));
                mySequence.PrependInterval(0.1f);
                mySequence.OnComplete(() =>
                {
                    _audio.PlaySfx(_audio.boomClip);
                    PlayerLogic.CurrentScore++;
                    OnChangeScore?.Invoke();

                    ParticleSystem newBoom =
                        Instantiate(particles[Random.Range(0, particles.Length)],
                            objectTransform.position, Quaternion.identity);
                    newBoom.transform.localScale = Vector3.one * 0.7f;
                    CinemachineEffects.Instance.BloomEffect();
                    
                    Destroy(other.gameObject, 0.1f);

                    CheckVictory();
                });
                mySequence.Play();
            }
        }

        private void CheckVictory()
        {
            if (PlayerLogic.CurrentScore >= PlayerLogic.MaxScore)
            {
                PlayerPrefs.SetInt(Prefs.CompleteLevel, 1);
                StartCoroutine(VictoryIe());
            }
        }

        private IEnumerator VictoryIe()
        {
            PlayerLogic.CanMove = false;
            _playerAnimator.SetTrigger(Victory);
            _audio.PlaySfx(victoryClip);
            yield return new WaitForSeconds(4f);
            OnVictoryUI?.Invoke();
        }

        // private void FixedUpdate()
        // {
        //     transform.position = _player.position;
        // }
    }
}