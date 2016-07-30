using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
    {

    public int level = 1;
    public float setTime = 6.0f;
    public float dimStart = 0.0f;
    public float dimTime = 4.0f;
    public Light dimLight;
    public float dimAmt = 0.01f;

    //Camera c;
    float timer;

	void Start ()
    {
       // c = GetComponent<Camera>();
        timer = 0.0f;
	}
	
	void Update ()
    {
        timer += Time.deltaTime;
        //c.fieldOfView -= zoomSpeed;
        if(timer > dimStart && timer < 3.0f && dimLight.intensity < 2.0)
        {
            dimLight.intensity += dimAmt;
        }
        else if (timer > dimTime && timer < setTime)
        {
            dimLight.intensity -= dimAmt;
        }
        else if(timer > setTime || Input.GetMouseButtonDown(0))
            SceneManager.LoadScene(level);

    }
}
