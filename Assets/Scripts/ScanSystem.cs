using System.Collections.Generic;
using UnityEngine;

public class ScanSystem : MonoBehaviour
{
    public Transform viewCamera;
    public float scanRadius = 25f;
    public float targetRange = 40f;

    public LayerMask animalLayers = ~0;

    public bool IsScanning { get; private set; }
    public Animal CurrentTarget { get; private set; }

    private readonly HashSet<Animal> scannedAnimals = new HashSet<Animal>();
    private readonly HashSet<Animal> animalsFoundThisFrame = new HashSet<Animal>();

    private void Update()
    {
        if (viewCamera == null) return;

        IsScanning = Input.GetKey(KeyCode.E);
        animalsFoundThisFrame.Clear();

        if (IsScanning)
        {
            Collider[] hits = Physics.OverlapSphere(viewCamera.position, scanRadius, animalLayers, QueryTriggerInteraction.Ignore);

            foreach (Collider hit in hits)
            {
                Animal animal = hit.GetComponentInParent<Animal>();

                if (animal == null || !animal.IsAlive) continue;
                if (HasClearLineOfSight(animal)) animalsFoundThisFrame.Add(animal);
            }
        }

        foreach (Animal animal in scannedAnimals)
        {
            if (animal != null && !animalsFoundThisFrame.Contains(animal)) animal.SetScanned(false);
        }

        foreach (Animal animal in animalsFoundThisFrame)
        {
            if (animal != null) animal.SetScanned(true);
        }

        scannedAnimals.Clear();

        foreach (Animal animal in animalsFoundThisFrame)
        {
            if (animal != null) scannedAnimals.Add(animal);
        }

        CurrentTarget = null;
        if (!IsScanning) return;

        if (Physics.Raycast(viewCamera.position, viewCamera.forward, out RaycastHit targetHit, targetRange, ~0, QueryTriggerInteraction.Ignore))
        {
            Animal candidate = targetHit.collider.GetComponentInParent<Animal>();
            if (candidate != null && candidate.IsAlive && candidate.IsScanned) CurrentTarget = candidate;
        }
    }

    private bool HasClearLineOfSight(Animal animal)
    {
        Vector3 direction = animal.transform.position - viewCamera.position;
        float distance = direction.magnitude;

        if (distance < 0.1f) return true;

        if (Physics.Raycast(viewCamera.position, direction.normalized, out RaycastHit hit, distance + 0.5f, ~0, QueryTriggerInteraction.Ignore))
        {
            Animal hitAnimal = hit.collider.GetComponentInParent<Animal>();
            return hitAnimal == animal;
        }

        return false;
    }
}