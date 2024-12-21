using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHoopLogic : MonoBehaviour
{
    HoopSpawner hoopSpawner;
    Animator animator;
    [SerializeField] AudioSource audioSource;

    /*------------------------------*/

    private void Start()
    {
        hoopSpawner = GameObject.Find("HoopSpawner").GetComponent<HoopSpawner>();
        animator = transform.parent.gameObject.GetComponent<Animator>();
        //audioSource = transform.parent.gameObject.GetComponent<AudioSource>();
        animator.Play("hoop_shrink_in_anim");
    }
   
}
