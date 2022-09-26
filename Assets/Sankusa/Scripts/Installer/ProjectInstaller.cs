using UnityEngine;
using Zenject;
using Sankusa.unity1week202209.Domain;
using Sankusa.unity1week202209.DataAccess;
using SankusaLib.SceneManagementLib;

namespace Sankusa.unity1week202209.Installer {
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // 外部(自作ライブラリ)
            // シーン間データ保存用クラス
            Container.BindInterfacesAndSelfTo<SceneArgStore>()
                     .AsSingle()
                     .NonLazy();
            
            // Domain
            Container.BindInterfacesAndSelfTo<PlayerInfo>()
                     .AsSingle()
                     .NonLazy();
                     
            Container.BindInterfacesAndSelfTo<SquareStructureStorage>()
                     .AsSingle()
                     .NonLazy();

            Container.BindInterfacesAndSelfTo<SaveData>()
                     .AsSingle()
                     .NonLazy();

            Container.BindInterfacesAndSelfTo<GameInitializer>()
                     .AsSingle()
                     .NonLazy();

            Container.Bind<IRankingAccessor>()
                     .To<NCMBRankingAccessor>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}