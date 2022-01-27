using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerIntent : MonoBehaviour
    {
        [SerializeField]
        Character m_SelectedCharacter;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.action.WasPerformedThisFrame())
            {
                return;
            }

            var direction = context.action.ReadValue<Vector2>();
            var intention = new Character.Intention(Character.Intent.Move, direction);
            m_SelectedCharacter.ReceiveIntent(intention);
        }
    }
}
