using UnityEngine;
using System.Collections;

public class CloudMovement : MonoBehaviour
{
    public Vector2 velocity = Vector2.zero;
    public Vector3 startPos;

    private Rigidbody2D body2d;

    void Awake()
    {
        startPos = transform.position;
        body2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        body2d.velocity = velocity;
        print(startPos);
        print(transform.position);
        if(transform.position.x >= (startPos.x + 120))
        {
            transform.position = startPos;
        }
    }

}
