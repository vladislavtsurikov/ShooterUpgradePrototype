using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;

namespace AutoStrike.FirstPersonCamera.Data
{
    public sealed class FirstPersonCameraRigData : ComponentData
    {
        public Camera Camera { get; private set; }
        public Transform YawTransform { get; private set; }
        public Transform PitchTransform { get; private set; }

        public void Initialize(Camera camera, Transform yawTransform, Transform pitchTransform)
        {
            Camera = camera;
            YawTransform = yawTransform;
            PitchTransform = pitchTransform;
        }
    }
}
