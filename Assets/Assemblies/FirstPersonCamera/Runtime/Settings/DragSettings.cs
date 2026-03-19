using System;
using UnityEngine;

namespace AutoStrike.FirstPersonCamera.FirstPersonCamera.Runtime
{
    [Serializable]
    public sealed class DragSettings
    {
        [SerializeField]
        private Vector2 _sensitivity = new(0.12f, 0.12f);

        public Vector2 Sensitivity => _sensitivity;
    }
}
