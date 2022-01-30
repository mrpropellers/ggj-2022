using System.Collections;
using UnityEngine;

namespace GGJ.UI
{
    [RequireComponent(typeof(InventoryPanelController))]
    public class HudInventoryPanelLinker : MonoBehaviour
    {
        #region Cached Sibling Components
        private InventoryPanelController m_inventoryPanelController;
        #endregion

        #region Engine Messages
        private void Awake()
        {
            // Cache components.
            m_inventoryPanelController = GetComponent<InventoryPanelController>();
        }

        private void OnTransformParentChanged()
        {
            if (isActiveAndEnabled)
            {
                TryLinkToHud();
            }
        }

        private void OnEnable()
        {
            TryLinkToHud();
        }

        private void OnDisable()
        {
            // Clean up.
            currentHud = null;
        }
        #endregion

        #region Hud Link
        private Hud m_CurrentHud;
        private Hud currentHud
        {
            get => m_CurrentHud;
            set {
                if (ReferenceEquals(m_CurrentHud, value))
                {
                    return;
                }
                if (!ReferenceEquals(m_CurrentHud, null))
                {
                    HandleHudLost(m_CurrentHud);
                }
                m_CurrentHud = value;
                if (!ReferenceEquals(m_CurrentHud, null))
                {
                    HandleHudDiscovered(m_CurrentHud);
                }
            }
        }

        private void TryLinkToHud()
        {
            currentHud = GetComponentInParent<Hud>();
        }

        private void HandleHudDiscovered(Hud hud)
        {
            hud.OnObservedCharacterDiscovered += OnHudObservedCharacterDiscovered;
            hud.OnObservedCharacterLost += OnHudObservedCharacterLost;
            if (!ReferenceEquals(hud.ObservedCharacter, null))
            {
                OnHudObservedCharacterDiscovered(hud.ObservedCharacter);
            }
        }

        private void HandleHudLost(Hud hud)
        {
            if (!ReferenceEquals(hud.ObservedCharacter, null))
            {
                OnHudObservedCharacterLost(hud.ObservedCharacter);
            }
            hud.OnObservedCharacterLost -= OnHudObservedCharacterLost;
            hud.OnObservedCharacterDiscovered -= OnHudObservedCharacterDiscovered;
        }
        #endregion

        private void OnHudObservedCharacterDiscovered(Character character)
        {
            m_inventoryPanelController.inventory = character.GetComponent<IHoldItems>();
        }

        private void OnHudObservedCharacterLost(Character character)
        {
            m_inventoryPanelController.inventory = null;
        }
    }
}
