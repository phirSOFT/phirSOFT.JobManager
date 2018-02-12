﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    public sealed class JobManagerJob : IJob, INotifyPropertyChanged
    {
        private readonly IJobManager _manager;

        public JobManagerJob(IJobManager manager)
        {
            _manager = manager;
            if (manager is INotifyPropertyChanged notify)
                notify.PropertyChanged += (sender, args) =>
                {
                    switch (args.PropertyName)
                    {
                        case nameof(IJobManager.CanDisplayOverallProgress):
                            OnPropertyChanged(nameof(SupportProgress));
                            break;
                        case nameof(IJobManager.OverallProgress):
                            OnPropertyChanged(nameof(Progress));
                            break;
                        case nameof(IJobManager.OverallStatus):
                            OnPropertyChanged(nameof(Status));
                            if (Status == JobStatus.Succeded || Status == JobStatus.Faulted)
                                OnFinished();
                            break;
                    }
                };
        }

        public bool SupportCancellation { get; }
        public bool SupportPausing { get; }
        public bool SupportProgress => _manager.CanDisplayOverallProgress;
        public double Progress => _manager.OverallProgress;
        public JobStatus Status => _manager.OverallStatus;
        public string Title { get; set; }
        public string Description { get; set; }
        public bool CanCancel { get; }
        public bool CanPause { get; }
        public bool CanResume { get; }

        public void Cancel()
        {
            throw new NotSupportedException();
        }

        public void Pause()
        {
            throw new NotSupportedException();
        }

        public void Resume()
        {
            throw new NotSupportedException();
        }

        public event EventHandler Finished;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnFinished()
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
}