using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shell;
using phirSOFT.JobManager.Core;

namespace phirSOFT.JobManager.Wpf
{
    public class JobManagerStatus
    {
        public static readonly DependencyProperty JobManagerProperty = DependencyProperty.RegisterAttached(
            "JobManager", typeof(IJobManager), typeof(JobManagerStatus),
            new PropertyMetadata(default(IJobManager), PropertyChangedCallback));

        private static readonly Dictionary<IJobManager, List<TaskbarItemInfo>> Items =
            new Dictionary<IJobManager, List<TaskbarItemInfo>>();

        private static readonly object ItemsLock = new object();

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.OldValue is INotifyPropertyChanged oldValue)
                oldValue.PropertyChanged -= JobChanged;

            if (dependencyPropertyChangedEventArgs.NewValue is INotifyPropertyChanged newValue)
                newValue.PropertyChanged += JobChanged;

            lock (ItemsLock)
            {
                var info = (TaskbarItemInfo) dependencyObject;
                var old = (IJobManager) dependencyPropertyChangedEventArgs.OldValue;
                var @new = (IJobManager) dependencyPropertyChangedEventArgs.NewValue;
                if (old != null)
                    if (Items[old].Count == 1)
                        Items.Remove(old);
                    else
                        Items[old].Remove(info);

                if (@new != null)
                    if (Items.ContainsKey(@new))
                        Items.Add(@new, new List<TaskbarItemInfo> {info});
                    else
                        Items[@new].Add(info);
            }
        }

        private static void JobChanged(object sender, PropertyChangedEventArgs e)
        {
            var manager = (IJobManager) sender;
            lock (ItemsLock)
            {
                TaskbarItemProgressState state;

                switch (manager.OverallStatus)
                {
                    case JobStatus.Running:
                        state = manager.CanDisplayOverallProgress
                            ? TaskbarItemProgressState.Normal
                            : TaskbarItemProgressState.Indeterminate;
                        break;
                    case JobStatus.Paused:
                        state = TaskbarItemProgressState.Paused;
                        break;
                    case JobStatus.Succeded:
                        state = TaskbarItemProgressState.None;
                        break;
                    case JobStatus.Faulted:
                        state = TaskbarItemProgressState.Error;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                foreach (var info in Items[manager])
                {
                    info.ProgressValue = manager.OverallProgress;
                    info.ProgressState = state;
                }
            }
        }

        public static void SetJobManager(TaskbarItemInfo element, IJobManager value)
        {
            element.SetValue(JobManagerProperty, value);
        }

        public static IJobManager GetJobManager(TaskbarItemInfo element)
        {
            return (IJobManager) element.GetValue(JobManagerProperty);
        }
    }
}