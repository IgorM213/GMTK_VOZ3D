using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainManager : MonoBehaviour
{
    public static TrainManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject vagonPrefab;
    [SerializeField] private GameObject turretPrefab1;
    [SerializeField] private GameObject turretPrefab2;
    [SerializeField] SplineContainer sineController;
    [SerializeField] GameObject glavaVoza;

    [Header("Turrets on Train")]
    [SerializeField] private List<Turret> turrets = new List<Turret>();

    [System.Serializable]
    public class TrainPart
    {
        public GameObject cartObject; // The cart GameObject (vagon)
        public Turret[] turrets = new Turret[2]; // Each cart has 2 turrets
        public Vagon vagonScript;
    }

    [Header("Train Parts")]
    [SerializeField] private List<TrainPart> trainParts = new List<TrainPart>();

    private int activeTurrets = 0;
    private int activeVagons = 0;
    [SerializeField] float vagonOffsetStep = .4f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Clear any existing train parts
        foreach (var part in trainParts)
        {
            if (part.cartObject != null)
                Destroy(part.cartObject);
        }
        trainParts.Clear();
        turrets.Clear();
        activeTurrets = 0;
        activeVagons = 0;

        // Instantiate first vagon and first turret
        AddNewVagonWithTurret(turretPrefab1); // Start with the regular turret
    }

    // Instantiates a new vagon and places a turret at position 1
    private void AddNewVagonWithTurret(GameObject turretPrefab)
    {
        float offset = (activeVagons+1) * vagonOffsetStep;
        if(activeVagons == 0)
        {
            offset = .16f;
        }

        // Instantiate vagon
        GameObject vagonObj = Instantiate(vagonPrefab, Vector3.zero, Quaternion.identity, transform);
        Vagon vagonScript = vagonObj.GetComponent<Vagon>();
        vagonScript.OffsetDistance = offset;
        vagonScript.splineContainer = sineController; // Assuming sineController has a SplineContainer reference
        vagonScript.glavaVozaObj = glavaVoza;

        Transform pos1 = vagonScript.pos1;
        GameObject turretObj = Instantiate(turretPrefab, pos1 != null ? pos1.position : vagonObj.transform.position, pos1 != null ? pos1.rotation : Quaternion.identity, vagonObj.transform);
        Turret turretScript = turretObj.GetComponent<Turret>();

        TrainPart part = new TrainPart
        {
            cartObject = vagonObj,
            turrets = new Turret[2],
            vagonScript = vagonScript
        };
        part.turrets[0] = turretScript;
        trainParts.Add(part);

        vagonObj.SetActive(true);
        turretObj.SetActive(true);

        RegisterTurret(turretScript);

        activeVagons++;
        activeTurrets++;
    }

    public void ActivateNextTurret(int turretPrefabIndex) // 0 for turretPrefab1, 1 for turretPrefab2
    {
        if (turretPrefabIndex < 0 || turretPrefabIndex >= 2)
        {
            Debug.LogError("Invalid turret prefab index: " + turretPrefabIndex);
            return;
        }

        GameObject turretPrefab = turretPrefabIndex == 0 ? turretPrefab1 : turretPrefab2;

        // Find the next available slot for a turret
        int totalTurrets = trainParts.Count * 2;
        if (activeTurrets < totalTurrets)
        {
            // Try to fill the second slot in the last vagon first
            TrainPart lastPart = trainParts[trainParts.Count - 1];
            if (lastPart.turrets[1] == null)
            {
                Transform pos2 = lastPart.vagonScript.pos2;
                GameObject turretObj = Instantiate(turretPrefab, pos2 != null ? pos2.position : lastPart.cartObject.transform.position, pos2 != null ? pos2.rotation : Quaternion.identity, lastPart.cartObject.transform);
                Turret turretScript = turretObj.GetComponent<Turret>();
                lastPart.turrets[1] = turretScript;
                turretObj.SetActive(true);
                RegisterTurret(turretScript);
                activeTurrets++;
                return;
            }
        }

        // If all slots are filled, add a new vagon and place a turret at position 1
        AddNewVagonWithTurret(turretPrefab);
    }

    public void RegisterTurret(Turret turret)
    {
        if (!turrets.Contains(turret))
            turrets.Add(turret);
    }

    public void UnregisterTurret(Turret turret)
    {
        if (turrets.Contains(turret))
            turrets.Remove(turret);
    }

    public void UpgradeTurretDamage(float amount)
    {
        foreach (var turret in turrets)
        {
            turret.UpgradeDamage(amount);
        }
    }

    public void UpgradeTurretFireRate(float amount)
    {
        foreach (var turret in turrets)
        {
            turret.UpgradeFireRate(amount);
        }
    }

    public void UpgradeTurretRadius(float amount)
    {
        foreach (var turret in turrets)
        {
            turret.UpgradeRadius(amount);
        }
    }
}