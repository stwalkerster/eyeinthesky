﻿namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;
    
    public class Template : ITemplate
    {
        private string stalkFlag;
        private bool templateIsEnabled;
        private bool stalkIsEnabled;
        private bool mailEnabled;
        private string description;
        private string searchTree;
        private TimeSpan? expiryDuration;

        public Template(string flag)
        {
            this.Flag = flag;
            this.LastUpdateTime = DateTime.Now;
            this.searchTree = "<false />";
        }

        public Template(
            string flag,
            string stalkFlag,
            bool templateIsEnabled,
            bool stalkIsEnabled,
            bool mailEnabled,
            string description,
            DateTime? lastUpdateTime,
            TimeSpan? expiryDuration,
            string searchTree)
        {
            this.stalkFlag = stalkFlag;
            this.templateIsEnabled = templateIsEnabled;
            this.stalkIsEnabled = stalkIsEnabled;
            this.mailEnabled = mailEnabled;
            this.description = description;
            this.searchTree = searchTree;
            this.expiryDuration = expiryDuration;
            this.Flag = flag;
            this.LastUpdateTime = lastUpdateTime;
        }

        public string Flag { get; private set; }
        public DateTime? LastUpdateTime { get; private set; }

        public TimeSpan? ExpiryDuration
        {
            get { return this.expiryDuration; }
            set
            {
                this.expiryDuration = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }

        public string StalkFlag
        {
            get { return this.stalkFlag; }
            set
            {
                this.stalkFlag = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }

        public bool TemplateIsEnabled
        {
            get { return this.templateIsEnabled; }
            set
            {
                this.templateIsEnabled = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }

        public bool StalkIsEnabled
        {
            get { return this.stalkIsEnabled; }
            set
            {
                this.stalkIsEnabled = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }
        
        public bool MailEnabled
        {
            get { return this.mailEnabled; }
            set
            {
                this.mailEnabled = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }
        
        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }
        
        public string SearchTree
        {
            get { return this.searchTree; }
            set
            {
                this.LastUpdateTime = DateTime.Now;
                this.searchTree = value;
            }
        }
    }
}