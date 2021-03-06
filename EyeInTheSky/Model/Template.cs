﻿namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public class Template : ITemplate
    {
        private string stalkFlag;
        private bool templateIsEnabled;
        private bool stalkIsEnabled;
        private string description;
        private string searchTree;
        private TimeSpan? expiryDuration;

        public Template(string flag)
        {
            this.Identifier = flag;
            this.LastUpdateTime = DateTime.UtcNow;
            this.searchTree = "<false />";
        }

        public Template(
            string flag,
            string stalkFlag,
            bool templateIsEnabled,
            bool stalkIsEnabled,
            string description,
            DateTime? lastUpdateTime,
            TimeSpan? expiryDuration,
            string searchTree,
            string watchChannel)
        {
            this.stalkFlag = stalkFlag;
            this.templateIsEnabled = templateIsEnabled;
            this.stalkIsEnabled = stalkIsEnabled;
            this.description = description;
            this.searchTree = searchTree;
            this.expiryDuration = expiryDuration;
            this.Identifier = flag;
            this.LastUpdateTime = lastUpdateTime;
            this.WatchChannel = watchChannel;
        }

        public string Identifier { get; private set; }
        public DateTime? LastUpdateTime { get; private set; }

        public TimeSpan? ExpiryDuration
        {
            get { return this.expiryDuration; }
            set
            {
                this.expiryDuration = value;
                this.LastUpdateTime = DateTime.UtcNow;
            }
        }

        public string StalkFlag
        {
            get { return this.stalkFlag; }
            set
            {
                this.stalkFlag = value;
                this.LastUpdateTime = DateTime.UtcNow;
            }
        }

        public string WatchChannel { get; set; }

        public bool TemplateIsEnabled
        {
            get { return this.templateIsEnabled; }
            set
            {
                this.templateIsEnabled = value;
                this.LastUpdateTime = DateTime.UtcNow;
            }
        }

        public bool StalkIsEnabled
        {
            get { return this.stalkIsEnabled; }
            set
            {
                this.stalkIsEnabled = value;
                this.LastUpdateTime = DateTime.UtcNow;
            }
        }

        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;
                this.LastUpdateTime = DateTime.UtcNow;
            }
        }

        public string SearchTree
        {
            get { return this.searchTree; }
            set
            {
                this.LastUpdateTime = DateTime.UtcNow;
                this.searchTree = value;
            }
        }
    }
}