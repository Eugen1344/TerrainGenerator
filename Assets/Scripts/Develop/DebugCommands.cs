using UnityEngine;

namespace Develop
{
    public class DebugCommands : MonoBehaviour
    {
        public Collider PlayerCollider;

        public bool NoClip
        {
            get => PlayerCollider.enabled;
            set => PlayerCollider.enabled = value;
        }

        private void Awake()
        {
            InitCommandBar();
        }

        private void InitCommandBar()
        {
            //Commands.AddCheckbox("NoClip", () => NoClip, val => NoClip = val);
        }
    }
}