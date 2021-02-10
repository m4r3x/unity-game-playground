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
    public float maxLifeTime = 3f;
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
    // Position of bullet in last update
    Vector3 m_LastRootPosition;
    public Vector3 initialPosition { get; private set; }
    public Vector3 initialDirection { get; private set; }
    public float initialCharge { get; private set; }
    public UnityAction onShoot;
    Vector3 m_Velocity;
    List<Collider> m_IgnoredColliders;
    private PlayerWeaponsManager playerWeaponsManager;
    const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;
    
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
        {
            onShoot.Invoke();
        }
    }
    
    void OnShoot()
    {
        m_LastRootPosition = transform.position;
        m_Velocity = transform.forward * speed;
        
        m_IgnoredColliders = new List<Collider>();
        
        // Ignore colliders of owner (prevent self shooting)
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();
        m_IgnoredColliders.AddRange(ownerColliders);

    }
    
    void Update()
    {
        // Move
        // We ignore Gravity for simplicity.
        transform.position += m_Velocity * Time.deltaTime;
  
        // Hit detection
        Vector3 displacementSinceLastFrame = tip.position - m_LastRootPosition;
        RaycastHit[] hits = Physics.SphereCastAll(m_LastRootPosition, radius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, hittableLayers, k_TriggerInteraction);
        foreach (var hit in hits)
        {
            if (IsHitValid(hit)) OnHit(hit.point, hit.normal, hit.collider);
        }
        m_LastRootPosition = transform.position;
    }
    
    bool IsHitValid(RaycastHit hit)
    {
        // ignore hits with specific ignored colliders (self colliders, trees etc)
        if (m_IgnoredColliders != null && m_IgnoredColliders.Contains(hit.collider))
        {
            return false;
        }
        
        return true;
    }

    void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        // inflict damage if target can recieve one
        ActorDamageable damageable = collider.GetComponent<ActorDamageable>();
        if (damageable)
        {
            damageable.InflictDamage(damage, owner);
        }
        
        // Self Destruct
        Destroy(this.gameObject);
    }
}
