using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    public PlayerInput playerInput;

    // Event for AI system
    public static event Action<Vector3> OnPlayerInteraction;

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
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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

        if (switchAction.triggered)
        {
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
        currentAudio = audioClip;
        currentInteractionType = interactionType;
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

            // Trigger AI event - tenant might investigate
            OnPlayerInteraction?.Invoke(currentLight.transform.position);
        }
    }

    private void DrawCurtains()
    {
        if (currentCurtain != null)
        {
            var animator = currentCurtain.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("draw");
            }

            // Trigger AI event
            OnPlayerInteraction?.Invoke(currentCurtain.transform.position);
        }
        SwitchOffLight();
    }

    private void HideInBasket()
    {
        UIManager.instance.HideInstructionPopup();

        // Trigger AI event before hiding
        if (currentBasket != null)
        {
            OnPlayerInteraction?.Invoke(currentBasket.transform.position);
        }

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

                // Keep player upright
                player.transform.rotation = Quaternion.identity;
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
        if (currentLight != null)
        {
            currentLight.SetActive(true);

            // Trigger AI event for interactive objects
            OnPlayerInteraction?.Invoke(currentLight.transform.position);
        }

        if (currentAudio != null)
        {
            AudioManager.instance.PlayAudio(currentAudio);
        }
    }
}