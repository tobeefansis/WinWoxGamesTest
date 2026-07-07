using TMPro;
using UnityEngine;

public class DiamondCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void SetCount(int collected, int total)
        => label.text = $"Diamonds: {collected} / {total}";

    public void SetVisible(bool visible)
        => gameObject.SetActive(visible);
}
