using System.Collections;
using UnityEngine;

namespace Mechanics.Traps
{
    public class StoneFace : Movable
    {
        [SerializeField] private bool isDown;
        private bool _isDown;
        [SerializeField] private float _distance;
        private float initSpeed = 0f;
        private float _acceleration = 100f;
        [SerializeField] private Animator _animator;
        private static readonly int IsDown = Animator.StringToHash("isDown");
        private bool _isWaiting = false;
        private bool _callDuringTransition = false;
        private int _numOfPlayersOnPressurePlate = 0;
        private static readonly int IsDying = Animator.StringToHash("isDying");

        // Start is called before the first frame update
        void Start()
        {
            _isDown = isDown;
            _animator.SetBool(IsDown,true);
        }

        public void SwitchState(bool enabling)
        {
            if (enabling) _numOfPlayersOnPressurePlate++;
            else if(_numOfPlayersOnPressurePlate>0) _numOfPlayersOnPressurePlate--;
            
            if(_numOfPlayersOnPressurePlate>1) return;
            
            if (MoveFixedDistanceAccelerated /*|| _callDuringTransition*/)
            {
                /*if (!_callDuringTransition) _callDuringTransition = true;*/
                return;
            }

            if (_isDown)
            {
                SetFixedDistanceAccelerated(Vector2.up, initSpeed, _distance, _acceleration);
                _isDown = false;
                _animator.SetBool(IsDown,false);
                if (!_isWaiting)
                {
                    _isWaiting = true;
                    StartCoroutine(WaitEndTransition());
                }
            } else
            {
                if(_numOfPlayersOnPressurePlate>0) return;
                SetFixedDistanceAccelerated(Vector2.down, initSpeed, _distance, _acceleration);
                _isDown = true;
                _animator.SetBool(IsDown,true);
                if (!_isWaiting)
                {
                    _isWaiting = true;
                    StartCoroutine(WaitEndTransition());
                }
            }
        }

        private void ReturnToDefaultState()
        {
            if(MoveFixedDistanceAccelerated) return;
            if (!isDown)
            {
                SetFixedDistanceAccelerated(Vector2.up, initSpeed, _distance, _acceleration);
                _isDown = false;
                _animator.SetBool(IsDown,false);
                /*StartCoroutine(WaitReturnToDefault());*/
            } else
            {
                SetFixedDistanceAccelerated(Vector2.down, initSpeed, _distance, _acceleration);
                _isDown = true;
                _animator.SetBool(IsDown,true);
                /*StartCoroutine(WaitReturnToDefault());*/
            }
        }

        private IEnumerator WaitReturnToDefault()
        {
            while (MoveFixedDistanceAccelerated)
            {
                yield return null;
            }
            
            _callDuringTransition = false;
        }

        private IEnumerator WaitEndTransition()
        {
            while (MoveFixedDistanceAccelerated)
            {
                yield return null;
            }
        
            if(_isDown != isDown && _numOfPlayersOnPressurePlate == 0) ReturnToDefaultState();

            _isWaiting = false;
        }

        public void Die()
        {
            _animator.SetBool(IsDying,true);
            gameObject.layer = LayerMask.NameToLayer("Particle");
            Rb.constraints = RigidbodyConstraints2D.None;
            GetComponent<Rigidbody2D>().gravityScale = 10;
            AddForce(new Vector2(Random.Range(-0.5f,0.5f),1f), 50f);
            Rb.MoveRotation(Random.Range(0,180));
            StartCoroutine(WaitBeforeDestroy());
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(1.2f);
            Destroy(gameObject);
        }
    }
}
