using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisons : MonoBehaviour
{
    private AudioSource crashSound;

    void Awake ()
    {
        crashSound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            crashSound.Play();
        }
    }
}
