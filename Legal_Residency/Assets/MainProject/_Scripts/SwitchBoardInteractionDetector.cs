using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBoardInteractionDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.ShowLightBoardInstruction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.HideLightBoardInstruction();
        }
    }
}
