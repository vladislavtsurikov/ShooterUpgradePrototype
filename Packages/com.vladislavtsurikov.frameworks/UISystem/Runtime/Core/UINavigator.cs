using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UINavigator
    {
        public static async UniTask Show<T>(CancellationToken ct = default)
            where T : UIHandler =>
            await Handle<T>(true, ct);

        public static async UniTask Hide<T>(CancellationToken ct = default)
            where T : UIHandler =>
            await Handle<T>(false, ct);

        private static async UniTask Handle<T>(bool isShow, CancellationToken ct)
            where T : UIHandler
        {
            Type parentType = ResolveParentType<T>();

            T handler = parentType == null
                ? UIHandlerResolver.FindHandler<T>()
                : UIHandlerResolver.FindHandler<T>(parentType);

            if (handler == null)
            {
                Debug.LogError(
                    $"[UINavigator] Cannot {(isShow ? "Show" : "Hide")}. UIHandler of type {typeof(T).Name}" +
                    (parentType != null ? $" with parent {parentType.Name}" : "") + " not found.");
                return;
            }

            if (isShow)
            {
                await handler.Show(ct);
            }
            else
            {
                await handler.Hide(ct);
            }
        }

        private static Type ResolveParentType<T>() where T : UIHandler
        {
            var attribute = (UIParentAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(UIParentAttribute));
            return attribute?.ParentType;
        }
    }
}
