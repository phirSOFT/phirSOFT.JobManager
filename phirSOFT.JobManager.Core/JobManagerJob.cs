using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    public class JobManagerJob : IJob, INotifyPropertyChanged
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
        public bool SupportCancellation { get; } = false;
        public bool SupportPausing { get; } = false;
        public bool SupportProgress => _manager.CanDisplayOverallProgress;
        public double Progress => _manager.OverallProgress;
        public JobStatus Status => _manager.OverallStatus;
        public string Title { get; set; }
        public string Description { get; set; }
        public bool CanCancel { get; } = false;
        public bool CanPause { get; } = false;
        public bool CanResume { get; } = false;
        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Finished;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnFinished()
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
}
