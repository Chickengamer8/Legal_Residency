using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketInteractionDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.ShowInstructionPopup(InteractionType.HidingInBasket);
            InteractionManager.instance.SetupInteraction(InteractionType.HidingInBasket, null, null, this.gameObject);
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
