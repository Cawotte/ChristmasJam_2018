using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxisInput
{
    public string Name;
    public bool IsPressedDown = false;
    public bool IsReleased = false;
    public bool WasPressedLastFrame = false;
    
    public float InputValue
    {
        get
        {
            return Input.GetAxis(Name);
        }
    }

    public AxisInput(string name)
    {
        Name = name;
    }

    public bool IsPressed()
    {
        return (Input.GetAxis(Name) != 0f);
    }
    
    
}
