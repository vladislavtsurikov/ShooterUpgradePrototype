using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.Nody.Runtime.Core;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    [Name("Spawn Prefabs")]
    [ParentRequired(typeof(AfterLoadOperationsSettings))]
    public class SpawnPrefabs : Action
    {
        public List<GameObject> GameObjects = new();

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            foreach (GameObject gameObject in GameObjects)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                Object.Instantiate(gameObject);
            }

            await UniTask.CompletedTask;
            return true;
        }
    }
}
