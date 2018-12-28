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
        if (verticalInput.IsReleased)
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
        if (horizontalInput.IsReleased)
        {
            //If he's not stopping.
            if (!isStopping)
            {
                Debug.Log("Set Base Speed!");
                SetVelocity(data.BaseSpeed);
            }
        }

        //Jump
        if ( verticalInput.IsPressedDown )
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
        if (horizontalInput.IsPressedDown)
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
    }

    private void MoveToward(float xAxis)
    {
        if (xAxis == 0f) return;

        int direction = (xAxis > 0) ? 1 : -1;

        Vector3 velocity = rb.velocity;
        velocity.x = direction * data.BaseSpeed;
        rb.velocity = velocity;
    }

    private void StopHorizontalMovement()
    {
        Vector3 velocity = rb.velocity;
        velocity.x = 0f;
        rb.velocity = velocity;
    }


    private class Axis
    {
        public string Name;
        public bool IsPressed = false;
        
        public Axis (string name)
        {
            Name = name;
        }

        public bool AxisPressedDown()
        {
            if (IsPressed)
            {
                return false;
            }
            else
            {
                if (Input.GetAxis(Name) != 0f)
                {
                    IsPressed = true;
                    return true;
                }
                return false;
            }
        }
        
        public float GetInput()
        {
            return Input.GetAxis(Name);
        }

        public bool AxisPressedUp()
        {
            if (IsPressed && Input.GetAxis(Name) == 0f )
            {
                IsPressed = false;
                return true;
            }
            return false;
        }
    }
}
