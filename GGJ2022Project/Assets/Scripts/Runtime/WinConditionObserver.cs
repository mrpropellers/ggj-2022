using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.GlobalConsts;
using UnityEngine;

namespace GGJ
{
    public class WinConditionObserver : MonoBehaviour
    {
        List<Item> m_AllIngredients;

        [SerializeField]
        GameObject m_LevelFailPrefab;

        [SerializeField]
        GameObject m_LevelSucceedPrefab;

        void Start()
        {
            var allItems = FindObjectsOfType<Item>();
            m_AllIngredients = new List<Item>(allItems.Where(item => item.CompareTag(Tags.Ingredient)));
            var allOvens = FindObjectsOfType<Oven>();
            foreach (var oven in allOvens)
            {
                oven.GetComponent<BoardPiece>().OnInteractedWith.AddListener(CheckForWin);
            }

            var allAi = FindObjectsOfType<IntentionProvider>();
            foreach (var ai in allAi)
            {
                if (ai.TryGetComponent<Inventory>(out var inventory))
                {
                    inventory.OnItemAdd.AddListener(CheckForLoss);
                }
            }

            StageState.Instance.OnLevelEndResolved.AddListener(InstantiateLevelEndPrefab);
        }

        void InstantiateLevelEndPrefab(StageState.LevelEndResult result)
        {
            switch (result)
            {
                case StageState.LevelEndResult.Failure:
                    Instantiate(m_LevelFailPrefab);
                    break;
                case StageState.LevelEndResult.Success:
                    Instantiate(m_LevelSucceedPrefab);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"No handling for {result}");
            }
        }

        void CheckForWin()
        {
            var stageState = StageState.Instance;
            var playerInventory = stageState.PlayerCharacter.GetComponent<Inventory>();
            foreach (var ingredient in m_AllIngredients)
            {
                if (!playerInventory.IsHolding(ingredient))
                {
                    Debug.Log($"{playerInventory.name} is missing ingredients!");
                    stageState.OnLevelIncomplete?.Invoke();
                    return;
                }
            }
            Debug.Log($"{playerInventory.name} has all the ingredients!");
            stageState.OnLevelSuccess?.Invoke();
        }

        void CheckForLoss(Item item)
        {
            if (m_AllIngredients.Contains(item))
            {
                Debug.Log($"An enemy picked up {item.name}! Player loses.");
                StageState.Instance.OnLevelFailure?.Invoke();
            }
        }
    }
}
