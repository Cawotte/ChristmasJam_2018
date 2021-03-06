using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Data data;
    [SerializeField] private float minimalTimeInFog = 1f;


    //[SerializeField] private bool isJumping = false;
    //[SerializeField] private bool isStopping = false;
    [SerializeField] private Form form = Form.Vampire;
    [SerializeField] private State state = State.Walking;

    [SerializeField] private Vector3 veloc;
    private Rigidbody2D rb;
    private Collider2D collider;

    //Inputs
    private AxisInput verticalInput;
    private AxisInput horizontalInput;
    private AxisInput fogInput;
    private AxisInput jumpInput;

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
        public float batVerticalSpeed;
        public float BaseSpeed;
        public float VerySlowMultiplier;
        public float SlowMultiplier;
        public float FastMultiplier;
        public float VeryFastMultiplier;

        public float NormalStunDuration;
        public float LongStunDuration;
        
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
        jumpInput = inputManager.Get("Action1");  //a - space
        fogInput = inputManager.Get("Action2"); //z
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        veloc = rb.velocity;
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
                if (jumpInput.IsPressedDown)
                {
                    Jump();
                }
                else if (fogInput.IsPressedDown)
                {
                    StartFogging();
                }
                else if (verticalInput.IsPressedDown)
                {
                    StartFlying();
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
                else if (verticalInput.IsPressedDown)
                {
                    StartFlying();
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
                    StartWalking();
                }
                else
                {
                    timeSpentInFog += Time.deltaTime;
                }
                break;
            case (State.Flying):
                ManipulateVerticalSpeed();
                if (fogInput.IsPressedDown)
                {
                    OnEndBat();
                    StartFogging();
                }
                break;
        }

        switch (state)
        {
            case (State.Stun):
                if (form != Form.Stun)
                {
                    form = Form.Stun;
                }
                break;
            case (State.Walking):
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
            case (State.Flying):
                if (form != Form.Bat)
                {
                    form = Form.Bat;
                }
                break;
        }

    }

    #region trigger and collsion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (form != Form.Fog && collision.gameObject.tag.Equals("Obstacle"))
        {
            if (form == Form.Bat)
            {
                OnEndBat();
                Stun(data.LongStunDuration);
            }
            else
            {
                Stun(data.NormalStunDuration);
            }
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag.Equals("Victory"))
        {
            gameManager.WinTheGame();
        }
    }

    //useful when popping out of fog inside obstacle
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (form == Form.Bat && !collision.gameObject.tag.Equals("Ceiling"))
        {
            //Debug.Log("No ceileing collision!");
            OnEndBat();
            StartWalking();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
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

    private void StartFlying()
    {
        form = Form.Bat;
        state = State.Flying;

        rb.gravityScale = 0f;
        SetVelocity(data.VeryFastSpeed);
        gameObject.transform.position += Vector3.up * 0.1f;
        //rb.AddForce(Vector3.up, ForceMode2D.Impulse);
    }

    private void OnEndBat()
    {
        rb.gravityScale = gravityScale;
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

    private void ManipulateVerticalSpeed()
    {
        //Up when pressed, down else.
        if (verticalInput.IsPressed() && verticalInput.InputValue > 0f)
        {
            SetVerticalVelocity(data.batVerticalSpeed);
        }
        else
        {
            SetVerticalVelocity(-data.batVerticalSpeed);
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
        if (state == State.Stun) yield break;
        float t = 0f;
        StopHorizontalMovement();
        SetVerticalVelocity(0f);
        state = State.Stun;
        form = Form.Stun;

        while (t < duration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        StartWalking();
    }

    public enum Form
    {
        Vampire, Wolf, Bat, Fog, Stun
    }
    public enum State
    {
        Stun, Walking, Jumping, Stopping, Phasing, Flying
    }
    
}
