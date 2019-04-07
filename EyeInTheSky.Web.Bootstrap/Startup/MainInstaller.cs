namespace EyeInTheSky.Web.Bootstrap.Startup
{
    using System.Collections.Generic;

    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using EyeInTheSky.Web.Startup;
    using Moq;
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

            var ircClientMock = new Mock<IIrcClient>();
            ircClientMock.Setup(x => x.ExtBanTypes).Returns("a");
            ircClientMock.Setup(x => x.ExtBanDelimiter).Returns("$");
            ircClientMock.Setup(x => x.Nickname).Returns("EyeInTheSkyBot");
            ircClientMock.Setup(x => x.ClientName).Returns("Freenode");

            var ircChannels = new Dictionary<string, IrcChannel>();
            var chan = new IrcChannel("##stwalkerster-development");
            ircChannels.Add(chan.Name, chan);
            ircClientMock.Setup(x => x.Channels).Returns(ircChannels);

            IrcUser user;
            IrcChannelUser ircChannelUser;

            user = IrcUser.FromPrefix("stwalkerster!stwalkerst@wikimedia/stwalkerster", ircClientMock.Object);
            user.Account = "stwalkerster";
            ircChannelUser = new IrcChannelUser(user, chan.Name);
            chan.Users.Add(user.Nickname, ircChannelUser);

            user = IrcUser.FromPrefix("ChanServ!ChanServ@services.", ircClientMock.Object);
            ircChannelUser = new IrcChannelUser(user, chan.Name);
            ircChannelUser.Operator = true;
            chan.Users.Add(user.Nickname, ircChannelUser);

            user = IrcUser.FromPrefix(
                "EyeInTheSkyBot!eyeinthesk@unaffiliated/stwalkerster/bot/eyeintheskybot",
                ircClientMock.Object);
            ircChannelUser = new IrcChannelUser(user, chan.Name);
            ircChannelUser.Voice = true;
            chan.Users.Add(user.Nickname, ircChannelUser);

            container.Register(
                // Main application
                Component.For<IApplication>().ImplementedBy<Launch>(),
                Classes.FromAssemblyNamed("EyeInTheSky").InNamespace("EyeInTheSky.Services").WithServiceAllInterfaces(),
                Classes.FromAssemblyNamed("EyeInTheSky").InNamespace("EyeInTheSky.Services.ExternalProviders").WithServiceAllInterfaces(),
                Component.For<IIrcClient>().Instance(ircClientMock.Object)
            );
        }
    }
}