using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation
{
    public class Fade : SceneOperation
    {
        public CanvasGroup Group;
        public Image Image;
        public float Seconds = 1f;
        public Color Color = Color.black;

        public override async UniTask OnLoad(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Image.color = Color;
            Group.alpha = 0;
            await Group.Fade(1, Seconds);
        }

        public override async UniTask OnUnload(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Group.alpha = 1;
            await Group.Fade(0, Seconds);
        }

        // private void Start()
        // {
        //     Group.alpha = 0;
        //
        //     OnLoad().Forget();
        // }
    }
}