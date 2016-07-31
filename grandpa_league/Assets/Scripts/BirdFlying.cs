using UnityEngine;
using System.Collections;

public class BirdFlying : MonoBehaviour {

    public Vector2 velocity = Vector2.zero;
    public Vector3 StartPos;

    private Rigidbody2D body2d;

	void Awake ()
    {
        body2d = GetComponent<Rigidbody2D>();
        StartPos = transform.position;
	}
	
	void FixedUpdate ()
    {
        body2d.velocity = velocity;
	}
}
