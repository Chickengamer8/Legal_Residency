using StarterAssets;
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
    public GameObject currentBasket;
    public AudioClip currentAudio;

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

    public void SetupInteraction(InteractionType interactionType, GameObject light = null, GameObject curtains = null,
        GameObject basket = null, AudioClip audioClip = null)
    {
        playerInRange = true;
        currentLight = light;
        currentCurtain = curtains;
        currentBasket = basket;
        currentInteractionType = interactionType;
        currentAudio = audioClip;
    }

    public void DisconnectInteraction()
    {
        playerInRange = false;
        currentLight = null;
        currentCurtain = null;
        currentBasket = null;
        currentAudio = null;
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
            case InteractionType.HidingInBasket:
                HideInBasket();
                break;
            case InteractionType.InteractiveObject:
                InteractWithObject();
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
        SwitchOffLight();
    }

    private void HideInBasket()
    {
        UIManager.instance.HideInstructionPopup();
        StartCoroutine(HideInBasketSequence());
    }
    private IEnumerator HideInBasketSequence()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        ThirdPersonController playerController = null;
        CharacterController characterController = null;

        if (player != null && currentBasket != null)
        {
            // Store components and disable movement
            playerController = player.GetComponent<ThirdPersonController>();
            characterController = player.GetComponent<CharacterController>();

            if (playerController != null)
                playerController.enabled = false;
            if (characterController != null)
                characterController.enabled = false;

            // 1. Fade out to black
            yield return StartCoroutine(UIManager.instance.FadeOut());

            // 2. Move and orient player while screen is BLACK
            Transform hidePosition = currentBasket.transform.Find("HidePosition");

            if (hidePosition != null)
            {
                player.transform.position = hidePosition.position;
                player.transform.rotation = hidePosition.rotation;
            }
            else
            {
                Vector3 basketPosition = currentBasket.transform.position;

                // Position inside basket
                player.transform.position = new Vector3(
                    basketPosition.x,
                    basketPosition.y + 0.5f, // Adjust this based on your basket height
                    basketPosition.z
                );

                // Keep player upright (don't rotate)
                player.transform.rotation = Quaternion.identity;
                // Or face a specific direction:
                // player.transform.rotation = Quaternion.LookRotation(Vector3.forward);
            }

            // 3. Wait a moment while screen stays black
            yield return new WaitForSeconds(0.2f);

            // 4. Fade back in
            yield return StartCoroutine(UIManager.instance.FadeIn());

            // 5. Re-enable components
            if (characterController != null)
                characterController.enabled = true;
            if (playerController != null)
                playerController.enabled = true;
        }
    }

    private void InteractWithObject()
    {
        Debug.Log("Interacting with object");
        currentLight.SetActive(true);
        AudioManager.instance.PlayAudio(currentAudio);
    }
}
