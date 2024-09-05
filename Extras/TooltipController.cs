using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{
    public GameObject tooltip;
    private SpriteRenderer spriteRenderer;
    public TextMeshPro titleText;
    public TextMeshPro descriptionText;

    private bool isShowing = false;
    private Coroutine fadeCoroutine;
    private Camera mainCamera;

    void Awake(){
        spriteRenderer = tooltip.GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        tooltip.SetActive(false);
    }

    void Update(){
        if (isShowing)
            return;

        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 newPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x + 250, mouseScreenPosition.y, mainCamera.nearClipPlane));
        newPos.z = -7f;
        tooltip.transform.position = newPos;
    }

    public void showTooltip(string title, string description){
        tooltip.SetActive(true);
        titleText.text = title;
        descriptionText.text = description;
    }

    public void hide(){
        StopAllCoroutines();
        tooltip.SetActive(false);
    }
}
