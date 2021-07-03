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
    ///     Double click; Third click
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
            if (JumpState == PlayerJumpStates.Check) 
                JumpState = PlayerJumpStates.Boom;

            tap++;
            if (tap == 1)
                StartCoroutine(DoubleTapInterval());

            else if (tap > 1)
            {
                if (JumpState == PlayerJumpStates.None) 
                    JumpState = PlayerJumpStates.StartJump;
                tap = 0;
            }
        }
        
        private IEnumerator DoubleTapInterval()
        {
            yield return new WaitForSeconds(interval);
            tap = 0;
        }

        private void Start() => _timer = 0;

        private void FixedUpdate()
        {
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