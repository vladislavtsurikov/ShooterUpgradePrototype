using System.Threading;
using Cysharp.Threading.Tasks;
using Nody.Runtime.Core;
using UnityEngine;
using UnityEngine.Rendering;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [Name("AutoStrike/Actions/DisableDeathInteractions")]
    [Group("Death")]
    public sealed class DisableDeathInteractionsAction : EntityMonoBehaviourAction
    {
        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (EntityMonoBehaviour == null)
            {
                return UniTask.FromResult(false);
            }

            DisableColliders();
            DisableRigidbodies();
            DisableShadows();

            return UniTask.FromResult(true);
        }

        private void DisableColliders()
        {
            Collider[] colliders = EntityMonoBehaviour.GetComponentsInChildren<Collider>(true);
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider collider = colliders[index];
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        private void DisableRigidbodies()
        {
            Rigidbody[] rigidbodies = EntityMonoBehaviour.GetComponentsInChildren<Rigidbody>(true);
            for (int index = 0; index < rigidbodies.Length; index++)
            {
                Rigidbody rigidbody = rigidbodies[index];
                if (rigidbody == null)
                {
                    continue;
                }

                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = true;
            }
        }

        private void DisableShadows()
        {
            Renderer[] renderers = EntityMonoBehaviour.GetComponentsInChildren<Renderer>(true);
            for (int index = 0; index < renderers.Length; index++)
            {
                Renderer renderer = renderers[index];
                if (renderer != null)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }
            }
        }
    }
}
