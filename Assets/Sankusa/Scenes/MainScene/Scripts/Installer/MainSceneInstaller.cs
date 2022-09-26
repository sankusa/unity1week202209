using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sankusa.unity1week202209.MainScene.View;
using Sankusa.unity1week202209.MainScene.GameLoop;

namespace Sankusa.unity1week202209.MainScene.Installer {
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private RankingView rankingView;
        [SerializeField] private BattleHistoryView battleHistoryView;
        [SerializeField] private SquareStructureEditView structureEditView;
        [SerializeField] private SquareStructureDisplayView structureDisplayView;

        public override void InstallBindings() {
            // MainScene.View
            Container.BindInterfacesAndSelfTo<BattleHistoryView>()
                     .FromInstance(battleHistoryView)
                     .AsSingle();

            Container.BindInterfacesAndSelfTo<RankingView>()
                     .FromInstance(rankingView)
                     .AsSingle();

            Container.BindInterfacesAndSelfTo<SquareStructureEditView>()
                     .FromInstance(structureEditView)
                     .AsSingle();

            Container.BindInterfacesAndSelfTo<SquareStructureDisplayView>()
                     .FromInstance(structureDisplayView)
                     .AsSingle();

            // MainScne.GameLoop
            Container.BindInterfacesAndSelfTo<MainSceneGameLoop>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}