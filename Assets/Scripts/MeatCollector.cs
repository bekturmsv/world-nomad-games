using UnityEngine;

public class MeatCollector : MonoBehaviour
{
    public float pickupRadius = 2f;

    public int MeatCount { get; private set; }
    public Animal NearbyCarcass { get; private set; }

    private void Update()
    {
        NearbyCarcass = null;
        float closestDistance = float.MaxValue;

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius, ~0, QueryTriggerInteraction.Ignore);

        foreach (Collider hit in hits)
        {
            Animal animal = hit.GetComponentInParent<Animal>();

            if (animal == null || !animal.CanCollectMeat) continue;

            float distance = Vector3.Distance(transform.position, animal.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                NearbyCarcass = animal;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) &&
            NearbyCarcass != null &&
            NearbyCarcass.TryCollectMeat())
        {
            MeatCount++;
        }
    }
}