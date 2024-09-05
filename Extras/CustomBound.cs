using UnityEngine;

public class CustomBound {

    public GameObject gameObject { get; private set;}
    public float minX { get; private set;} 
    public float maxX { get; private set;}
    public float minY { get; private set;} 
    public float maxY { get; private set;} 

    public CustomBound(GameObject gameObject, Bounds bounds, Vector2 pos){
        this.gameObject = gameObject;
        this.minX = pos.x - bounds.size.x / 2;
        this.maxX = pos.x + bounds.size.x / 2;
        this.minY = pos.y - bounds.size.y / 2;
        this.maxY = pos.y + bounds.size.y / 2;
    }

    public bool isInside(Bounds bounds, Vector2 pos) {
        float otherMinX = pos.x - bounds.size.x / 2;
        float otherMaxX = pos.x + bounds.size.x / 2;
        float otherMinY = pos.y - bounds.size.y / 2;
        float otherMaxY = pos.y + bounds.size.y / 2;

        return (otherMinX < this.maxX && otherMaxX > this.minX) && (otherMinY < this.maxY && otherMaxY > this.minY);;
    }

    public bool isInsideX(Bounds bounds, Vector2 pos) {
        float otherMinX = pos.x - bounds.size.x / 2;
        float otherMaxX = pos.x + bounds.size.x / 2;

      //  return (otherMinX >= minX && otherMinX <= maxX) || (otherMaxX >= minX && otherMaxX <= maxX);
        return (otherMinX < this.maxX && otherMaxX > this.minX);
    }

    public bool isFullyInside(Bounds bounds, Vector2 pos) {
        float otherMinX = pos.x - bounds.size.x / 2;
        float otherMaxX = pos.x + bounds.size.x / 2;
        float otherMinY = pos.y - bounds.size.y / 2;
        float otherMaxY = pos.y + bounds.size.y / 2;

        return (otherMinX >= this.minX && otherMaxX <= this.maxX) && (otherMinY >= this.minY && otherMaxY <= this.maxY);
    }

    public float[] distanceToDeathCollider(Bounds bounds) {
        Transform deathTransform = gameObject.transform.GetChild(0);
        Vector2 deathPos = deathTransform.position;
        Bounds deathBounds = deathTransform.GetComponent<Collider2D>().bounds;

        float distanceX = Mathf.Max(0, Mathf.Abs(bounds.center.x - deathPos.x) - (bounds.size.x / 2 + deathBounds.size.x / 2));
        float distanceY = Mathf.Max(0, Mathf.Abs(bounds.center.y - deathPos.y) - (bounds.size.y / 2 + deathBounds.size.y / 2));

        float maxDistanceX = (maxX - minX) / 2;
        float maxDistanceY = (maxY - minY) / 2;

        float normalizedDistanceX = 1 - Mathf.Clamp01(distanceX / maxDistanceX);
        float normalizedDistanceY = 1 - Mathf.Clamp01(distanceY / maxDistanceY);

        float distanceNegX = Mathf.Max(0, Mathf.Abs(bounds.center.x - deathPos.x) - (bounds.size.x / 2 + deathBounds.size.x / 2));
        float distanceNegY = Mathf.Max(0, Mathf.Abs(bounds.center.y - deathPos.y) - (bounds.size.y / 2 + deathBounds.size.y / 2));

        float normalizedDistanceNegX = 1 - Mathf.Clamp01(distanceNegX / maxDistanceX);
        float normalizedDistanceNegY = 1 - Mathf.Clamp01(distanceNegY / maxDistanceY);

        return new float[] { normalizedDistanceX, normalizedDistanceY, normalizedDistanceNegX, normalizedDistanceNegY };
    }
    
}