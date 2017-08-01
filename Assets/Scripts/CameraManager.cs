using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private Coroutine cameraShakeCoroutine = null;
    private Vector3 initialPossition;

    private void Awake()
    {
        initialPossition = this.transform.position;
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
