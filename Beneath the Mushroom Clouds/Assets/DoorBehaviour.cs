using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    public bool isOpen = false;

    private Animator animator;

    void Awake(){
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        isOpen = true;
        animator.SetTrigger("open");
    }

    public void Close()
    {
        isOpen = false;
        animator.SetTrigger("close");
    }
}
