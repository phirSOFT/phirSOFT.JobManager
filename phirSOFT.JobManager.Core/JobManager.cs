using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    public class JobManager : IJobManager, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly HashSet<IJob> _registredJobs;
        private readonly ReaderWriterLockSlim _registredJobsLock;
        private readonly object _handlerLock;

        public EventHandler JobFinishedHandler { get; set; }

        public double OverallProgress => throw new System.NotImplementedException();

        public bool CanDisplayOverallProgress => throw new System.NotImplementedException();

        public JobStatus OverallStatus => throw new System.NotImplementedException();

        public int Count => _registredJobs.Count;

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void OnJobPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IJob> GetEnumerator()
        {
            _registredJobsLock.EnterReadLock();
            var enumerator =  _registredJobs.GetEnumerator();
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

        protected virtual void Job_Finished(object sender, EventArgs e)
        {
            JobFinishedHandler(sender, e);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(Count));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}