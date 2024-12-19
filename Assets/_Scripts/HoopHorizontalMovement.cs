using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopHorizontalMovement : BaseHoopLogic
{
    [Header("Custom Attributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform maxHeight, minHeight;
    private int moveDir = 1;

    private void Update()
    {
        transform.Translate(new Vector3(0, moveDir * moveSpeed * Time.deltaTime, 0), Space.World);
        if (transform.position.y >= maxHeight.position.y)
        {
            moveDir = -1;
        }
        else if(transform.position.y <= minHeight.position.y)
        {
            moveDir = 1;
        }
    }
}
