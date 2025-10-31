using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

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

    [SerializeField]
    private float fadeDuration = 1f;
    [SerializeField]
    private Image fadeImage;

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
            case InteractionType.HidingInBasket:
                instructionPopupText.text = "Press E to hide in basket";
                break;
            case InteractionType.InteractiveObject:
                instructionPopupText.text = "Press E to turn this on";
                break;
            default:
                break;
        }
    }

    public IEnumerator FadeOut()
    {
        // Fade to black
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0, 1, normalizedTime);
            fadeImage.color = color;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    public IEnumerator FadeIn()
    {
        // Fade from black
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(1, 0, normalizedTime);
            fadeImage.color = color;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }
}
