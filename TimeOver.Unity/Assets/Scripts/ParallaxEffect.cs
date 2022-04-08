using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length, startPos;
    public float parallaxFactor;
    public GameObject playerCamera;

    private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        var positionX = playerCamera.transform.position.x;
        var temp = positionX * (1 - parallaxFactor);
        var distance = positionX * parallaxFactor;
        var transform1 = transform;
        var position = transform1.position;
        transform1.position = new Vector3(startPos + distance, position.y, position.z);
        
        if (temp > startPos + (length / 2))
        {
            startPos += length;
        }
        else if (temp < startPos - (length / 2))
        {
            startPos -= length;
        }
    }
}