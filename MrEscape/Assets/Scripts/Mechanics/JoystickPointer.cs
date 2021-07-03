using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mechanics
{
    public enum PlayerJumpStates
    {
        None,
        StartJump,
        Check,
        Boom,
        EndBoom
    }
    
    /// <summary>
    ///     Double click - Player jumping
    /// </summary>
    public class JoystickPointer : MonoBehaviour, IPointerClickHandler
    {
        float interval = 0.5f;
        int tap;
        public static PlayerJumpStates JumpState { get; set; }

        [SerializeField] private float flyTime = 1.5f;
        private float _timer;

        public void OnPointerClick(PointerEventData eventData)
        {
            // Debug.Log(eventData.clickCount + " - click count");
            // Debug.Log(JumpState + " - JumpState");

            // if (eventData.clickCount == 2 && JumpState == PlayerJumpStates.None)
            // {
            //     JumpState = PlayerJumpStates.StartJump;
            //     Debug.Log("double click");
            // }

            if (JumpState == PlayerJumpStates.Check)
            {
                JumpState = PlayerJumpStates.Boom;
                // Debug.Log($"Boom");
            }
            
            ////////////////
            tap++;
 
            if (tap == 1)
            {
                StartCoroutine(DoubleTapInterval());
            }
 
            else if (tap > 1)
            {
                if (JumpState == PlayerJumpStates.None)
                {
                    JumpState = PlayerJumpStates.StartJump;
                    // Debug.Log("double click");
                }

                // clean tap calculation
                tap = 0;
            }
        }
        
        private IEnumerator DoubleTapInterval()
        {
            yield return new WaitForSeconds(interval);
            tap = 0;
        }

        private void Start()
        {
            _timer = 0;
        }

        private void FixedUpdate()
        {
            // Debug.Log("VAR = " + JumpState);
            if (JumpState == PlayerJumpStates.Check) 
                _timer += Time.deltaTime;

            if (_timer >= flyTime)
            {
                JumpState = PlayerJumpStates.None;
                _timer = 0;
            }
        }
    }
}