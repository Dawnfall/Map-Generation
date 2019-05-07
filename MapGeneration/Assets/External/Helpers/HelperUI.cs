using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace helper
{
    public static class HelperUI
    {
        public static void addMethodToButton(string buttonName, UnityAction action)
        {
            Button button = HelperUnity.findComponentOnGameObject<Button>(buttonName);

            if (button != null)
                button.onClick.AddListener(action);
        }

        public static void addMethodToButton(string buttonName, GameObject parent, UnityAction action)
        {
            Button button = HelperUnity.findComponentOnChild<Button>(buttonName, parent);

            if (button != null)
                button.onClick.AddListener(action);
        }

        public static void setCanvasGroup(CanvasGroup canvasGroup, float alpha, bool isInteractable, bool doBlockRaycasts, bool doIgnoreParentGroups)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = doBlockRaycasts;
            canvasGroup.ignoreParentGroups = doIgnoreParentGroups;
        }
    }
}