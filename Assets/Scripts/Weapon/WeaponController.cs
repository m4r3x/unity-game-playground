using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class WeaponController : MonoBehaviour
{
    [Header("Bullet options")]
    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform weaponMuzzle;
    
    [Header("Shoot Parameters")]
    [Tooltip("The projectile prefab")]
    public WeaponBullet bulletPrefab;
    [Tooltip("Minimum duration between two shots")]
    public float delayBetweenShots = 5f;
    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
    [Range(0f, 1f)]
    public float aimZoomRatio = 1f;
    public UnityAction onShoot;
    public event Action OnShootProcessed;
    public GameObject owner { get; set; }
    // sourcePrefab of given weapon
    public GameObject sourcePrefab { get; set; }
    float m_LastTimeShot = Mathf.NegativeInfinity;

    public bool HandleShootInputs(bool inputDown)
    {
        if (inputDown)
        {
            return TryShoot();
        }
        return false;
    }
    
    bool TryShoot()
    {
        if (m_LastTimeShot + delayBetweenShots < Time.time)
        {
            HandleShoot();

            return true;
        }

        return false;
    }
    
    void HandleShoot()
    {
        Vector3 shotDirection = GetShotDirection(weaponMuzzle);
        WeaponBullet newBullet = Instantiate(bulletPrefab, weaponMuzzle.position, Quaternion.LookRotation(shotDirection));
        newBullet.Shoot(this);
        m_LastTimeShot = Time.time;

        // Callback on shoot
        if (onShoot != null)
        {
            onShoot();
        }

        OnShootProcessed?.Invoke();
    }
    
    public Vector3 GetShotDirection(Transform shootTransform)
    {
        return Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, 0);
    }
}
