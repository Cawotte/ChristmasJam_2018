using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private string[] axisNames;

    [HideInInspector] public AxisInput[] Axis;


    private void Awake()
    {
        Axis = new AxisInput[axisNames.Length];
        for (int i = 0; i < Axis.Length; i++)
        {
            Axis[i] = new AxisInput(axisNames[i]);
            Axis[i].IsPressedDown = false;
            Axis[i].IsReleased = false;
        }
    }
    
    
    private void LateUpdate()
    {
        for (int i = 0; i < Axis.Length; i++)
        {

            //If currently released and being pressed
            if (!Axis[i].WasPressedLastFrame && Axis[i].IsPressed() )
            {
                Axis[i].IsReleased = false;
                Axis[i].IsPressedDown = true;
            }
            //If currently pressed and being released.
            else if (Axis[i].WasPressedLastFrame && !Axis[i].IsPressed() )
            {
                Axis[i].IsReleased = true;
                Axis[i].IsPressedDown = false;
            }
            else
            {
                Axis[i].IsReleased = false;
                Axis[i].IsPressedDown = false;
            }

            Axis[i].WasPressedLastFrame = Axis[i].IsPressed();
        }
    }

    public AxisInput Get(string axisName)
    {
        for (int i = 0; i < Axis.Length; i++)
        {
            if ( Axis[i].Name.Equals(axisName))
            {
                return Axis[i];
            }
        }
        return null;
    }
}
