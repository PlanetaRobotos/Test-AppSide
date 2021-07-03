using System;
using System.Collections;
using Logic;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    [Obsolete("Do not use it")]public class TimerGame : MonoBehaviour
    {
        private void Start()
        {
            PlayerPrefs.SetInt(Prefs.UseTimer, 0);
        }
        
        public void StartTimer()
        {
            if (PlayerPrefs.GetInt(Prefs.UseTimer) == 1)
            {
                StartCoroutine(TimerIe());
            }
        }

        private static IEnumerator TimerIe()
        {
            yield return new WaitForSeconds(PlayerLogic.CurrentMaxTime);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}