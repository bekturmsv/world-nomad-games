using UnityEngine;

public class Animal : MonoBehaviour
{
    public string animalName = "Олень";

    public GameObject aliveVisual;
    public GameObject carcassVisual;
    public GameObject scanMarker;

    public bool IsAlive { get; private set; } = true;
    public bool IsScanned { get; private set; }

    private bool meatCollected;
    public bool CanCollectMeat => !IsAlive && !meatCollected;
    public string DisplayName => animalName;

    private void Awake()
    {
        IsAlive = true;
        IsScanned = false;

        if (aliveVisual != null) aliveVisual.SetActive(true);
        if (carcassVisual != null) carcassVisual.SetActive(false);
        if (scanMarker != null) scanMarker.SetActive(false);
    }

    public void SetScanned(bool value)
    {
        IsScanned = value && IsAlive;
        if (scanMarker != null) scanMarker.SetActive(IsScanned);
    }

    public void Kill()
    {
        if (!IsAlive) return;

        IsAlive = false;
        IsScanned = false;

        if (aliveVisual != null) aliveVisual.SetActive(false);
        if (carcassVisual != null) carcassVisual.SetActive(true);
        if (scanMarker != null) scanMarker.SetActive(false);
    }

    public bool TryCollectMeat()
    {
        if (!CanCollectMeat) return false;
        meatCollected = true;

        if (carcassVisual != null) carcassVisual.SetActive(false);
        return true;
    }
}