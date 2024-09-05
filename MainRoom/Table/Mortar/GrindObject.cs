using UnityEngine;

public class GrindObject : MonoBehaviour
{
    public string id;
    public float amount, originalAmount;
    public InventoryManager inventoryManager;
    public MortarView mortarView;
    private Camera mainCamera;

    private ParticleSystem dustEffectInstance;
    private Vector3 originalScale;

    private void Start(){
        mainCamera = Camera.main;
        originalScale = transform.localScale;
        if (mortarView.dustEffectPrefab){
            dustEffectInstance = Instantiate(mortarView.dustEffectPrefab, transform.position, Quaternion.identity);
            dustEffectInstance.GetComponent<ParticleCollider>().grindObject = this;
            dustEffectInstance.GetComponent<ParticleCollider>().inventoryManager = inventoryManager;
            dustEffectInstance.Stop();
        }
    }

    private Vector2 lastMouseMove;

    private void OnMouseOver(){
        if(inventoryManager.currentSelectedItem != "PISTIL")
            return;

        Vector2 old = lastMouseMove;
        if(lastMouseMove == null){
            lastMouseMove = Input.mousePosition;
            return;
        }

        lastMouseMove = Input.mousePosition;

        if(old == lastMouseMove){
         //   dustEffectInstance.Pause();
            return;
        }
        Vector3 scale = new Vector3(transform.localScale.x - (originalScale.x * 0.002f), transform.localScale.y - (originalScale.y * 0.002f), 1);
        transform.localScale = scale;

        if(scale.x <= 0.0f || scale.y <= 0.0f || amount <= 0.0f){
            mortarView.destroyGrindObject(this);
            return;
        }

        float toGive = originalAmount * 0.002f;

        amount -= toGive;
        mortarView.dataHolder.modifyInventory("GRINDED_" + id, toGive, DataModifyType.ADD);
            

        if(dustEffectInstance){
            var emission = dustEffectInstance.emission;
            emission.rateOverTime = 10;
            dustEffectInstance.Play();
        }
    }

    private void OnMouseExit(){
       // dustEffectInstance.Pause();
    }

    public void onClick(){
        mortarView.destroyGrindObject(this);
    }

    private void OnDestroy(){
        if (dustEffectInstance)
            Destroy(dustEffectInstance.gameObject);
    }
}
