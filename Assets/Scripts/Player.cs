using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Data data;
    [SerializeField] private float minimalTimeInFog = 1f;


    //[SerializeField] private bool isJumping = false;
    //[SerializeField] private bool isStopping = false;
    [SerializeField] private Form form = Form.Vampire;
    [SerializeField] private State state = State.Walking;

    private Rigidbody2D rb;
    private Collider2D collider;

    //Inputs
    private AxisInput verticalInput;
    private AxisInput horizontalInput;
    private AxisInput fogInput;
    private AxisInput batInput;

    //timer
    private float timeSpentInFog = 0f;

    //remember values
    private float lastKnownSpeed;
    private float gravityScale;

    public Form CurrentForm { get => form; }

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
        collider = GetComponent<Collider2D>();

        SetVelocity(data.BaseSpeed);
        gravityScale = rb.gravityScale;
    }

    private void Start()
    {
        verticalInput = inputManager.Get("Vertical");
        horizontalInput = inputManager.Get("Horizontal");
        batInput = inputManager.Get("Action1");
        fogInput = inputManager.Get("Action2");
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
                ManipulateHorizontalSpeed();

                //Check jump or stop
                if (verticalInput.IsPressed())
                {
                    if (verticalInput.InputValue > 0f)
                    {
                        Jump();
                        //form = Form.Wolf;
                    }
                    else
                    {
                        StopHorizontalMovement();
                        state = State.Stopping;
                    }
                }
                else if (fogInput.IsPressedDown)
                {
                    StartFogging();
                }
                break;
            case (State.Jumping):
                //Can only regulate speed
                ManipulateHorizontalSpeed();

                //Check fogging
                if (fogInput.IsPressedDown)
                {
                    StartFogging();
                }
                //Check if ground is touched
                else if ( rb.velocity.y == 0f)
                {
                    StartWalking();
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
            case (State.Phasing):
                //turn back to vampire from fog
                if (timeSpentInFog > minimalTimeInFog && !fogInput.IsPressed())
                {
                    //collider.enabled = true;
                    //rb.gravityScale = gravityScale;
                    StartWalking();

                }
                else
                {
                    timeSpentInFog += Time.deltaTime;
                }
                break;
        }

        switch (state)
        {
            case (State.Stun):
                if (form != Form.Vampire)
                {
                    form = Form.Vampire;
                }
                break;
            case (State.Walking):
                if (form != Form.Vampire)
                {
                    form = Form.Vampire;
                }
                break;
            case (State.Stopping):
                if (form != Form.Vampire)
                {
                    form = Form.Vampire;
                }
                break;
            case (State.Jumping):
                if (form != Form.Wolf)
                {
                    form = Form.Wolf;
                }
                break;
            case (State.Phasing):
                if (form != Form.Fog)
                {
                    form = Form.Fog;
                }
                break;
        }

    }

    #region trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (form != Form.Fog && collision.gameObject.tag.Equals("Obstacle"))
        {
            //Debug.Log("obstacle touched!");
            Stun(2f);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (form != Form.Fog && collision.gameObject.tag.Equals("Obstacle"))
        {
            Stun(2f);
            Destroy(collision.gameObject);
        }
    }
    #endregion

    #region start States
    private void StartWalking()
    {
        form = Form.Vampire;
        state = State.Walking;
        
        SetVelocity(data.BaseSpeed);
    }
    private void StartFogging()
    {
        //init
        form = Form.Fog;
        state = State.Phasing;
        timeSpentInFog = 0f;
        SetVerticalVelocity(0f);
        //desactive collisions
        //collider.enabled = false;
        //rb.gravityScale = 0f;

        //Go slow
        SetVelocity(data.VerySlowSpeed);
        
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
        Vector3 velocity = rb.velocity;
        velocity.y += Mathf.Sqrt(-2.0f * (Physics2D.gravity.y * rb.gravityScale) * data.JumpHeight);
        rb.velocity = velocity;
        
        //rb.AddForce(Vector3.up * data.JumpHeight, ForceMode2D.Impulse);
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

        if (rb.velocity.x == direction * speed) return;

        Vector3 velocity = rb.velocity;
        velocity.x = direction * speed;
        rb.velocity = velocity;
        lastKnownSpeed = speed;
    }

    private void SetVerticalVelocity(float value)
    {
        Vector3 velocity = rb.velocity;
        velocity.y = value;
        rb.velocity = velocity;
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
        form = Form.Vampire;
        StopHorizontalMovement();

        while (t < duration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        state = State.Walking;
        SetVelocity(data.BaseSpeed);
    }

    public enum Form
    {
        Vampire, Wolf, Bat, Fog
    }
    public enum State
    {
        Stun, Walking, Jumping, Stopping, Phasing
    }
    
}
