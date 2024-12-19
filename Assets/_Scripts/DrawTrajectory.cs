using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    [SerializeField] float timeStep = 0.1f;
    [SerializeField] GameObject[] dots;
    [SerializeField] GameObject parentDot;

    Vector2 gravity;

    private void Start()
    {
        gravity = Physics2D.gravity;
        hideDots();
    }

    public void RenderDots(Vector3 ballPos, Vector3 velocity)
    {
        unHideDots();
        for (int i = 1; i < dots.Length + 1; i++)
        {
            float timePassed = i * timeStep;
            dots[i-1].transform.position = ballPos + velocity * timePassed + 0.5f * (Vector3)gravity * timePassed * timePassed;
        }
    }
    public void hideDots()
    {
        parentDot.SetActive(false);
    }
    public void unHideDots()
    {
        parentDot.SetActive(true);
    }
}