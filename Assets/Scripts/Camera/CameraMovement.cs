using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float scaleFactor;
    [SerializeField] Vector2 scaleRange;

    new Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }
    private void Update()
    {
        movement();
        scale();
    }

    void movement()
    {
        float x = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        float y = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;

        transform.position += new Vector3(x, y, 0f);
    }
    void scale()
    {
        if (Input.GetKey(KeyCode.E))
            camera.orthographicSize += scaleFactor * Time.deltaTime;
        else if (Input.GetKey(KeyCode.Q))
            camera.orthographicSize -= scaleFactor * Time.deltaTime;

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, scaleRange.x, scaleRange.y);
    }
}
