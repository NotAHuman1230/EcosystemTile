using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float speed;
    [SerializeField] float scaleFactor;
    [SerializeField] float scaleMultiplier;
    [SerializeField] Vector2 scaleRange;

    new Camera camera;
    float multiplier = 1f;

    private void Start()
    {
        camera = GetComponent<Camera>();
        multiplier = 1f;
    }
    private void Update()
    {
        movement();
        scale();
    }

    float pixelDistanceToUnit(float pixels)
    {
        float scale = (camera.orthographicSize * 2f) / camera.pixelHeight;
        return pixels * scale;
    }

    void movement()
    {
        float x = Input.GetAxisRaw("Horizontal") * pixelDistanceToUnit(speed) * Time.deltaTime;
        float y = Input.GetAxisRaw("Vertical") * pixelDistanceToUnit(speed) * Time.deltaTime;

        transform.position += new Vector3(x, y).normalized * pixelDistanceToUnit(speed) * Time.deltaTime;
    }
    void scale()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            camera.orthographicSize += scaleFactor * multiplier * Time.deltaTime;
            multiplier += scaleMultiplier * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            camera.orthographicSize -= scaleFactor * multiplier * Time.deltaTime;
            multiplier += scaleMultiplier * Time.deltaTime;
        }
        else
            multiplier = 1f;

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, scaleRange.x, scaleRange.y);
    }
}
