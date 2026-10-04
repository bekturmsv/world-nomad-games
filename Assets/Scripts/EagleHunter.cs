using System.Collections;
using UnityEngine;

public class EagleHunter : MonoBehaviour
{
    public Transform eagleRoot;
    public Transform eagleEye;
    public Transform eagleRestPoint;

    public Transform playerCamera;
    public ScanSystem scanSystem;
    public FpsPlayerController playerController;

    public float flightSpeed = 12f;
    public float cameraBlendTime = 0.4f;

    public bool IsFlying { get; private set; }

    private void Awake()
    {
        if (scanSystem == null) scanSystem = GetComponent<ScanSystem>();

        if (playerController == null) playerController = GetComponent<FpsPlayerController>();

        if (playerCamera == null && scanSystem != null) playerCamera = scanSystem.viewCamera;
    }

    private void Update()
    {
        if (IsFlying || scanSystem == null || scanSystem.CurrentTarget == null) return;
        if (eagleRoot == null || eagleEye == null || eagleRestPoint == null || playerCamera == null) return;

        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(AttackTarget(scanSystem.CurrentTarget));
    }

    private IEnumerator AttackTarget(Animal target)
    {
        if (target == null) yield break;

        IsFlying = true;

        Transform originalCameraParent = playerCamera.parent;
        Vector3 originalCameraLocalPosition = playerCamera.localPosition;
        Quaternion originalCameraLocalRotation = playerCamera.localRotation;

        Vector3 originalCameraWorldPosition = playerCamera.position;
        Quaternion originalCameraWorldRotation = playerCamera.rotation;

        Vector3 eagleRestPosition = eagleRestPoint.position;
        Quaternion eagleRestRotation = eagleRestPoint.rotation;

        // Player does not move the camera, while eagle hunting
        if (playerController != null) playerController.enabled = false;

        eagleRoot.SetParent(null, true);

        // POV from player to eagle
        yield return BlendCameraToEagle(playerCamera, eagleEye, cameraBlendTime);

        // Hunting
        while (target != null && target.IsAlive)
        {
            Vector3 destination = target.transform.position + Vector3.up * 0.7f;
            Vector3 direction = destination - eagleRoot.position;

            if (direction.magnitude <= 0.45f) break;

            eagleRoot.position = Vector3.MoveTowards(eagleRoot.position, destination, flightSpeed * Time.deltaTime);

            if (direction.sqrMagnitude > 0.001f)
            {
                eagleRoot.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }

            yield return null;
        }

        // Camera on EagleEye, follows after eagle
        if (target != null && target.IsAlive)
        {
            target.Kill();
            yield return new WaitForSeconds(0.5f);
        }

        // Return eagle on the hand
        yield return FlyBackTo(eagleRestPosition, eagleRestRotation);

        // POV From eagle to player
        playerCamera.SetParent(null, true);

        yield return BlendCameraToPlayer(playerCamera, originalCameraWorldPosition, originalCameraWorldRotation, cameraBlendTime);

        playerCamera.SetParent(originalCameraParent, false);
        playerCamera.localPosition = originalCameraLocalPosition;
        playerCamera.localRotation = originalCameraLocalRotation;

        // Eagle on the hand
        eagleRoot.SetParent(eagleRestPoint, false);
        eagleRoot.localPosition = Vector3.zero;
        eagleRoot.localRotation = Quaternion.identity;

        if (playerController != null) playerController.enabled = true;

        IsFlying = false;
    }

    private IEnumerator BlendCameraToEagle(
        Transform cameraTransform,
        Transform eagleView,
        float duration)
    {
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            cameraTransform.position = Vector3.Lerp(startPosition, eagleView.position, t);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, eagleView.rotation, t);

            yield return null;
        }

        cameraTransform.SetParent(eagleView, false);
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;
    }

    private IEnumerator FlyBackTo(
        Vector3 destination,
        Quaternion destinationRotation)
    {
        while (Vector3.Distance(eagleRoot.position, destination) > 0.12f)
        {
            Vector3 direction = destination - eagleRoot.position;

            eagleRoot.position = Vector3.MoveTowards(eagleRoot.position, destination, flightSpeed * Time.deltaTime);

            if (direction.sqrMagnitude > 0.001f)
            {
                eagleRoot.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }

            yield return null;
        }

        eagleRoot.position = destination;
        eagleRoot.rotation = destinationRotation;
    }

    private IEnumerator BlendCameraToPlayer(
        Transform cameraTransform,
        Vector3 destinationPosition,
        Quaternion destinationRotation,
        float duration)
    {
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            cameraTransform.position = Vector3.Lerp(startPosition, destinationPosition, t);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, destinationRotation, t);
            yield return null;
        }

        cameraTransform.position = destinationPosition;
        cameraTransform.rotation = destinationRotation;
    }
}