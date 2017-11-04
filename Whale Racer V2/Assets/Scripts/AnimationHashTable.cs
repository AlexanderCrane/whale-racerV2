using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHashTable : MonoBehaviour {
    public int moveFloat;
    public int turnFloat;

    public int movementState;
    public int jumpState;
    public int emergeState;
    public int submergeState;

    public int groundedBool;
    public int jumpBool;
    public int underwaterBool;
    public int diveBool;
    public int subMovementBool;

    ///  <summary>
    /// Awake method. Initializes the class with hash values for each animation.
    ///  </summary>
    void Awake () {
        moveFloat = Animator.StringToHash("Speed");
        turnFloat = Animator.StringToHash("Turn");

        movementState = Animator.StringToHash("Base Layer.2D Movement");
        jumpState = Animator.StringToHash("Base Layer.Jump");
        emergeState = Animator.StringToHash("Base Layer.Emerge");
        submergeState = Animator.StringToHash("Base Layer.Submerge");

        groundedBool = Animator.StringToHash("Grounded");
        jumpBool = Animator.StringToHash("Jump");
        underwaterBool = Animator.StringToHash("Underwater");
        diveBool = Animator.StringToHash("Dive");
        subMovementBool = Animator.StringToHash("SubMovement");
    }

}
