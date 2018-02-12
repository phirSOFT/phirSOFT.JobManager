using System;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    [PublicAPI]
    public interface IJob
    {
        /// <summary>
        ///     Determines wether this job supports cancelation.
        /// </summary>
        bool SupportCancellation { get; }

        /// <summary>
        ///     Determines wheter this job supports pausing.
        /// </summary>
        bool SupportPausing { get; }

        /// <summary>
        ///     Determines wheter this job can display a progress.
        /// </summary>
        bool SupportProgress { get; }

        /// <summary>
        ///     Gets the progress of the job in percent (if supported).
        /// </summary>
        double Progress { get; }

        /// <summary>
        ///     Gets the current status of the job.
        /// </summary>
        JobStatus Status { get; }

        /// <summary>
        ///     Gets the title of the job.
        /// </summary>
        string Title { get; }

        /// <summary>
        ///     Gets a more detailed description of the job.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Gets whether the job currently can be cancelled. (if cancellation is supported)
        /// </summary>
        bool CanCancel { get; }

        /// <summary>
        ///     Gets wether the job currently can be paused. (if pausing is supported)
        /// </summary>
        bool CanPause { get; }

        /// <summary>
        ///     Gets wether the job currently can be resumed. (if pausing is supported)
        /// </summary>
        bool CanResume { get; }

        /// <summary>
        ///     Cancels the job. (if supported)
        /// </summary>
        void Cancel();

        /// <summary>
        ///     Pauses the job. (if supported)
        /// </summary>
        void Pause();

        /// <summary>
        ///     Resumes the job. (if supported)
        /// </summary>
        void Resume();

        /// <summary>
        ///     Is Invoked, when the job finished (sucessfull or not).
        /// </summary>
        event EventHandler Finished;
    }
}