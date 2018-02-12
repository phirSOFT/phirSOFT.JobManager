using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    public sealed class TaskJob : IJob, INotifyPropertyChanged
    {
        private readonly Task _internalTask;
        private readonly CancellationTokenSource _cts;
        private readonly PauseTokenSource _pts;
        private string _description;
        private double _progress;

        public TaskJob(Task task, bool supportProgress = false, CancellationTokenSource cancellationTokenSource = null,
            PauseTokenSource pauseTokenSource = null)
        {
            _internalTask = task;
            SupportProgress = supportProgress;
            _cts = cancellationTokenSource;
            _pts = pauseTokenSource;
        }

        public bool SupportCancellation => _cts != null;
        public bool SupportPausing => _pts != null;
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
                        return _pts.IsPaused ? JobStatus.Paused : JobStatus.Running;

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

        public bool CanCancel => _cts != null;
        public bool CanPause => _pts != null && !_pts.IsPaused;
        public bool CanResume => _pts != null && !_pts.IsPaused;

        public void Cancel()
        {
            _cts.Cancel();
        }

        public void Pause()
        {
            _pts.IsPaused = true;
        }

        public void Resume()
        {
            _pts.IsPaused = false;
        }

        public event EventHandler Finished;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}