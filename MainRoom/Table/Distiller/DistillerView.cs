using Unity.VisualScripting;
using UnityEngine;

public class DistillerView : MonoBehaviour
{
    public GameObject bar, slider, bigWater, smallWater, distil_hit_zone;
    public DataHolder dataHolder;
    public InventoryManager inventoryManager;

    private Vector3[] bigWaterPositions = {
        new Vector3(0, 0, -0.1f),
        new Vector3(0, -0.1451f, -0.1f),
        new Vector3(0, -0.2488f, -0.1f),
        new Vector3(0, -0.2695f, -0.1f),
        new Vector3(0, -0.3317f, -0.1f),
        new Vector3(0, -0.4354f, -0.1f),
        new Vector3(0, -0.591f, -0.1f),
        new Vector3(0, -0.7051f, -0.1f),
        new Vector3(0, -0.9f, -0.1f),
        new Vector3(0, 0, -0.1f),
    };

    private Vector2[] bigWaterScale = {
        new Vector2(1.38889f, 1.38889f),
        new Vector2(1.38889f, 1.285122f),
        new Vector2(1.38889f, 1.18719f),
        new Vector2(1.38889f, 1.118059f),
        new Vector2(1.38889f, 1.031637f),
        new Vector2(1.38889f, 0.9337263f),
        new Vector2(1.38889f, 0.855931f),
        new Vector2(1.38889f, 0.795382f),
        new Vector2(1.38889f, 0.7118287f),
        new Vector2(1.38889f, 0f),
    };

    private Vector3[] smallWaterPositions = {
        new Vector3(0, 0, -0.1f),
        new Vector3(0, -1.62f, -0.1f),
        new Vector3(0, -1.4825f, -0.1f),
        new Vector3(0, -1.1248f, -0.1f),
        new Vector3(0, -0.9689f, -0.1f),
        new Vector3(0, -0.8313f, -0.1f),
        new Vector3(0, -0.6889f, -0.1f),
        new Vector3(0, -0.5583f, -0.1f),
        new Vector3(0, -0.2971f, -0.1f),
        new Vector3(0, 0, -0.1f),
    };

    private Vector2[] smallWaterScale = {
        new Vector2(1.38889f, 0f),
        new Vector2(1.38889f, 0.3410169f),
        new Vector2(1.38889f, 0.3996097f),
        new Vector2(1.38889f, 0.6365338f),
        new Vector2(1.38889f, 0.7511609f),
        new Vector2(1.38889f, 0.8556688f),
        new Vector2(1.38889f, 0.9479998f),
        new Vector2(1.38889f, 1.030413f),
        new Vector2(1.38889f, 1.188673f),
        new Vector2(1.38889f, 1.38889f),
    };

    private Transform barTransform, sliderTransform;
    private float sliderSpeedDef = 2f;
    private float sliderSpeed;
    private bool movingRight = true;
    private int level = 0;
    private CustomBound zone;
    private GameObject hitZone;
    public string currentSelectedItem;
    public float amountSelected, amountToGivePerSuccess;
    public bool started;
    private BoxCollider2D sliderCollider;
    void Awake(){
        bigWater.transform.position = bigWaterPositions[0];
        bigWater.transform.localScale = bigWaterScale[0];
        smallWater.transform.position = smallWaterPositions[0];
        smallWater.transform.localScale = smallWaterScale[0];

        barTransform = bar.GetComponent<Transform>();
        sliderTransform = slider.GetComponent<Transform>();
        sliderCollider = slider.GetComponent<BoxCollider2D>();
    }

    void Update(){
        if(!started)
            return;

        moveSlider();
        checkInput();
    }

    void OnDisable(){
        currentSelectedItem = null;
        amountSelected = 0;
        started = false;
        level = 0;
        sliderSpeed = sliderSpeedDef;
        if (hitZone != null && !hitZone.IsDestroyed())
            Destroy(hitZone);

        bigWater.transform.position = bigWaterPositions[0];
        smallWater.transform.position = smallWaterPositions[0];
    }

    public void startMinigame(){
        float amount = dataHolder.getItemAmount(currentSelectedItem);

        if(amountSelected > amount)
            amountSelected = amount;

        if(!(amountSelected > 0.0f)){
            OnDisable();
            return;
        }

        started = true;
        amountToGivePerSuccess = amountSelected / 10;
        initializeLevel();
        dataHolder.modifyInventory(currentSelectedItem, amountSelected, DataModifyType.REMOVE);
        inventoryManager.refreshInventory();
    }

    void moveSlider(){
        float step = sliderSpeed * Time.deltaTime;
        Vector3 newPosition = sliderTransform.position;

        if (movingRight){
            newPosition.x += step;
            if (newPosition.x >= barTransform.position.x + barTransform.localScale.x / 2)
                movingRight = false;
        }
        else{
            newPosition.x -= step;
            if (newPosition.x <= barTransform.position.x - barTransform.localScale.x / 2)
                movingRight = true;
        }

        sliderTransform.position = newPosition;
    }

    void checkInput(){
        if (!Input.GetKeyDown(KeyCode.Space))
            return;


        if (!zone.isInside(sliderCollider.bounds, slider.transform.position)){
            levelUp();
            return;
        }

        dataHolder.modifyInventory("DISTILLED_" + currentSelectedItem, amountToGivePerSuccess, DataModifyType.ADD);
        dataHolder.showFloatingText("+" + amountToGivePerSuccess + " g", new Vector2(0,0));
        levelUp();
    }

    void initializeLevel() {
        if (hitZone != null && !hitZone.Equals(null))
            Destroy(hitZone);

        float zoneWidth = barTransform.lossyScale.x / (2 + level);
        float minX = barTransform.position.x - barTransform.lossyScale.x / 2 + zoneWidth / 2;
        float maxX = barTransform.position.x + barTransform.lossyScale.x / 2 - zoneWidth / 2;
        float zoneX = Random.Range(minX, maxX);

        hitZone = new GameObject("HitZone");
        hitZone.transform.parent = transform;
        hitZone.transform.position = new Vector3(zoneX, sliderTransform.position.y + 0.0361f, -0.15f);

        SpriteRenderer renderer = hitZone.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateDarkSprite();
        hitZone.transform.localScale = new Vector3(zoneWidth, barTransform.lossyScale.y, 1f);
        BoxCollider2D boxCollider2D = renderer.AddComponent<BoxCollider2D>();

        zone = new CustomBound(null, boxCollider2D.bounds, boxCollider2D.bounds.center);
        sliderSpeed = 2f + level * 3.5f;
    }

    Sprite CreateDarkSprite() {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.black);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }


    void levelUp(){
        if(level == 9){
            OnDisable();
            return;
        }

        level = Mathf.Min(level + 1, 9);

        bigWater.transform.position = bigWaterPositions[level];
        bigWater.transform.localScale = bigWaterScale[level];
        smallWater.transform.position = smallWaterPositions[level];
        smallWater.transform.localScale = smallWaterScale[level];

        initializeLevel();
    }
}
