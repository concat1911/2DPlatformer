using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//interface for objects that will be moved with script
//These are what other objects will call when wanting to interact with character to do things such as apply force or limit movement
namespace CharacterController2D
{ 

public interface IMove
{
    void SetVelocity(Vector2 vector);
    Vector2 GetVelocity();
    void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force);
    void AddForwardForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force);
}


public interface ICharacterMovement: IMove
{
    PlayerMovementInfo GetMovementInfo();
    void SetMovementConstraints(MovementConstraints constraints);
    MovementConstraints GetMovementConstraints();
    void CanControlMovement(bool canControl);
}

}