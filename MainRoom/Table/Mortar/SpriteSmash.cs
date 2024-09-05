using System.Collections.Generic;
using UnityEngine;

public class SpriteBreaker : MonoBehaviour
{
    public int initialPiecesPerRow = 2; // Initial number of pieces per row/column (2x2 for 4 pieces)
    public int piecesPerRow = 4; // Number of pieces per row/column after breaking each piece
    public float breakDuration = 5f; // Duration over which the sprite breaks
    public float spreadAmount = 0.5f; // Amount to spread the pieces
    public float mouseEffectRadius = 0.5f; // Radius of the mouse effect
    public float forceAmount = 2f; // Amount of force applied to the pieces
    public float alphaThreshold = 0.1f; // Threshold for transparency to exclude pieces

    private SpriteRenderer spriteRenderer;
    private List<GameObject> pieces = new List<GameObject>();
    private bool isBreaking = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BreakInitialSprite();
    }

    void Update()
    {
        if (isBreaking)
        {
            UpdatePieces();
        }
    }

    void OnMouseOver()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        foreach (GameObject piece in pieces)
        {
            if (piece.GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                BreakPiece(piece, mousePos);
            }
        }
    }

    void BreakInitialSprite()
    {
        Texture2D texture = spriteRenderer.sprite.texture;
        int pieceWidth = texture.width / initialPiecesPerRow;
        int pieceHeight = texture.height / initialPiecesPerRow;

        Bounds spriteBounds = spriteRenderer.bounds;
        Vector3 spriteSize = spriteBounds.size;
        Vector3 spriteMin = spriteBounds.min;

        for (int x = 0; x < initialPiecesPerRow; x++)
        {
            for (int y = 0; y < initialPiecesPerRow; y++)
            {
                Rect pieceRect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
                Sprite pieceSprite = Sprite.Create(texture, pieceRect, new Vector2(0.5f, 0.5f));
                GameObject pieceObject = new GameObject("Piece");
                SpriteRenderer pieceRenderer = pieceObject.AddComponent<SpriteRenderer>();
                pieceRenderer.sprite = pieceSprite;

                float posX = spriteMin.x + (spriteSize.x / initialPiecesPerRow) * (x + 0.5f);
                float posY = spriteMin.y + (spriteSize.y / initialPiecesPerRow) * (y + 0.5f);
                pieceObject.transform.position = new Vector3(posX, posY, transform.position.z);
                pieceObject.transform.parent = transform;

                pieces.Add(pieceObject);

                pieceObject.AddComponent<BoxCollider2D>();
                Rigidbody2D rb = pieceObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.mass = 1f;
                rb.drag = 5f; // Increase drag to prevent floating away
            }
        }

        spriteRenderer.enabled = false; // Hide the original sprite
    }

    void BreakPiece(GameObject piece, Vector3 mousePos)
    {
        if (piece.transform.childCount > 0) return; // Already broken

        Texture2D texture = piece.GetComponent<SpriteRenderer>().sprite.texture;
        int pieceWidth = texture.width / piecesPerRow;
        int pieceHeight = texture.height / piecesPerRow;

        Bounds pieceBounds = piece.GetComponent<SpriteRenderer>().bounds;
        Vector3 pieceSize = pieceBounds.size;
        Vector3 pieceMin = pieceBounds.min;

        for (int x = 0; x < piecesPerRow; x++)
        {
            for (int y = 0; y < piecesPerRow; y++)
            {
                Rect pieceRect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
                Color[] pixels = texture.GetPixels((int)pieceRect.x, (int)pieceRect.y, (int)pieceRect.width, (int)pieceRect.height);
                bool isTransparent = true;

                foreach (Color pixel in pixels)
                {
                    if (pixel.a > alphaThreshold)
                    {
                        isTransparent = false;
                        break;
                    }
                }

                if (!isTransparent)
                {
                    Sprite pieceSprite = Sprite.Create(texture, pieceRect, new Vector2(0.5f, 0.5f));
                    GameObject pieceObject = new GameObject("SubPiece");
                    SpriteRenderer pieceRenderer = pieceObject.AddComponent<SpriteRenderer>();
                    pieceRenderer.sprite = pieceSprite;

                    float posX = pieceMin.x + (pieceSize.x / piecesPerRow) * (x + 0.5f);
                    float posY = pieceMin.y + (pieceSize.y / piecesPerRow) * (y + 0.5f);
                    pieceObject.transform.position = new Vector3(posX, posY, piece.transform.position.z);
                    pieceObject.transform.parent = piece.transform;

                    pieces.Add(pieceObject);

                    pieceObject.AddComponent<BoxCollider2D>();
                    Rigidbody2D rb = pieceObject.AddComponent<Rigidbody2D>();
                    rb.gravityScale = 0;
                    rb.mass = 0.25f;
                    rb.drag = 10f; // Increase drag to prevent floating away

                    Vector2 force = (pieceObject.transform.position - mousePos).normalized * forceAmount;
                    rb.AddForce(force);
                }
            }
        }

        Destroy(piece.GetComponent<BoxCollider2D>());
        Destroy(piece.GetComponent<Rigidbody2D>());
    }

    void UpdatePieces()
    {
        foreach (GameObject piece in pieces)
        {
            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 localPosition = piece.transform.localPosition;
                localPosition.x += Random.Range(-spreadAmount, spreadAmount) * Time.deltaTime;
                localPosition.y += Random.Range(-spreadAmount, spreadAmount) * Time.deltaTime;
                piece.transform.localPosition = localPosition;
            }
        }
    }
}
