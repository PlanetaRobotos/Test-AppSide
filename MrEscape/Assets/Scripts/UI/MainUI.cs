using System;
using Logic;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Collision = Mechanics.Character.Collision;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private GameObject finishPanel;
        
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Slider scoreSlider;

        [SerializeField] private Image soundsButton;

        [SerializeField] private Sprite[] audioSprites;

        [SerializeField] private AudioSource uiAudioSource;
        
        private AudioSource[] _audioSources;

        private void OnEnable()
        {
            Collision.OnChangeScore += () =>
            {
                ChangeScore();
                ChangeSliderValue();
            };

            Collision.OnVictoryUI += VictoryUI;

            PlayerLogic.OnFirstGame += InitAudio;
        }

        private void OnDisable()
        {
            Collision.OnChangeScore -= ChangeScore;
            Collision.OnChangeScore -= ChangeSliderValue;
            Collision.OnVictoryUI -= VictoryUI;

            PlayerLogic.OnFirstGame -= InitAudio;
        }

        private void Awake()
        {
            _audioSources = FindObjectsOfType<AudioSource>();
        }

        private void Start()
        {
            uiAudioSource = GetComponent<AudioSource>();
            
            InitAudio();

            ChangeLevel();
            ChangeScore();

            scoreSlider.maxValue = PlayerLogic.MaxScore;
            scoreSlider.value = 0;
        }

        private void VictoryUI()
        {
            finishPanel.SetActive(true);
        }

        public void Restart()
        {
            Time.timeScale = 1;
            // uiAudioSource.Play();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit()
        {
            uiAudioSource.Play();
            Application.Quit();
        }

        private void ChangeScore()
        {
            int newScore = Mathf.Clamp(PlayerLogic.CurrentScore, 0, PlayerLogic.MaxScore);
            scoreText.text = $"{newScore}/{PlayerLogic.MaxScore}";
        }

        private void ChangeLevel()
        {
            levelText.text = $"Level {PlayerPrefs.GetInt(Prefs.UILevel)}";
        }

        private void ChangeSliderValue()
        {
            float val = Mathf.Clamp(scoreSlider.value + 1, 0, PlayerLogic.MaxScore);
            scoreSlider.value = (int) val;
        }

        public void ChangePause(bool pause)
        {
            uiAudioSource.Play();
            Time.timeScale = pause ? 0 : 1;
        }

        public void ChangeAudio()
        {
            uiAudioSource.Play();
            bool isMute = PlayerPrefs.GetInt(Prefs.MuteAudio) == 1;
            // Debug.Log("isMute = " + isMute);

            soundsButton.sprite = isMute ? audioSprites[0] : audioSprites[1];

            foreach (AudioSource source in _audioSources)
                source.mute = !isMute;

            PlayerPrefs.SetInt(Prefs.MuteAudio, isMute ? 0 : 1);
        }

        private void InitAudio()
        {
            bool isMute = PlayerPrefs.GetInt(Prefs.MuteAudio) == 1;

            soundsButton.sprite = isMute ? audioSprites[1] : audioSprites[0];

            foreach (AudioSource source in _audioSources)
                source.mute = isMute;
        }
    }
}