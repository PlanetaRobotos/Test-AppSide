using System;
using System.Collections;
using Logic;
using Mechanics.Character;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Collision = Mechanics.Character.Collision;

namespace UI
{
    /// <summary>
    /// PLayer is losing when time is out
    /// </summary>
    public class TimerGame : MonoBehaviour
    {
        [SerializeField] private Image clockImage;
        [SerializeField] private Color onColor;
        [SerializeField] private Color offColor;

        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject timerBg;

        [SerializeField] private AudioClip loseClip;

        private Animator _animator;
        private float _timer;
        private Animator _playerAnimator;
        private bool _isLevelComplete;

        private static readonly int Lose = Animator.StringToHash("Lose");

        private void OnEnable()
        {
            Collision.OnVictoryTimer += LevelComplete;
        }

        private void OnDisable()
        {
            Collision.OnVictoryTimer -= LevelComplete;
        }

        private void Start()
        {
            _animator = timerText.GetComponent<Animator>();
            _playerAnimator = GameObject.FindWithTag(Tags.Player).GetComponent<Animator>();

            bool isUsing = PlayerPrefs.GetInt(Prefs.UseTimer) == 1;
            clockImage.color = isUsing ? onColor : offColor;
        }

        /// <summary>
        /// Ui changing and prefs
        /// </summary>
        public void ChangeTimer()
        {
            bool isUsing = PlayerPrefs.GetInt(Prefs.UseTimer) == 1;

            if (isUsing)
            {
                clockImage.color = offColor;
                PlayerPrefs.SetInt(Prefs.UseTimer, 0);
            }
            else
            {
                clockImage.color = onColor;
                PlayerPrefs.SetInt(Prefs.UseTimer, 1);
            }
        }

        /// <summary>
        /// Use timer
        /// </summary>
        public void ActivateTimer()
        {
            bool isUsing = PlayerPrefs.GetInt(Prefs.UseTimer) == 1;

            if (isUsing)
            {
                StartCoroutine(TimerIe());
                timerBg.SetActive(true);
            }
            else
            {
                timerBg.SetActive(false);
            }
        }

        private void LevelComplete()
        {
            _isLevelComplete = true;
        }

        private IEnumerator TimerIe()
        {
            _timer = PlayerLogic.CurrentMaxTime;
            while (_timer >= 0)
            {
                _timer -= Time.deltaTime;
                timerText.text = $"{Math.Round(_timer, 2)}";
                if (_timer < 10f && !_animator.enabled)
                    _animator.enabled = true;

                if (_isLevelComplete) break;
                yield return null;
            }

            if (_isLevelComplete) yield break;

            timerText.gameObject.SetActive(false);
            PlayerLogic.CanMove = false;
            _playerAnimator.SetTrigger(Lose);
            Audio.Instance.PlaySfx(loseClip);

            yield return new WaitForSeconds(6f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}