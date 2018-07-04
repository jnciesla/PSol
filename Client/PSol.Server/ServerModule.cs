using System;
using Ninject.Modules;
using PSol.Data.Repositories;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services;
using PSol.Data.Services.Interfaces;

namespace PSol.Server
{
    public class ServerModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IUserRepository)).To(typeof(UserRepository)).InSingletonScope();
            Bind(typeof(IUserService)).To(typeof(UserService)).InSingletonScope();
            Bind(typeof(IGameService)).To(typeof(GameService)).InSingletonScope();
            Bind(typeof(IStarRepository)).To(typeof(StarRepository)).InSingletonScope();
            Bind(typeof(IStarService)).To(typeof(StarService)).InSingletonScope();
            Bind(typeof(IMobRepository)).To(typeof(MobRepository)).InSingletonScope();
            Bind(typeof(IMobService)).To(typeof(MobService)).InSingletonScope();
            Bind(typeof(ICombatService)).To(typeof(CombatService)).InSingletonScope();
        }
    }
}
