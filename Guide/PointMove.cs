using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class PointMove : MonoBehaviour {
    public List<GameObject> warningColliders = new();
    public List<GameObject> deathColliders = new();
    public List<GameObject> potionColliders = new();
    public PotScript potScript;
    private Dictionary<CustomBound, BoundType> colliders = new();
    public DialogueManager dialogueManager;

    private Collider2D playerCollider;
    public bool moving = false;

    void Awake() {
        playerCollider = GetComponent<Collider2D>();
        foreach(GameObject gameObject in warningColliders)
            try{
                deathColliders.Add(gameObject.transform.GetChild(0).gameObject);
            }
            catch(Exception){continue;}
        helperMap(warningColliders, BoundType.WARNING);
        helperMap(deathColliders, BoundType.DEATH);
        helperMap(potionColliders, BoundType.POTION);
    }
    
    public float targetx = 0.0f;
    public float targety = 0.0f;
    float threshold = 0.00011f;

    public void movePoint(float x, float y) {
        targetx += x;
        targety += y;
    }

    bool check = false;
    void Update() {
        Vector2 pos = transform.position;

        if(Mathf.Abs(pos.x - targetx) > threshold){
            pos.x += targetx > pos.x ? 0.0001f : -0.0001f;
            check = true;
        }
            
        if(Mathf.Abs(pos.y - targety) > threshold){
            pos.y += targety > pos.y ? 0.0001f : -0.0001f;
            check = true;    
        }

        if(check){
            moving = true;
            transform.position = pos;
            check = false;
            checkCollisions();
        }
        else
            moving = false;
    }

    bool hasTouchedWarning = false, hasTouchedPotion = false;

    private void checkCollisions() {
        if(cooldown != 0 && cooldown - DateTimeOffset.Now.ToUnixTimeMilliseconds() >= 0)
            return;
        
        cooldown = 0;
        Bounds bounds = playerCollider.bounds;
        Vector2 pos = transform.position;

        float proximityX = Mathf.Min((float)(Mathf.Abs(transform.position.x) / 0.49), 1f);
        float proximityY = Mathf.Min((float)(Mathf.Abs(transform.position.y) / 0.49), 1f);
        bool posX = transform.position.x >= 0, posY = transform.position.y >= 0;

        if(potScript != null && potScript.gameObject.activeSelf)
            potScript.updateCircle(posX ? proximityX : -proximityX, posY ? proximityY : -proximityY);

        hasTouchedWarning = false;
        hasTouchedPotion = false;
        float warning = 0f;
        foreach (CustomBound bound in colliders.Keys) {
            if (!bound.isInside(bounds, pos))
                continue;

            switch (colliders[bound]) {
                case BoundType.WARNING:
                    warning = handleWarningCollision(bound, playerCollider);
                    hasTouchedWarning = true;
                    break;
                case BoundType.DEATH:
                    potScript.potionProgress = 0f;
                    potScript.currentPotion = null;
                    potScript.failProgress = 0f;
                    handleDeathCollision();
                    return;
                case BoundType.POTION:
                    handlePotionCollision(bound, playerCollider);
                    hasTouchedPotion = true;
                    if(potScript.currentPotion == null)
                        potScript.currentPotion = bound.gameObject.GetComponent<PotionData>();

                    break;
            }
        }

        if(!hasTouchedPotion){
            potScript.potionProgress = 0f;
            potScript.currentPotion = null;
        }

        potScript.failProgress = !hasTouchedWarning ? 0 : warning;
    }

    private float handleWarningCollision(CustomBound bound, Collider2D collider) {
        float[] proximity = bound.distanceToDeathCollider(collider.bounds);

        float value = Math.Abs(proximity.Max());
        return Mathf.Abs(value >= 1f ? Math.Abs(proximity.Min()) : 0f);
    }
    private long cooldown;
    private void handleDeathCollision() {
        if(cooldown - DateTimeOffset.Now.ToUnixTimeMilliseconds() >= 0)
            return;

        targetx = 0.0f;
        targety = 0.0f;
        check = false;
        transform.position = new Vector3(targetx, targety, -0.1f);
        potScript.resetPotion();
        dialogueManager.startDialogue("POTION_FAIL");
        cooldown = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 1000;
    }

    private void handlePotionCollision(CustomBound bound, Collider2D collider) {
        Vector2 potionPos = bound.gameObject.transform.position;
        Vector2 playerPos = playerCollider.bounds.center;

        float distance = Vector2.Distance(playerPos, potionPos);
        float maxDistance = Mathf.Max(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y) + Mathf.Max(bound.gameObject.GetComponent<Collider2D>().bounds.extents.x, bound.gameObject.GetComponent<Collider2D>().bounds.extents.y);
        float normalizedDistance = 1 - Mathf.Clamp01(distance / maxDistance);

        potScript.potionProgress = normalizedDistance;
    }

    private void helperMap(List<GameObject> list, BoundType type) {
        foreach (GameObject gameObject in list) {
            if(!gameObject.activeInHierarchy)
                continue;
            Collider2D collider2D = gameObject.GetComponent<Collider2D>();

            if (collider2D != null)
                colliders.Add(new CustomBound(gameObject, collider2D.bounds, gameObject.transform.position), type);
        }
    }
}