using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    public class InventorySlotTextUpdater : MonoBehaviour
    {
        public TextMeshProUGUI currentCount;
        public TextMeshProUGUI maxCount;

        private InventorySlot inventorySlot;


        private void SetText(string currentValue, string maxValue)
        {
            currentCount.text = currentValue;
            maxCount.text = "/" + maxValue;
        }

        private void HideText()
        {
            currentCount.text = "";
            maxCount.text = "";
        }

        private void SetTextToInfinity()
        {
            currentCount.text = "";
            maxCount.text = "∞";
        }
    }
}