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
    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
    [Range(0f, 1f)]
    public UnityAction onShoot;
    public GameObject owner { get; set; }
    // GameObject of attached weapon. Coming from PlayerWeaponManager.
    public GameObject sourcePrefab { get; set; }
    float lastTimeShot = Mathf.NegativeInfinity;
    const float ShotsInterval = 1.5f;

    public bool HandleShootInputs(bool inputDown)
    {
        if (inputDown) return TryShoot();

        return false;
    }
    
    bool TryShoot()
    {
        if (lastTimeShot + ShotsInterval < Time.time)
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
        lastTimeShot = Time.time;
        if (onShoot != null) onShoot();
    }
    
    public Vector3 GetShotDirection(Transform shootTransform)
    {
        return Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, 0);
    }
}
