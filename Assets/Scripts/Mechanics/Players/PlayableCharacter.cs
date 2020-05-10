using Scriptable_Objects;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Mechanics.Players
{
    public abstract class PlayableCharacter : Character
    {

        protected float speed;
        
        // Start is called before the first frame update
        void Start()
        {
            CurrentSpeed = statistics.movSpeed;
            CurrentHealth = statistics.maxHealth;
            CurrentAttack = statistics.attack;
            Debug.Log(CurrentSpeed);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            speed = Input.GetAxisRaw("Horizontal") * CurrentSpeed;
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump(15);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            MoveDynamic(new Vector2(speed,0));
        }
    }
}
