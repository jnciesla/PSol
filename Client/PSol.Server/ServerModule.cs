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
            //Bind(Type.GetType("DI.Data.DIConsoleEntities, DI.Data")).ToSelf().InSingletonScope();
            Bind(typeof(IUserRepository)).To(typeof(UserRepository)).InSingletonScope();
            Bind(typeof(IUserService)).To(typeof(UserService)).InSingletonScope();
        }
    }
}
