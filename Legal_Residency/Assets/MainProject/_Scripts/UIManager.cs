using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Instruction Popup Variables")]
    public GameObject instructionPopup;
    public Animator instructionPopupAnim;
    public TMP_Text instructionPopupText; 

    [Header("Popup Animation")]
    public float moveDuration = 2f;      // Duration of the sliding animation
    public float moveDistance = 500f;   // Distance the popup moves off-screen


    private RectTransform instructionPopupRectTransform;
    private Vector2 originalPosition;

    public GameObject spotLight;


    [SerializeField]
    private bool isInstructionOn;

    private Tween instructionTween;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        instructionPopupRectTransform = instructionPopup.GetComponent<RectTransform>();

        originalPosition = instructionPopupRectTransform.anchoredPosition;

        instructionPopupRectTransform.anchoredPosition = new Vector2(originalPosition.x + moveDistance, originalPosition.y);

        isInstructionOn = false;
    }

    public void ShowInstructionPopup(InteractionType interactionType)
    {
        if (!isInstructionOn)
        {
            AssignInstructionText(interactionType);
            instructionPopup.SetActive(true);
            isInstructionOn = true;

            instructionTween?.Kill();
            // Slide the popup into the original position from the right
            instructionTween = instructionPopupRectTransform.DOAnchorPos(originalPosition, moveDuration)
                .SetEase(Ease.Flash)
                .OnComplete(() =>
                {
                    instructionPopupAnim.SetBool("flash", true);
                });
        }
    }

    public void HideInstructionPopup()
    {
        if (isInstructionOn)
        {
            isInstructionOn = false;

            // Stop the "flash" animation
            instructionPopupAnim.SetBool("flash", false);

            // Kill any existing tween on the RectTransform to prevent conflicts
            instructionTween?.Kill();

            // Slide the popup back off-screen to the right
            instructionTween = instructionPopupRectTransform.DOAnchorPos(new Vector2(originalPosition.x + moveDistance, originalPosition.y), moveDuration)
                .SetEase(Ease.Flash)
                .OnComplete(() =>
                {
                    instructionPopupText.text = "";
                    instructionPopup.SetActive(false);
                });
        }
    }

    private void AssignInstructionText(InteractionType interactionType)
    {
        switch (interactionType)
        {
            case InteractionType.FlickingSwitch:
                instructionPopupText.text = "Press E to turn off light";
                break;
            case InteractionType.DrawingCurtains:
                instructionPopupText.text = "Press E to draw curtains";
                break;
        }
    }
}
