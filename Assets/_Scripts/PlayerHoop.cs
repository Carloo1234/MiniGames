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
    [SerializeField] AudioSource gainPointSound;
    Rigidbody2D rb;
    SpriteRenderer playersSprite;
    [SerializeField] HoopSpawner hoopSpawner;
    [SerializeField] SkinDatabase skinDatabase;

    Vector3 startMousePos = Vector3.zero;
    Vector3 endMousePos = Vector3.zero;
    public bool canShootBall = false;

    void Start()
    {
        Time.timeScale = 1;
        playersSprite = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        GameData data = SaveSystem.Load() ?? new GameData();
        playersSprite.sprite = skinDatabase.skins[data.currentlySelectedSkinIndex].sprite;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale != 0)
        {
            startMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            startMousePos.z = 0; // Keep the z position at 0 for 2D
        }

        if (Input.GetMouseButtonUp(0) && Time.timeScale != 0)
        {
            endMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            endMousePos.z = 0; // Keep the z position at 0 for 2D

            if (ValidateDrag(startMousePos, endMousePos))
            {
                canShootBall = true;
            }
        }

        if (Input.GetMouseButton(0) && Time.timeScale != 0)
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
        ballShootSound.volume = ((shootDirection * shootPower).magnitude / clampMagnitude)-0.2f;
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
        GameData data = SaveSystem.Load() ?? new GameData();
        int currentScore = data.totalScore;
        yield return new WaitForSecondsRealtime(4.5f);

        data = SaveSystem.Load() ?? new GameData();
        if (currentScore == data.totalScore && Time.timeScale != 0)
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
            HandleAddPoint(collision.gameObject);
        }
        if(collision.gameObject.tag == "Coin")
        {
            GameManager.Instance.AddCoin(collision.gameObject);
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

    private void HandleAddPoint(GameObject hoop)
    {
        hoopSpawner.SpawnHoop();
        GameManager.Instance.AddPoint(transform.position);
        transform.position = hoop.transform.position;
        MakeStatic();
        hoop.GetComponent<Collider2D>().enabled = false;
        Collider2D[] hoopChildColliders = hoop.GetComponentsInChildren<Collider2D>();
        foreach (var collider in hoopChildColliders)
        {
            collider.enabled = false;
        }

        float pitch = 1.0f;
        pitch += 0.025f * GameManager.Instance.multiplyer;
        pitch = Mathf.Clamp(pitch, 0.8f, 1.3f);
        gainPointSound.pitch = pitch;

        ballHitNetSound.Play();
        gainPointSound.Play();
        hoop.transform.parent.GetComponent<Animator>().Play("hoop_shrink_out_anim");
        Destroy(hoop, 1f);
        Destroy(hoop.transform.parent.gameObject, 5f);

        //stop horizontally moving hoop
        HoopHorizontalMovement movingHoopScript = hoop.GetComponent<HoopHorizontalMovement>();
        if (movingHoopScript != null)
        {
            movingHoopScript.StopMovement();
        }
    }
}