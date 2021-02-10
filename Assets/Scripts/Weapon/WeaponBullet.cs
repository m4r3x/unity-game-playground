using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponBullet : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Radius of this projectile's collision detection")]
    public float radius = 0.01f;
    [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
    public float maxLifeTime = 2.5f;
    [Tooltip("Layers this projectile can collide with")]
    public LayerMask hittableLayers = -1;
    [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
    public Transform tip;

    [Header("Movement")]
    [Tooltip("Speed of the projectile")]
    public float speed = 50f;
    
    [Header("Damage")]
    [Tooltip("Damage of the projectile")]
    public float damage = 100f;
    public GameObject owner { get; private set; }
    public Vector3 initialPosition { get; private set; }
    public Vector3 initialDirection { get; private set; }
    public UnityAction onShoot;
    Vector3 lastBulletPosition;
    Vector3 bulletVelocity;
    List<Collider> bulletIgnoredColliders;
    PlayerWeaponsManager playerWeaponsManager;
    
    private void OnEnable()
    {
        onShoot += OnShoot;
        Destroy(gameObject, maxLifeTime);
    }
    
    public void Shoot(WeaponController controller)
    {
        playerWeaponsManager = controller.owner.GetComponent<PlayerWeaponsManager>();
        owner = controller.owner;
        initialPosition = playerWeaponsManager.weaponCamera.transform.position;
        initialDirection = transform.forward;

        if (onShoot != null)
            onShoot.Invoke();
    }
    
    void OnShoot()
    {
        lastBulletPosition = transform.position;
        bulletVelocity = transform.forward * speed;
        
        // Ignore colliders of owner (prevent shooting same team or trees/terrain)
        bulletIgnoredColliders = new List<Collider>();
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();
        bulletIgnoredColliders.AddRange(ownerColliders);
    }
    
    void Update()
    {
        // We ignore Gravity for simplicity.
        transform.position += bulletVelocity * Time.deltaTime;
  
        // Hit detection
        Vector3 displacementSinceLastFrame = tip.position - lastBulletPosition;
        RaycastHit[] hits = Physics.SphereCastAll(lastBulletPosition, radius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, hittableLayers, QueryTriggerInteraction.Collide);
        foreach (var hit in hits)
        {
            if (IsHitValid(hit)) OnHit(hit.point, hit.normal, hit.collider);
        }
        lastBulletPosition = transform.position;
    }
    
    bool IsHitValid(RaycastHit hit)
    {
        // ignore hits with specific ignored colliders (self colliders, trees etc)
        if (bulletIgnoredColliders != null && bulletIgnoredColliders.Contains(hit.collider))
            return false;
        
        return true;
    }

    void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        ActorDamageable damageable = collider.GetComponent<ActorDamageable>();
        if (damageable)
            damageable.InflictDamage(damage, owner);
        
        Destroy(this.gameObject);
    }
}
