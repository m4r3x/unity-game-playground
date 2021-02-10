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
    [Tooltip("Layer to set FPS weapon gameObjects to")]
    public LayerMask FPSWeaponLayer;
    public bool isAiming { get; private set; }
    private WeaponController activeWeapon;
    public UnityAction<WeaponController, int> onAddedWeapon;
    private PlayerHandler m_InputHandler;
    private PlayerCharacterController m_PlayerCharacterController;
    private bool weaponAssigned = false;
    private void Start()
    {
        m_InputHandler = GetComponent<PlayerHandler>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerHandler, PlayerWeaponsManager>(m_InputHandler, this, gameObject);

        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerWeaponsManager>(m_PlayerCharacterController, this, gameObject);

        SetFOV(defaultFOV);

        AddWeapon(startingWeapon);
    }
    
    private void Update()
    {
        // handle aiming down sights
        isAiming = m_InputHandler.GetAimInputHeld();
        
        // handle shooting
        activeWeapon.HandleShootInputs(m_InputHandler.GetFireInputDown());
    }

    // Late Update aiming correction for camera
    private void LateUpdate()
    {
        UpdateWeaponAiming();
    }
    
    void UpdateWeaponAiming()
    {
        if (isAiming)
        {
            m_PlayerCharacterController.playerCamera.fieldOfView = defaultFOV - 25f;
        }
        else
        {
            m_PlayerCharacterController.playerCamera.fieldOfView = defaultFOV;
        }
    }

    // Sets the FOV of the main camera and the weapon camera simultaneously
    public void SetFOV(float fov)
    {
        m_PlayerCharacterController.playerCamera.fieldOfView = fov;
        weaponCamera.fieldOfView = fov;
    }
    
    // Adds a weapon to our inventory
    public bool AddWeapon(WeaponController weaponPrefab)
    {
        if (weaponAssigned == false)
        {
            // spawn the weapon prefab as child of the weapon socket
            WeaponController weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;

            // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
            weaponInstance.owner = gameObject;
            weaponInstance.sourcePrefab = weaponPrefab.gameObject;

            // Assign the first person layer to the weapon
            int layerIndex = Mathf.RoundToInt(Mathf.Log(FPSWeaponLayer.value, 2)); // This function converts a layermask to a layer index
            foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = layerIndex;
            }

            if (onAddedWeapon != null)
            {
                onAddedWeapon.Invoke(weaponInstance, 0);
            }
            weaponAssigned = true;
            activeWeapon = weaponInstance;
        }

        return weaponAssigned;
    }
}
