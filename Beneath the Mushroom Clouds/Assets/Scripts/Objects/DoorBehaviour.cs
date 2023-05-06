using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements behaviour of doors.
/// </summary>
public class DoorBehaviour : MonoBehaviour
{

    /// <summary>
    /// Whether the door is open or not.
    /// </summary>
    public bool isOpen = false;

    /// <summary>
    /// Reference to the animator of the door.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Gets the reference to the animator on awake.
    /// </summary>
    void Awake(){
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Opens the door.
    /// </summary>
    public void Open()
    {
        isOpen = true;
        animator.SetTrigger("open");
    }

    /// <summary>
    /// Closes the door.
    /// </summary>
    public void Close()
    {
        isOpen = false;
        animator.SetTrigger("close");
    }
}
