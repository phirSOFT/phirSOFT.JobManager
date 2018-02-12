using System;
using System.Collections.Generic;

namespace phirSOFT.JobManager.Core
{
    internal class JobStatusComparator : IComparer<JobStatus>
    {
        public int Compare(JobStatus x, JobStatus y)
        {
            int Convert(JobStatus status)
            {
                switch (status)
                {
                    case JobStatus.Running:
                        return 3;
                    case JobStatus.Paused:
                        return 2;
                    case JobStatus.Succeded:
                        return 1;
                    case JobStatus.Faulted:
                        return 4;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }
            }

            return Convert(x) - Convert(y);
        }
    }
}