using System.Collections.Generic;
using phirSOFT.JobManager.Core.Annotations;

namespace phirSOFT.JobManager.Core
{
    /// <summary>
    ///     Describes the interface of a job manager.
    /// </summary>
    [PublicAPI]
    public interface IJobManager : IReadOnlyCollection<IJob>
    {
        /// <summary>
        ///     Gets the overall progress of this job manager.
        /// </summary>
        /// <remarks>
        ///     This should be the average progress of all jobs, wich <see cref="IJob.SupportProgress" /> property is set to true.
        /// </remarks>
        double OverallProgress { get; }

        /// <summary>
        ///     Gets wheter an overall progress can be displayed.
        /// </summary>
        /// <remarks>
        ///     This should be true, if any progress can display a progress.
        /// </remarks>
        bool CanDisplayOverallProgress { get; }

        /// <summary>
        ///     Gets the overall status of this job manager
        /// </summary>
        /// <remarks>
        ///     The overall status should follow theese guidelines:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Status</term>
        ///             <description>Should be applied, if ...</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="JobStatus.Faulted" />
        ///             </term>
        ///             <description>any of the registred jobs is in faulted state.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="JobStatus.Running" />
        ///             </term>
        ///             <description>any of the registred jobs is running.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="JobStatus.Paused" />
        ///             </term>
        ///             <description>all registred running jobs are in paused states.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="JobStatus.Succeded" />
        ///             </term>
        ///             <description>all registred running jobs succeded, or no job is registred.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        JobStatus OverallStatus { get; }

        /// <summary>
        ///     Registers a job in this job manager.
        /// </summary>
        /// <param name="job">The job to be registred</param>
        void RegisterJob(IJob job);

        /// <summary>
        ///     Undos the registration of a job in this manager.
        /// </summary>
        /// <param name="job">The job to be deregistered</param>
        void DeregisterJob(IJob job);
    }
}