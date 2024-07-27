using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    // Duration of the shake
    public float duration = 0.5f;
    // Strength of the shake
    public Vector3 strength = new Vector3(1, 1, 0);
    // Number of shakes
    public int vibrato = 10;
    // Randomness of the shake
    public float randomness = 90f;
    // Whether the shake should fade out
    public bool fadeOut = true;

    private void Awake()
    {
        instance = this;
    }
    // Method to trigger the camera shake
    public void ShakeCamera()
    {
        transform.DOShakePosition(duration, strength, vibrato, randomness, false, fadeOut);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Example trigger, e.g., when space is pressed
        {
            ShakeCamera();
        }
    }
}
