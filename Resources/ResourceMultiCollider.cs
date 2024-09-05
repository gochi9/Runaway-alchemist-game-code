using UnityEngine;

public class ResourceMultiCollider : MonoBehaviour
{

    private ResourcesData parent;
    private BoxCollider2D boxCollider;

    void Awake(){
        boxCollider = GetComponent<BoxCollider2D>();
        Transform t = transform.parent;

        if(t == null)
            return;

        parent = t.gameObject.GetComponent<ResourcesData>();
    }

    void OnMouseDown() {
        if(parent != null && boxCollider != null)
            parent.resourceClick(boxCollider.bounds.center);
    }

    public void OnMouseOver(){
        Cursor.SetCursor(AssetsHolder.cursor2, Vector2.zero, CursorMode.Auto);
    }

    public void OnMouseExit(){
        Cursor.SetCursor(AssetsHolder.cursor1, Vector2.zero, CursorMode.Auto);
    }

    public void OnDisable(){
        OnMouseExit();
    }
}
