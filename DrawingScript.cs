using System.IO;
using UnityEngine;

//scrapped/postponed idea
public class DrawingScript : MonoBehaviour
{
    public Camera drawingCamera;
    public RenderTexture renderTexture;
    public SpriteRenderer spriteRenderer;
    public Material drawMaterial;

    private Texture2D drawingTexture;
    private bool isDrawing;
    private Vector3 previousPosition;

    void Start(){
        drawingTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        drawingTexture.filterMode = FilterMode.Point;
        drawingTexture.wrapMode = TextureWrapMode.Clamp;

        spriteRenderer.sprite = Sprite.Create(drawingTexture, new Rect(0, 0, drawingTexture.width, drawingTexture.height), new Vector2(0.5f, 0.5f));
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            isDrawing = true;
            previousPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
            isDrawing = false;


        if (isDrawing)
            Draw();
    }

    void Draw(){
        Vector3 currentPosition = Input.mousePosition;

        RaycastHit hit;
        Ray ray = drawingCamera.ScreenPointToRay(currentPosition);

        if(!Physics.Raycast(ray, out hit))
            return;


        RenderTexture.active = renderTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, renderTexture.width, renderTexture.height, 0);

        drawMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(Color.black);
        GL.Vertex3(previousPosition.x, previousPosition.y, 0);
        GL.Vertex3(currentPosition.x, currentPosition.y, 0);
        GL.End();

        GL.PopMatrix();
        RenderTexture.active = null;

        RenderTexture.active = renderTexture;
        drawingTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        drawingTexture.Apply();
        RenderTexture.active = null;

        previousPosition = currentPosition;
    }

    public void SaveDrawing(){
        byte[] bytes = drawingTexture.EncodeToPNG();
        string path = Application.persistentDataPath + "/drawing.png";
        File.WriteAllBytes(path, bytes);

        Debug.Log("Drawing saved to " + path);
    }

    public void LoadDrawing(){
        string path = Application.persistentDataPath + "/drawing.png";
        if (!File.Exists(path))
            return;

        byte[] bytes = File.ReadAllBytes(path);
        drawingTexture.LoadImage(bytes);
        drawingTexture.Apply();
    }
}
