using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//abstract class used for contoller classes
//controller classes provide input for other classes to use
namespace CharacterController2D
{ 

public abstract class Controller : MonoBehaviour
{
    public abstract InputData GetInputData(InputAction inputValue);
    public virtual float GetInputTime(InputAction inputValue) {return 0f;}
    public virtual bool HasNewInput() {return false;}

    public virtual float GetHorizontalAxis() {return 0f;}
    public virtual float GetVerticalAxis() {return 0f;}

    public virtual InputAction[] GetInputString() {return null;}
    public virtual bool InputStringContains(string inputActions) {return false;}
    public virtual InputData GetLastInputData() {return new InputData();}
    public virtual float GetLastInputTime() {return 0f;}

    public virtual void SetInputLock(InputAction inputType, bool locked) {}
    public virtual void SetInputLock(bool locked) {}
    public virtual bool GetInputLock(InputAction inputType) {return false;}
}

}