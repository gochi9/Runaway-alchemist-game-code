using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1.0f;
    public float fadeDuration = 1.0f;
    public TextMeshPro text;
    private Color originalColor;
    private float timeElapsed;

    void Start(){
        if (text == null)
            text = GetComponent<TextMeshPro>();

        originalColor = text.color;
    }

    void Update(){
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        timeElapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(1.0f, 0.0f, timeElapsed / fadeDuration);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (alpha <= 0)
            Destroy(gameObject);
    }

    public void setText(string message){
        if (text == null)
            text = GetComponent<TextMeshPro>();
        
        text.text = message;
    }
}
