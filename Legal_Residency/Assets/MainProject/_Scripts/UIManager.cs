using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //public PlayerInput playerInput;

    public GameObject instructionPopup;
    public Animator instructionPopupAnim;

    [Header("Popup Animation")]
    public float moveDuration = 2f;      // Duration of the sliding animation
    public float moveDistance = 500f;   // Distance the popup moves off-screen


    private RectTransform instructionPopupRectTransform;
    private Vector2 originalPosition;


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

    private void Update()
    {
        //if (playerInput.actions.FindAction("Build").triggered)
        //{
        //    if (playerInRange)
        //        SwitchOffLight();
        //}
    }

    public void ShowLightBoardInstruction()
    {
        if (!isInstructionOn)
        {
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

    public void HideLightBoardInstruction()
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
                    instructionPopup.SetActive(false);
                });
        }
    }

    private void SwitchOffLight()
    {

    }
}
