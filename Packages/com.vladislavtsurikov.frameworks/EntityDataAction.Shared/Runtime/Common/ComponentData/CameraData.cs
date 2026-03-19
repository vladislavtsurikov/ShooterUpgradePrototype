using System.Linq;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;

namespace AutoStrike.FirstPersonCamera.Data
{
    public sealed class CameraData : ComponentData
    {
        public Camera Camera { get; private set; }

        protected override void OnFirstSetupComponent(object[] setupData = null)
        {
            EntityMonoBehaviour entityMonoBehaviour = setupData?
                .OfType<EntityMonoBehaviour>()
                .FirstOrDefault();

            Camera = entityMonoBehaviour != null
                ? entityMonoBehaviour.GetComponentInChildren<Camera>(true)
                : null;
        }
    }
}
