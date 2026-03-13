using AutoStrike.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(TargetData))]
    [Name("AutoStrike/Actions/RotateToTarget")]
    public sealed class RotateToTargetAction : EntityMonoBehaviourAction
    {
        [SerializeField]
        private float _interpolationSpeed = 12f;

        private CompositeDisposable _subscriptions = new();
        private TargetData _targetData;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _targetData = Entity.GetData<TargetData>();

            _targetData.Target
                .Select(target =>
                    target != null
                        ? Observable.EveryUpdate()
                        : Observable.Empty<long>())
                .Switch()
                .Subscribe(_ => RotateStep())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        private void RotateStep()
        {
            EntityMonoBehaviour target = _targetData.Target.Value;
            if (target == null)
            {
                return;
            }

            Transform transform = EntityMonoBehaviour.transform;
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            float speed = Mathf.Max(0f, _interpolationSpeed);
            float t = 1f - Mathf.Exp(-speed * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }
    }
}
