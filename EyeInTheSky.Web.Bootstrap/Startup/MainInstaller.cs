namespace EyeInTheSky.Web.Bootstrap.Startup
{
    using System.Collections.Generic;

    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using EyeInTheSky.Web.Startup;
    using NSubstitute;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Facilities
            container.AddFacility<LoggingFacility>(f => f.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
            container.AddFacility<StartableFacility>(f => f.DeferredStart());

            container.Install(new WebInstaller());
            
            var ircClientMock = Substitute.For<IIrcClient>();
            ircClientMock.ExtBanTypes.Returns("a");
            ircClientMock.ExtBanDelimiter.Returns("$");
            ircClientMock.Nickname.Returns("EyeInTheSkyBot");
            ircClientMock.ClientName.Returns("Libera.Chat");

            var ircChannels = new Dictionary<string, IrcChannel>();
            var chan = new IrcChannel("##stwalkerster-development");
            ircChannels.Add(chan.Name, chan);
            ircClientMock.Channels.Returns(ircChannels);

            IrcUser user;
            IrcChannelUser ircChannelUser;

            user = IrcUser.FromPrefix("stwalkerster!test@user/.", ircClientMock);
            user.Account = "stwalkerster";
            user.SkeletonStatus = IrcUserSkeletonStatus.Full;
            ircChannelUser = new IrcChannelUser(user, chan.Name);
            chan.Users.Add(user.Nickname, ircChannelUser);


            user = IrcUser.FromPrefix("chanmember!test@user/.", ircClientMock);
            user.Account = "chanmember";
            user.SkeletonStatus = IrcUserSkeletonStatus.Full;
            ircChannelUser = new IrcChannelUser(user, chan.Name);
            chan.Users.Add(user.Nickname, ircChannelUser);

            user = IrcUser.FromPrefix("chanop!test@user/.", ircClientMock);
            user.Account = "chanop";
            user.SkeletonStatus = IrcUserSkeletonStatus.Full;
            ircChannelUser = new IrcChannelUser(user, chan.Name);
            ircChannelUser.Operator = true;
            chan.Users.Add(user.Nickname, ircChannelUser);

            container.Register(
                // Main application
                Component.For<IApplication>().ImplementedBy<Launch>(),
                Classes.FromAssemblyNamed("EyeInTheSky").InNamespace("EyeInTheSky.Services").WithServiceAllInterfaces(),
                Classes.FromAssemblyNamed("EyeInTheSky").InNamespace("EyeInTheSky.Services.ExternalProviders").WithServiceAllInterfaces(),
                Classes.FromAssemblyNamed("EyeInTheSky").InNamespace("EyeInTheSky.Services.Email").WithServiceAllInterfaces(),
                Component.For<IIrcClient>().Instance(ircClientMock)
            );

            container.Install(Configuration.FromXmlFile("alert-templates.xml"));
        }
    }
}