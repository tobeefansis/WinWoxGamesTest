using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Diamond : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ServiceLocator.Instance.Get<GameStateMachine>()?.CollectDiamond();
        Destroy(gameObject);
    }
}
