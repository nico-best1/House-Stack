using UnityEngine;

public class CeilingComponent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HouseComponent house = other.GetComponentInParent<HouseComponent>();
        if (house != null && !house.isLaunched)
            return;

        GameManager.Instance.NotifyCeilingContactStart();
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.NotifyCeilingContactEnd();
    }
}
