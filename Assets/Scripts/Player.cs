using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Data data;


    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isStopping = false;
    private Rigidbody2D rb;

    //Inputs
    private AxisInput verticalInput;
    private AxisInput horizontalInput;

    private float lastKnownSpeed;
    
    [Serializable]
    private struct Data
    {
        public float BaseSpeed;
        public float SlowSpeed;
        public float FastSpeed;
        public float JumpHeight;
    }
    
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

        //On vertical button release
        if (!verticalInput.IsPressed())
        {
            //If he was stopping, resume course.
            if (isStopping)
            {
                isStopping = false;

                Debug.Log("Set Base Speed! (Stopping)");
                SetVelocity(data.BaseSpeed);
            }
        }
        //On horizontal button release
        if (!horizontalInput.IsPressed())
        {
            //If he's not stopping.
            if (!isStopping)
            {
                Debug.Log("Set Base Speed!");
                SetVelocity(data.BaseSpeed);
            }
        }

        //Jump
        if ( verticalInput.IsPressedDown)
        {
            if (verticalInput.InputValue > 0f)
            {
                Jump();
            }
            else
            {
                StopHorizontalMovement();
                isStopping = true;
            }
        }

        //Speed
        if (horizontalInput.IsPressed())
        {
            //Fast speed
            if (horizontalInput.InputValue > 0f)
            {
                Debug.Log("Set Fast Speed!");
                SetVelocity(data.FastSpeed);
            }
            else
            {
                //Slow speed
                Debug.Log("Set Slow Speed!");
                SetVelocity(data.SlowSpeed);
            }
        }


        //When not moving, try moving.
        if (rb.velocity.x == 0f && !isStopping)
        {
            SetVelocity(lastKnownSpeed);
        }

    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * data.JumpHeight, ForceMode2D.Impulse);
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
    
}
