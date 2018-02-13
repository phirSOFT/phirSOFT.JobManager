using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    /// <summary>
    ///     Provides an <see cref="IJob" /> wrapper for an <see cref="IJobManager" />.
    /// </summary>
    public sealed class JobManagerJob : IJob, INotifyPropertyChanged
    {
        private readonly IJobManager _manager;

        /// <summary>
        ///     Wraps an <see cref="IJobManager" /> into an <see cref="IJob" />
        /// </summary>
        /// <param name="manager">The job manager to wrap.</param>
        /// <remarks>
        ///     Don't register a wrapped JobManager into itself. This could'd cause an infinite recursion. This also applies
        ///     if you wrap JobManager A into Job A and register this job into JobManager B, which you add wrapped into JobManager
        ///     A.
        /// </remarks>
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

        /// <inheritdoc />
        public bool SupportCancellation { get; }

        /// <inheritdoc />
        public bool SupportPausing { get; }

        /// <inheritdoc />
        public bool SupportProgress => _manager.CanDisplayOverallProgress;

        /// <inheritdoc />
        public double Progress => _manager.OverallProgress;

        /// <inheritdoc />
        public JobStatus Status => _manager.OverallStatus;

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public bool CanCancel { get; }

        /// <inheritdoc />
        public bool CanPause { get; }

        /// <inheritdoc />
        public bool CanResume { get; }

        /// <inheritdoc />
        public void Cancel()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Pause()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Resume()
        {
            throw new NotSupportedException();
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

        private void OnFinished()
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
}