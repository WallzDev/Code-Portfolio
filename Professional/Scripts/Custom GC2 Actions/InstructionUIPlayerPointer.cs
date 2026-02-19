using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Toggle Pointer")]
    [Description("Toggles the player's pointer on or off")]

    [Category("UI/Toggle Pointer")]

    [Keywords("Activate", "Deactivate", "Enable", "Disable", "Pointer", "Cursor", "Toggle")]
    [Keywords("MonoBehaviour", "Behaviour", "Script")]

    [Image(typeof(IconCursor), ColorTheme.Type.White)] //IconCursor or IconDot

    [Serializable]
    public class InstructionUIPlayerPointer : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private bool pointerActive;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Toggle Player Pointer to {pointerActive}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            PlayerHUDManager.Instance.TogglePointer(pointerActive);
            return DefaultResult;
        }
    }
}
