using System;
using UnityEngine;

namespace AutoStrike.FirstPersonCamera.Settings
{
    [Serializable]
    public sealed class StickSettings
    {
        [SerializeField]
        private Vector2 _sensitivity = new(180f, 180f);

        public Vector2 Sensitivity => _sensitivity;
    }
}
