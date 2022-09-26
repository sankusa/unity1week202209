using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sankusa.unity1week202209.View;
using Sankusa.unity1week202209.BattleScene.GameLoop;
using Sankusa.unity1week202209.BattleScene.Domain;
using Sankusa.unity1week202209.BattleScene.View;

namespace Sankusa.unity1week202209.BattleScene.Installer {
    public class BattleSceneInstaller : MonoInstaller
    {
        [SerializeField] private SquareStructureConverter structureConverter;
        [SerializeField] private ResultPerformer resultPerformer;
        [SerializeField] private BattleStatusView battleStatusView;

        public override void InstallBindings()
        {
            // View
            Container.BindInterfacesAndSelfTo<SquareStructureConverter>()
                     .FromInstance(structureConverter)
                     .AsSingle();

            // BattleScene.Domain
            Container.BindInterfacesAndSelfTo<BattleStatus>()
                     .AsSingle()
                     .NonLazy();

            // BattleScene.View
            Container.BindInterfacesAndSelfTo<ResultPerformer>()
                     .FromInstance(resultPerformer)
                     .AsSingle();

            Container.BindInterfacesAndSelfTo<BattleStatusView>()
                     .FromInstance(battleStatusView)
                     .AsSingle();

            // BattleScene.GameLoop
            Container.BindInterfacesAndSelfTo<BattleSceneGameLoop>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}