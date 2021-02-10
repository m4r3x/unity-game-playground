using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponsManager : MonoBehaviour
{
    [Tooltip("Starting weapon of player")]
    public WeaponController startingWeapon;
    [Header("References")]
    [Tooltip("Secondary camera used to avoid seeing weapon go throw geometries")]
    public Camera weaponCamera;
    [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
    public Transform weaponParentSocket;
    [Header("Misc")]
    [Tooltip("Field of view when not aiming")]
    public float defaultFOV = 90f;
    public bool isAiming { get; private set; }
    private WeaponController activeWeapon;
    private PlayerHandler playerHandler;
    private PlayerCharacterController playerCharacterController;
    private bool weaponAssigned = false;
    void Start()
    {
        playerHandler = GetComponent<PlayerHandler>();
        playerCharacterController = GetComponent<PlayerCharacterController>();
        
        DebugUtility.HandleErrorIfNullGetComponent<PlayerHandler, PlayerWeaponsManager>(playerHandler, this, gameObject);
        DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerWeaponsManager>(playerCharacterController, this, gameObject);

        SetFOV(defaultFOV);
        AddWeapon(startingWeapon);
    }
    
    void Update()
    {
        isAiming = playerHandler.GetAimInputHeld();
        activeWeapon.HandleShootInputs(playerHandler.GetFireInputDown());
    }

    void LateUpdate()
    {
        UpdateWeaponAiming();
    }
    
    void UpdateWeaponAiming()
    {
        if (isAiming)
            SetFOV(defaultFOV - 25f);
        else
            SetFOV(defaultFOV);
    }

    void SetFOV(float fov)
    {
        playerCharacterController.playerCamera.fieldOfView = fov;
        weaponCamera.fieldOfView = fov;
    }
    
    bool AddWeapon(WeaponController weaponPrefab)
    {
        if (weaponAssigned == false)
        {
            WeaponController weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.owner = gameObject;
            weaponInstance.sourcePrefab = weaponPrefab.gameObject;
            activeWeapon = weaponInstance;
            weaponAssigned = true;
        }

        return weaponAssigned;
    }
}
