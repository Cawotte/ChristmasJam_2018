using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Data data;


    //[SerializeField] private bool isJumping = false;
    //[SerializeField] private bool isStopping = false;
    [SerializeField] private State state = State.Walking;
    private Rigidbody2D rb;

    //Inputs
    private AxisInput verticalInput;
    private AxisInput horizontalInput;

    private float lastKnownSpeed;

    #region struct
    [Serializable]
    private struct Data
    {
        public float JumpHeight;
        public float BaseSpeed;
        public float VerySlowMultiplier;
        public float SlowMultiplier;
        public float FastMultiplier;
        public float VeryFastMultiplier;
        
        public float VerySlowSpeed
        {
            get { return BaseSpeed * VerySlowMultiplier; }
        }
        public float SlowSpeed
        {
            get { return BaseSpeed * SlowMultiplier; }
        }
        public float FastSpeed
        {
            get { return BaseSpeed * FastMultiplier; }
        }
        public float VeryFastSpeed
        {
            get { return BaseSpeed * VeryFastMultiplier; }
        }
    }
    #endregion


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        verticalInput = inputManager.Get("Vertical");
        horizontalInput = inputManager.Get("Horizontal");

        SetVelocity(data.BaseSpeed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Resume velocity to speed if moving and blocked by obstacle.
        if ( CanMoveHorizontaly() && rb.velocity.x == 0f)
        {
            SetVelocity(lastKnownSpeed);
        }
        switch (state)
        {
            case (State.Stun):
                //Can't do anything
                break;
            case (State.Walking):
                //Can Jump, and regulate speed depending on inputs.
                if (verticalInput.IsPressed())
                {
                    if (verticalInput.InputValue > 0f)
                    {
                        Jump();
                    }
                    else
                    {
                        StopHorizontalMovement();
                        state = State.Stopping;
                    }
                }
                ManipulateHorizontalSpeed();
                break;
            case (State.Jumping):
                //Can only regulate speed
                ManipulateHorizontalSpeed();

                //Check if ground is touched
                if ( rb.velocity.y == 0f)
                {
                    state = State.Walking;
                }
                break;
            case (State.Stopping):
                //Don't move horizontaly, until key is released.
                if (!verticalInput.IsPressed())
                {
                    SetVelocity(data.BaseSpeed);
                    state = State.Walking;
                }
                break;
        }

    }

    #region trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            //Debug.Log("obstacle touched!");
            Stun(2f);
            Destroy(collision.gameObject);
        }
    }
    #endregion


    private void ManipulateHorizontalSpeed()
    {
        //Speed
        if (horizontalInput.IsPressedDown)
        {
            //Fast speed
            if (horizontalInput.InputValue > 0f)
            {
                //Debug.Log("Set Fast Speed!");
                SetVelocity(data.FastSpeed);
            }
            else
            {
                //Slow speed
                //Debug.Log("Set Slow Speed!");
                SetVelocity(data.SlowSpeed);
            }
        }
        else if (horizontalInput.IsReleased)
        {
            SetVelocity(data.BaseSpeed);
        }
    }
    private void Jump()
    {
        rb.AddForce(Vector3.up * data.JumpHeight, ForceMode2D.Impulse);
        state = State.Jumping;
    }

    private void Stun(float duration)
    {
        StartCoroutine(StunFor(duration));
    }

    private bool CanMoveHorizontaly()
    {
        return state == State.Walking || state == State.Jumping;
    }

    private void SetVelocity(float speed, bool towardRight = true)
    {
        int direction = (towardRight) ? 1 : -1;

        Vector3 velocity = rb.velocity;
        velocity.x = direction * speed;
        rb.velocity = velocity;
        lastKnownSpeed = speed;
    }
    
    private void StopHorizontalMovement()
    {
        Vector3 velocity = rb.velocity;
        velocity.x = 0f;
        rb.velocity = velocity;
    }

    private IEnumerator StunFor(float duration)
    {
        float t = 0f;
        state = State.Stun;
        StopHorizontalMovement();

        while (t < duration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        state = State.Walking;
        SetVelocity(data.BaseSpeed);
    }
    public enum State
    {
        Stun, Walking, Jumping, Stopping
    }
    
}
