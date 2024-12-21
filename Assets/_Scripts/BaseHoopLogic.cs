using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHoopLogic : MonoBehaviour
{
    HoopSpawner hoopSpawner;
    Animator animator;
    [SerializeField] AudioSource audioSource;

    /*------------------------------*/

    [HideInInspector] public bool isPerfect = true;

    private void Start()
    {
        hoopSpawner = GameObject.Find("HoopSpawner").GetComponent<HoopSpawner>();
        animator = transform.parent.gameObject.GetComponent<Animator>();
        //audioSource = transform.parent.gameObject.GetComponent<AudioSource>();
        animator.Play("hoop_shrink_in_anim");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Spawn new hoop
            hoopSpawner.SpawnHoop();

            //Give point to player
            GameManager.Instance.AddPoint(isPerfect, transform.position);

            //Set player's position to this hoop and make rigidbody static

            collision.gameObject.transform.position = transform.position;
            collision.gameObject.GetComponent<PlayerHoop>().MakeStatic();

            //Disable all colliders in gameobject AND child gameobjects(such as gameover border).
            gameObject.GetComponent<Collider2D>().enabled = false;
            Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
            foreach (var collider in childColliders)
            {
                collider.enabled = false;
            }
            //float pitch = 0.8f;
            //pitch += 0.1f * GameManager.Instance.multiplyer;
            //pitch = Mathf.Clamp(pitch, 0.8f, 1.2f);
            //audioSource.pitch = pitch;
            audioSource.Play();
            animator.Play("hoop_shrink_out_anim");
            Destroy(gameObject, 1f);
            Destroy(transform.parent.gameObject, 5f);
        }
    }
}
