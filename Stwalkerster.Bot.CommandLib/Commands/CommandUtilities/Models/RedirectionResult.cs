namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// The redirection result.
    /// </summary>
    public class RedirectionResult
    {
        #region Fields

        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly IEnumerable<string> arguments;

        /// <summary>
        /// The target.
        /// </summary>
        private readonly IEnumerable<string> target;

        /// <summary>
        /// The channel targets
        /// </summary>
        private readonly IEnumerable<string> channelTargets;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="RedirectionResult"/> class.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="channelTargets">
        /// The channel targets.
        /// </param>
        public RedirectionResult(IEnumerable<string> arguments, IEnumerable<string> target, IEnumerable<string> channelTargets)
        {
            this.arguments = arguments;
            this.target = target;
            this.channelTargets = channelTargets;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public IEnumerable<string> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public IEnumerable<string> Target
        {
            get
            {
                return this.target;
            }
        }

        /// <summary>
        /// The channel targets
        /// </summary>
        public IEnumerable<string> ChannelTargets
        {
            get
            {
                return this.channelTargets;
            }
        }

        #endregion
    }
}