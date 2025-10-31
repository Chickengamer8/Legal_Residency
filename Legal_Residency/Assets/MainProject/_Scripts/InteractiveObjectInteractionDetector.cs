using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectInteractionDetector : MonoBehaviour
{
    public GameObject light;
    public AudioClip audioClip;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.ShowInstructionPopup(InteractionType.InteractiveObject);
            InteractionManager.instance.SetupInteraction(InteractionType.InteractiveObject, light, null, null, audioClip);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.HideInstructionPopup();
            InteractionManager.instance.DisconnectInteraction();
        }
    }
}
