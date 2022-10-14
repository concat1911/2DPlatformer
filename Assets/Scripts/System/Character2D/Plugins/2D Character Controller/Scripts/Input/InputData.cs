using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CharacterController2D
{ 

//list of inputs
//Warning: If adding new actions to enum check editor values as they may be changed such as in Scriptable Input Map
//  EX: None = 0, Up = 1, Down = 2
//      if you add in an action "Attack" between None and Up then Attack now == 1 and Up = 2
public enum InputAction
{
    None
    ,Up
    ,Down
    ,Left
    ,Right
    ,Run
    ,Crouch
    ,Jump
    ,Dash
    ,WallGrab
}



//data stored about each input
public class InputData
{
    public InputAction type;
    public bool isDown;
    public bool isHeld;
    public bool isNew;//if given this frame

    public bool inputLocked;

    public InputData()
    {
        type = InputAction.None;
        isDown = false;
        isHeld = false;
        inputLocked = false;
        isNew = false;
    }
    public InputData(InputAction typ)
    {
        type = typ;
        isDown = false;
        isHeld = false;
        inputLocked = false;
        isNew = false;
    }
    public InputData(InputAction type, bool isDown, bool isHeld, bool isNew, bool inputLocked)
    {
        this.type = type;
        this.isDown = isDown;
        this.isHeld = isHeld;
        this.isNew = isNew;
        this.inputLocked = inputLocked;
    }
}

}