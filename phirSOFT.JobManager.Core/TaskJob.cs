using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    /// <summary>
    ///     Wraps a <see cref="Task" /> into an <see cref="IJob" />
    /// </summary>
    public sealed class TaskJob : IJob, INotifyPropertyChanged
    {
        private readonly CancellationTokenSource _cts;
        private readonly Task _internalTask;
        private readonly PauseTokenSource _pts;
        private string _description;
        private double _progress;

        /// <summary>
        ///     Wraps a <see cref="Task" /> into a <see cref="IJob" />
        /// </summary>
        /// <param name="task">The <see cref="Task" /> to wrap</param>
        /// <param name="supportProgress">Returns wheter this task supports progress reporting.</param>
        /// <param name="cancellationTokenSource">The cancellation tokensource to cancel the task.</param>
        /// <param name="pauseTokenSource">The pause token soruce to pause the task</param>
        public TaskJob(Task task, bool supportProgress = false, CancellationTokenSource cancellationTokenSource = null,
            PauseTokenSource pauseTokenSource = null)
        {
            _internalTask = task;
            SupportProgress = supportProgress;
            _cts = cancellationTokenSource;
            _pts = pauseTokenSource;
        }

        /// <inheritdoc />
        public bool SupportCancellation => _cts != null;

        /// <inheritdoc />
        public bool SupportPausing => _pts != null;

        /// <inheritdoc />
        public bool SupportProgress { get; }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool CanCancel => _cts != null;

        /// <inheritdoc />
        public bool CanPause => _pts != null && !_pts.IsPaused;

        /// <inheritdoc />
        public bool CanResume => _pts != null && !_pts.IsPaused;

        /// <inheritdoc />
        public void Cancel()
        {
            _cts.Cancel();
        }

        /// <inheritdoc />
        public void Pause()
        {
            _pts.IsPaused = true;
        }

        /// <inheritdoc />
        public void Resume()
        {
            _pts.IsPaused = false;
        }

        /// <inheritdoc />
        public event EventHandler Finished;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}