using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    public class TaskJob : IJob, INotifyPropertyChanged
    {
        private readonly CancellationTokenSource cts;
        private readonly PauseTokenSource pts;
        private readonly Task _internalTask;
        private string _description;
        private double _progress = 0;

        public TaskJob(Task task, bool supportProgress = false, CancellationTokenSource cancellationTokenSource = null, PauseTokenSource pauseTokenSource = null)
        {
            _internalTask = task;
            SupportProgress = supportProgress;
            cts = cancellationTokenSource;
            pts = pauseTokenSource;
        }
        public bool SupportCancellation => cts != null;
        public bool SupportPausing => pts != null;
        public bool SupportProgress { get; }

        public double Progress
        {
            get => _progress;
            set
            {
                if (Math.Abs(_progress - value) < double.Epsilon) return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        public JobStatus Status
        {
            get
            {
                switch (_internalTask.Status)
                {
                    case TaskStatus.Faulted:
                    case TaskStatus.Canceled:
                        return JobStatus.Faulted;
                    case TaskStatus.Created:
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingForChildrenToComplete:
                    case TaskStatus.WaitingToRun:
                        return pts.IsPaused ? JobStatus.Paused : JobStatus.Running;

                    case TaskStatus.RanToCompletion:
                        return JobStatus.Succeded;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string Title { get; set; }

        public string Description
        {
            get => _description;
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public bool CanCancel => cts != null;
        public bool CanPause => pts != null && !pts.IsPaused;
        public bool CanResume => pts != null && !pts.IsPaused;
        public void Cancel()
        {
            cts.Cancel();
        }

        public void Pause()
        {
            pts.IsPaused = true;
        }

        public void Resume()
        {
            pts.IsPaused = false;
        }

        public event EventHandler Finished;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
