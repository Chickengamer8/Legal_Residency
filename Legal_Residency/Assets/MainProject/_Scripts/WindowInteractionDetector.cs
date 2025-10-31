using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInteractionDetector : MonoBehaviour
{
    public GameObject light;
    public GameObject curtains;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.ShowInstructionPopup(InteractionType.DrawingCurtains);
            InteractionManager.instance.SetupInteraction(InteractionType.DrawingCurtains, light, curtains);
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
