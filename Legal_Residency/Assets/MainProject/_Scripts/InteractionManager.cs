using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    public PlayerInput playerInput;

    [HideInInspector]
    public InteractionType currentInteractionType;
    [HideInInspector]
    public GameObject currentLight;
    public GameObject currentCurtain;

    [HideInInspector]
    public bool playerInRange;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        currentLight = null;
        playerInRange = false;

        currentInteractionType = InteractionType.None;
    }

    private void Update()
    {
        var switchAction = playerInput.actions.FindAction("Switch");

        // Debug the action state
        //Debug.Log($"Switch action - IsPressed: {switchAction.IsPressed()}, Triggered: {switchAction.triggered}");

        if (switchAction.triggered)
        {
            //Debug.Log("Switch action triggered!");
            if (playerInRange)
            {
                CheckAndTriggerInteraction();
            }
            else
            {
                Debug.Log("Player not in range");
            }
        }
    }

    public void SetupInteraction(InteractionType interactionType, GameObject light = null, GameObject curtains = null)
    {
        playerInRange = true;
        currentLight = light;
        currentInteractionType = interactionType;
    }

    public void DisconnectInteraction()
    {
        playerInRange = false;
        currentLight = null;
        currentCurtain = null;
        currentInteractionType = InteractionType.None;
    }

    private void CheckAndTriggerInteraction()
    {
        switch (currentInteractionType)
        {
            case InteractionType.FlickingSwitch:
                SwitchOffLight();
                break;
            case InteractionType.DrawingCurtains:
                DrawCurtains();
                break;

            default:
                break;
        }
    }

    private void SwitchOffLight()
    {
        if (currentLight != null)
        {
            currentLight.SetActive(false);
            UIManager.instance.HideInstructionPopup();
        }
    }

    private void DrawCurtains()
    {
        var animator = currentCurtain.GetComponent<Animator>();
        animator.SetTrigger("draw");
    }
}
