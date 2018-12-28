using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Data data;

    private Rigidbody2D rb;
    private Axis jump;

    [Serializable]
    private struct Data
    {
        public float Speed;
        public float JumpHeight;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jump = new Axis("Vertical");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        jump.CheckAxisReleased();

        if ( jump.PressAxis() )
        {
            Debug.Log("Jump!");
            Jump();
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            MoveToward(Input.GetAxis("Horizontal"));
        }
        else
        {
            StopHorizontalMovement();
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * data.JumpHeight, ForceMode2D.Impulse);
    }

    private void MoveToward(float xAxis)
    {
        if (xAxis == 0f) return;

        int direction = (xAxis > 0) ? 1 : -1;

        Vector3 velocity = rb.velocity;
        velocity.x = direction * data.Speed;
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
        public bool isDown = false;
        
        public Axis (string name)
        {
            Name = name;
        }
        public bool PressAxis()
        {
            if (isDown)
            {
                return false;
            }
            else
            {
                if (Input.GetAxis(Name) != 0f)
                {
                    isDown = true;
                    return true;
                }
                return false;
            }
        }
        
        public float GetInput()
        {
            return Input.GetAxis(Name);
        }

        public void CheckAxisReleased()
        {
            if (isDown && Input.GetAxis(Name) == 0f )
            {
                isDown = false;
            }
        }
    }
}
