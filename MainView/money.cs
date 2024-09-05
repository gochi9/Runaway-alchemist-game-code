using TMPro;
using UnityEngine;

public class money : MonoBehaviour
{
    public DataHolder dataHolder;
    private TextMeshPro textMeshPro;

    void Awake(){
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public void OnEnable(){
        textMeshPro.text = dataHolder.money.ToString();
    }
}
