using UnityEngine;

public class HoopLogic : MonoBehaviour
{
    HoopSpawner hoopSpawner;
    Animator animator;

  /*------------------------------*/

    public bool isPerfect = true;

    private void Start()
    {
        hoopSpawner = GameObject.Find("HoopSpawner").GetComponent<HoopSpawner>();
        animator = gameObject.GetComponent<Animator>();
        animator.Play("hoop_shrink_in_anim");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Spawn new hoop
            //////if (hoopSpawner) hoopSpawner.SpawnHoop();

            //Give point to player

            //////GameManager.Instance.AddPointtransform.position);

            //Set player's position to this hoop and make rigidbody static

            //////collision.gameObject.transform.position = transform.position;
            //////collision.gameObject.GetComponent<PlayerHoop>().MakeStatic();

            //Disable all colliders in gameobject AND child gameobjects(such as gameover border).
            //////gameObject.GetComponent<Collider2D>().enabled = false;
            //Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
            //////foreach (var collider in childColliders)
            //////{
            //////    collider.enabled = false;
            //////}

            //play hoop shrinkout animation which calls destroy function with animation event.
            //////if(animator) animator.Play("hoop_shrink_out_anim");
        }
    }

    private void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
