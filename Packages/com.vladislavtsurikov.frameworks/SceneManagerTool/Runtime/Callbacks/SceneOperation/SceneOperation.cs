using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation
{
    public abstract class SceneOperation : MonoBehaviour
    {
        public abstract UniTask OnLoad(CancellationToken token);
        public abstract UniTask OnUnload(CancellationToken token);
    }
}