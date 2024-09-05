using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour 
{
    public InventoryType inventoryType = InventoryType.BREW;
    public GameObject slotPrefab;
    public float horizontalSlotSpacing = 1.1f;
    public float verticalSlotSpacing = 1.1f;
    public float scrollSpeed = 10f;
    public Transform firstSlotLocation;
    public string currentSelectedItem;
    public TextInputField scaleInputField;

    public float selectedDropAmount;
    private List<Transform> slots = new List<Transform>();
    public DataHolder dataHolder;

    private Bounds bounds;
    private CustomBound customBound;
    private GameObject iconMouse;
    private SpriteRenderer iconMouseRenderer;
    private Camera mainCamera;

    void Awake(){
        dataHolder = transform.parent.parent.GetComponent<DataHolder>();
        Collider2D inventoryBounds = GetComponent<BoxCollider2D>();
        bounds = inventoryBounds.bounds;
        customBound = new CustomBound(this.gameObject, bounds, this.transform.position);
        inventoryBounds.enabled = false;
        mainCamera = Camera.main;
        refreshInventory();
    }

    void OnEnable(){
        refreshInventory();

        if(inventoryType == InventoryType.POTIONS)
            return;

        scaleInputField.gameObject.SetActive(true);
        scaleInputField.inventoryManager = this;
    }

    public void OnDisable(){
        scaleInputField.gameObject.SetActive(false);
        Destroy(iconMouse);
        iconMouse = null;
        Cursor.visible = true;
        currentSelectedItem = null;
    }

    void LateUpdate(){
        updateIcon();
        handleScroll();
    }

    private void updateIcon(){
        if(currentSelectedItem != null && currentSelectedItem.Length > 0 && iconMouse == null){
            iconMouse = Instantiate(slotPrefab, transform.parent);
            iconMouseRenderer = iconMouse.GetComponent<SpriteRenderer>();
            iconMouseRenderer.sprite = dataHolder.GetSprite(currentSelectedItem);
            Cursor.visible = false;
        }

        if((currentSelectedItem == null || currentSelectedItem.Length < 1) && iconMouse != null){
            Destroy(iconMouse);
            iconMouse = null;
            Cursor.visible = true;
        }

        if(iconMouse != null){
            Vector3 mouseScreenPosition = Input.mousePosition;
            iconMouse.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
        }
    }

    public void destroyIcon(){
        Destroy(iconMouse);
        iconMouse = null;
        Cursor.visible = true;
        currentSelectedItem = null;
    }

    private void handleScroll(){
        if(slots.Count < 1)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0)
            return;

        bool canScrollUp = !customBound.isFullyInside(slots[0].GetComponent<BoxCollider2D>().bounds, slots[0].position);
        bool canScrollDown = !customBound.isFullyInside(slots[slots.Count - 1].GetComponent<BoxCollider2D>().bounds, slots[slots.Count - 1].position);

        if ((scroll > 0 && !canScrollUp) || (scroll < 0 && !canScrollDown))
            return;

        foreach (Transform slot in slots) {
            Vector3 position = slot.localPosition;
            position.y -= scroll * scrollSpeed * Time.deltaTime;
            slot.localPosition = position;

            slot.gameObject.SetActive(customBound.isInside(slot.GetComponent<BoxCollider2D>().bounds, slot.position));
        }
    }

    private Transform CreateSlot(string id, int index) {//0.2f 0.16f, 1f
        GameObject instance = Instantiate(slotPrefab, transform);
        instance.transform.localScale = new Vector3(0.2f, 0.16f, 1f);
        instance.transform.position = new Vector3(instance.transform.position.x, instance.transform.position.y, -0.1f);

        SlotData slotData = instance.GetComponent<SlotData>();
        slotData.holdingItemID = id;
        slotData.textMeshPro.text = PotScript.formatDouble(dataHolder.getItemAmount(id)) + " g";

        GameObject iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(instance.transform);
        SpriteRenderer iconRenderer = iconObject.AddComponent<SpriteRenderer>();
        iconRenderer.sprite = dataHolder.GetSprite(id);
        iconObject.transform.localPosition = Vector3.zero;
        iconObject.transform.localScale = new Vector3(1.5f, 1.2f, 1f);
        iconObject.transform.position = new Vector3(iconObject.transform.position.x, iconObject.transform.position.y, -0.1f);

        int row = index / 2;
        int column = index % 2;

        float xPosition = firstSlotLocation.localPosition.x + column * horizontalSlotSpacing;
        float yPosition = firstSlotLocation.localPosition.y - row * verticalSlotSpacing;

        instance.transform.localPosition = new Vector3(xPosition, yPosition, 0);
        return instance.transform;
    }

    public void refreshInventory() {
        ClearSlots();

        if(currentSelectedItem != null && dataHolder.getItemAmount(currentSelectedItem) <= 0.0f)
            currentSelectedItem = null;

        int i = 0;
        foreach (string id in dataHolder.inventoryData.Keys) {
            if (id == null || id.Length < 1 || !isCorrectItem(id))
                continue;

            Transform slot = CreateSlot(id, i++);
            slots.Add(slot);
        }
    }

    public void refreshInventory(string potion) {
        ClearSlots();

        if(currentSelectedItem != null && dataHolder.getItemAmount(currentSelectedItem) <= 0.0f)
            currentSelectedItem = null;

        int i = 0;
        foreach (string id in dataHolder.inventoryData.Keys) {
            if (id == null || id.Length < 1 || !isCorrectItem(id) || !id.Contains(potion))
                continue;

            Transform slot = CreateSlot(id, i++);
            slots.Add(slot);
        }
    }

    public void sortInventory(Coords coords){
        if (slots.Count == 0)
            return;

        Dictionary<string, float[]> itemsToSort = new Dictionary<string, float[]>();
        foreach (string id in dataHolder.inventoryData.Keys){
            if (id == null || id.Length < 1 || !isCorrectItem(id))
                continue;

            float amount = dataHolder.getItemAmount(id);
            if (amount <= 0.0f)
                continue;

            itemsToSort.Add(id, new float[] { dataHolder.moveDirectionsPer0_1g[id][0], dataHolder.moveDirectionsPer0_1g[id][1] });
        }

        var sortedItems = itemsToSort.OrderBy(item => {
            float[] coordsArray = item.Value;
            switch (coords) {
                case Coords.UPPER:
                    return -coordsArray[1];
                case Coords.DOWN:
                    return coordsArray[1];
                case Coords.LEFT:
                    return coordsArray[0];
                case Coords.RIGHT:
                    return -coordsArray[0];
                default:
                    return 0;
            }
        }).ToList();

        ClearSlots();

        int i = 0;
        foreach (var item in sortedItems){
            Transform slot = CreateSlot(item.Key, i++);
            slots.Add(slot);
        }
    }

    private bool isCorrectItem(string id){
        ItemType type = dataHolder.GetItemType(id);

        if(type == ItemType.NULL)
            return false;

        if(inventoryType == InventoryType.BREW && type == ItemType.POTION)
            return false;

        if(inventoryType == InventoryType.PREPARE_RAW_INGREDIENTS && type != ItemType.RAW)
            return false;

        if(inventoryType == InventoryType.POTIONS && type != ItemType.POTION)
            return false;

        return true;
    }

    private void ClearSlots() {
        foreach (Transform child in slots) 
            Destroy(child.gameObject);
        
        slots.Clear();
    }


}
