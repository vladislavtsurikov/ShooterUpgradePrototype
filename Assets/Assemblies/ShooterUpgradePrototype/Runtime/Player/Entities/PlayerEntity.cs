using System;
using AutoStrike.Actions;
using AutoStrike.FirstPersonCamera.Actions;
using AutoStrike.FirstPersonCamera.Data;
using AutoStrike.Input.Actions;
using AutoStrike.Input.Data;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace ShooterUpgradePrototype.Player.Entities
{
    public sealed class PlayerEntity : EntityMonoBehaviour
    {
        [SerializeField] private StatsEntityConfig _statsConfig;

        protected override Type[] ComponentDataTypesToCreate() =>
            new[]
            {
                typeof(StatsEntityData),
                typeof(FirstPersonCameraRigData),
                typeof(MoveInputData),
                typeof(LookInputData),
                typeof(FireInputData),
            };

        protected override Type[] ActionTypesToCreate() =>
            new[]
            {
                typeof(ApplyStatLevelsByTableAction),
                typeof(ReadMoveInputAction),
                typeof(ReadLookInputAction),
                typeof(ReadFireInputAction),
                typeof(MoveByInputAction),
                typeof(DesktopFirstPersonCameraLookAction),
                typeof(AttackTargetAction)
            };

        protected override void OnAfterCreateDataAndActions()
        {
            base.OnAfterCreateDataAndActions();

            FirstPersonCameraRigData cameraRigData = GetData<FirstPersonCameraRigData>();
            cameraRigData?.Initialize(ResolveCamera(), transform, ResolvePitchTransform());

            StatsEntityData stats = GetData<StatsEntityData>();
            if (stats == null || _statsConfig == null)
            {
                return;
            }

            stats.SourceType = StatsEntitySourceType.Global;
            stats.GlobalConfig = _statsConfig;
            stats.RebuildFromCollection();

            ApplyStatLevelsByTableAction applyLevelsAction = GetAction<ApplyStatLevelsByTableAction>();
            applyLevelsAction?.ApplyLevels();
        }

        private Camera ResolveCamera() => GetComponentInChildren<Camera>(true);

        private Transform ResolvePitchTransform()
        {
            Camera camera = ResolveCamera();
            if (camera == null)
            {
                return transform;
            }

            return camera.transform.parent != null
                ? camera.transform.parent
                : camera.transform;
        }
    }
}
