using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public PlayerInput playerInput;

    public GameObject instructionPopup;
    public Animator instructionPopupAnim;

    [Header("Popup Animation")]
    public float moveDuration = 2f;      // Duration of the sliding animation
    public float moveDistance = 500f;   // Distance the popup moves off-screen

    public bool playerInRange;

    private RectTransform instructionPopupRectTransform;
    private Vector2 originalPosition;

    [HideInInspector]
    public PopupType currentPopupType;

    [SerializeField]
    private bool isInstructionOn;

    private Tween instructionTween;
}
