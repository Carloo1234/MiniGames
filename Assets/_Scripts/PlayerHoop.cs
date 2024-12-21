using System.Collections;
using UnityEngine;

public class PlayerHoop : MonoBehaviour
{
    [SerializeField] AudioSource dieSound;
    [SerializeField] float minDragDistance = 2f;
    [SerializeField] float shootPower;
    [SerializeField] float rotationPower;
    [SerializeField] DrawTrajectory trajectoryRenderer;
    [SerializeField] float clampMagnitude;
    [SerializeField] AudioSource hoopHitSound;
    [SerializeField] AudioSource ballShootSound;
    [SerializeField] AudioSource ballHitNetSound;
    Rigidbody2D rb;
    SpriteRenderer playersSprite;
    [SerializeField] HoopSpawner hoopSpawner;

    Vector3 startMousePos = Vector3.zero;
    Vector3 endMousePos = Vector3.zero;
    bool canShootBall = false;

    void Start()
    {
        Time.timeScale = 1;
        playersSprite = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        playersSprite.sprite = GameManager.Instance.balls[PlayerPrefs.GetInt("SelectedBallIndex", 0)];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            startMousePos.z = 0; // Keep the z position at 0 for 2D
        }

        if (Input.GetMouseButtonUp(0))
        {
            endMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            endMousePos.z = 0; // Keep the z position at 0 for 2D

            if (ValidateDrag(startMousePos, endMousePos))
            {
                canShootBall = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            currentMousePos.z = 0; // Keep the z position at 0 for 2D

            if (ValidateDrag(startMousePos, currentMousePos))
            {
                Vector3 velocity = Vector3.ClampMagnitude(((startMousePos - currentMousePos) * shootPower), clampMagnitude);
                trajectoryRenderer.RenderDots(transform.position, velocity);
            }
            else
            {
                trajectoryRenderer.hideDots();
            }
        }
    }

    private bool ValidateDrag(Vector2 startMousePos, Vector2 endMousePos)
    {
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            return Vector2.Distance(startMousePos, endMousePos) > minDragDistance;
        }
        return false;
    }

    private void ShootBall(Vector2 startMousePos, Vector2 endMousePos)
    {
        trajectoryRenderer.hideDots();
        MakeDynamic();
        Vector2 shootDirection = startMousePos - endMousePos;
        rb.AddForce(Vector3.ClampMagnitude((shootDirection * shootPower), clampMagnitude), ForceMode2D.Impulse);
        ballShootSound.volume = (shootDirection * shootPower).magnitude / clampMagnitude;
        Debug.Log((shootDirection * shootPower).magnitude / clampMagnitude);
        ballShootSound.Stop();
        ballShootSound.Play();
        int randNum = UnityEngine.Random.Range(1, 11);
        if (randNum >= 5)
        {
            randNum = -1;
        }
        else
        {
            randNum = 1;
        }
        rb.AddTorque(rotationPower * randNum);
        this.startMousePos = Vector3.zero;
        this.endMousePos = Vector3.zero;
    }



    public void MakeStatic()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void MakeDynamic()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }


    void FixedUpdate()
    {
        if (canShootBall)
        {
            ShootBall(startMousePos, endMousePos);
            StartCoroutine(DeathTimer());
            canShootBall = false;
        }
    }

    IEnumerator DeathTimer()
    {
        int currentScore = PlayerPrefs.GetInt("TotalScore", 0);
        yield return new WaitForSecondsRealtime(4.5f);

        if (currentScore == PlayerPrefs.GetInt("TotalScore", 0) && Time.timeScale != 0)
        {
            GameManager.Instance.GameOver();
            dieSound.Play();
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            dieSound.Play();
        }
        if(collision.gameObject.tag == "Point")
        {
            hoopSpawner.SpawnHoop();
            GameManager.Instance.AddPoint(transform.position);
            transform.position = collision.gameObject.transform.position;
            MakeStatic();
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            Collider2D[] hoopChildColliders = collision.gameObject.GetComponentsInChildren<Collider2D>();
            foreach (var collider in hoopChildColliders)
            {
                collider.enabled = false;
            }
            collision.gameObject.transform.parent.GetComponent<AudioSource>().Play();
            collision.gameObject.transform.parent.GetComponent<Animator>().Play("hoop_shrink_out_anim");
            Destroy(collision.gameObject, 1f);
            Destroy(collision.gameObject.transform.parent.gameObject, 5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "HoopRing")
        {
            hoopHitSound.Stop();
            hoopHitSound.Play();
        }
    }
}