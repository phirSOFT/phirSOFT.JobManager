using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    public sealed class JobManager : IJobManager, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly HashSet<IJob> _registredJobs;
        private readonly ReaderWriterLockSlim _registredJobsLock;
        private readonly IComparer<JobStatus> _statusConverter;

        public JobManager()
        {
            _registredJobs = new HashSet<IJob>();
            _registredJobsLock = new ReaderWriterLockSlim();

            JobFinishedHandler = (sender, args) =>
            {
                if (((IJob) sender).Status == JobStatus.Succeded)
                    DeregisterJob((IJob) sender);
            };

            _statusConverter = new JobStatusComparator();
        }

        [PublicAPI]
        public EventHandler JobFinishedHandler { get; set; }

        public double OverallProgress
        {
            get
            {
                _registredJobsLock.EnterReadLock();
                var result = _registredJobs.Where(job => job.Status == OverallStatus && job.SupportProgress)
                    .Average(job => job.Progress);
                _registredJobsLock.ExitReadLock();
                return result;
            }
        }

        public bool CanDisplayOverallProgress
        {
            get
            {
                _registredJobsLock.EnterReadLock();
                var result = _registredJobs.Any(job => job.Status == OverallStatus && job.SupportProgress);
                _registredJobsLock.ExitReadLock();
                return result;
            }
        }

        public JobStatus OverallStatus
        {
            get
            {
                _registredJobsLock.EnterReadLock();
                JobStatus max;
                using (var enumerator = _registredJobs.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                        return JobStatus.Succeded;

                    Debug.Assert(enumerator.Current != null, "enumerator.Current != null");
                    max = enumerator.Current.Status;
                    while (enumerator.MoveNext())
                    {
                        Debug.Assert(enumerator.Current != null, "enumerator.Current != null");
                        if (_statusConverter.Compare(max, enumerator.Current.Status) < 0)
                            max = enumerator.Current.Status;
                    }
                }

                return max;
            }
        }

        public int Count => _registredJobs.Count;

        public void DeregisterJob(IJob job)
        {
            _registredJobsLock.EnterWriteLock();
            var deleted = _registredJobs.Remove(job);
            _registredJobsLock.ExitWriteLock();

            if (!deleted) return;
            job.Finished -= Job_Finished;

            if (job is INotifyPropertyChanged notify)
                notify.PropertyChanged -= OnJobPropertyChanged;
        }

        public IEnumerator<IJob> GetEnumerator()
        {
            _registredJobsLock.EnterReadLock();
            var enumerator = _registredJobs.GetEnumerator();
            _registredJobsLock.ExitReadLock();

            return enumerator;
        }

        public void RegisterJob(IJob job)
        {
            _registredJobsLock.EnterWriteLock();
            var contained = _registredJobs.Add(job);
            _registredJobsLock.ExitWriteLock();

            if (contained) return;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, job));
            job.Finished += Job_Finished;

            if (job is INotifyPropertyChanged notify)
                notify.PropertyChanged += OnJobPropertyChanged;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnJobPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var job = (IJob) sender;

            switch (e.PropertyName)
            {
                case nameof(IJob.Status):
                    OnPropertyChanged(nameof(OverallStatus));
                    OnPropertyChanged(nameof(OverallProgress));
                    OnPropertyChanged(nameof(CanDisplayOverallProgress));
                    break;
                case nameof(IJob.Progress):
                    if (job.Status == OverallStatus)
                        OnPropertyChanged(nameof(OverallProgress));
                    break;
                case nameof(IJob.SupportProgress):
                    if (job.Status == OverallStatus)
                    {
                        OnPropertyChanged(nameof(CanDisplayOverallProgress));
                        OnPropertyChanged(nameof(OverallProgress));
                    }

                    break;
            }
        }

        private void Job_Finished(object sender, EventArgs e)
        {
            JobFinishedHandler(sender, e);
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(Count));
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}