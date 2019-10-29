using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBulletImpact : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactPrefab;
    [SerializeField] private Sprite[] bulletImpactAnimation;
    [SerializeField] private Vector2 impactOffset;

    public void Create(Vector3 position, Vector3 normal)
    {
        var obj = Instantiate(bulletImpactPrefab);
        var transform = obj.GetComponent<BulletImpact>().BulletRotate;
        obj.transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg);
        obj.transform.position += transform.rotation * impactOffset;
        var anim = obj.GetComponent<AnimationOnceThenDestroy>();
        anim.Animation = bulletImpactAnimation;
        anim.StartAnimation();
    }
}
