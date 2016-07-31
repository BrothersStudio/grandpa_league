using UnityEngine;
using System.Collections;

public class BirdFlying : MonoBehaviour {

    public Vector2 velocity = Vector2.zero;
    public Vector3 startPos;
    public int waitTime;
    private Rigidbody2D body2d;

	void Awake ()
    {
        body2d = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        
	}

    void Start ()
    {
        StartCoroutine(RestartPosition());
    }
	
	void FixedUpdate ()
    {
        body2d.velocity = velocity;
	}

    IEnumerator RestartPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            transform.position = startPos;
        }
        
    }
}
