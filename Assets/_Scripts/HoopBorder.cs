using UnityEngine;

public class HoopBorder : MonoBehaviour
{
    BaseHoopLogic hoopLogic;
    void Start()
    {
        hoopLogic = GetComponentInParent<BaseHoopLogic>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.perfectShot = false;
        }

    }
}
