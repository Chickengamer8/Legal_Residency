using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightInteractionDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("IT'S UNSAFE!!!!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("IT'S SAFE AGAIN!");
    }
}
