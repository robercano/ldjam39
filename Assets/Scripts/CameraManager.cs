using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera camera;
    int currentWidth = 0;
    int currentHeight = 0;
    private Coroutine cameraShakeCoroutine = null;
    private Vector3 initialPossition;
    private void Awake()
    {
        initialPossition = this.transform.position;
    }
    void Start()
    {
        camera = GetComponent<Camera>();
        currentWidth = Screen.width;
        currentHeight = Screen.height;
        UpdateCameraSize();
    }

    void Update()
    {
        int width = Screen.width;
        int height = Screen.height;

        if (currentWidth != width ||
            currentHeight != height)
        {
            currentWidth = width;
            currentHeight = height;
            UpdateCameraSize();
        }
    }
    void UpdateCameraSize()
    {
        camera.orthographicSize = 32.0f * currentHeight / currentWidth / 2.0f;
    }

    //Add a camera shake effect
    public void AddCameraShake(float shakeMagnitude, Vector3 shakeDirection)
    {
        if (this.cameraShakeCoroutine != null)
            this.StopCoroutine(this.cameraShakeCoroutine);

        this.cameraShakeCoroutine = this.StartCoroutine(this.CameraShake(shakeMagnitude, shakeDirection));
    }

    //Camera shake effect
    public IEnumerator CameraShake(float shakeMagnitude, Vector3 shakeDirection)
    {
        while (shakeMagnitude > 0.01f)
        {
            Vector3 shakeMovement = shakeDirection * shakeMagnitude;
            this.transform.position = initialPossition + shakeMovement;
            shakeDirection *= -1;
            shakeMagnitude /= 2f;

            yield return new WaitForSeconds(0.05f);
        }

        transform.position = initialPossition;
    }
}
