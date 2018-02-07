namespace phirSOFT.JobManager.Core
{
    /// <summary>
    /// States of a job
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// The job is currently executed.
        /// </summary>
        Running,

        /// <summary>
        /// The execution of the job is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The job sucessfully completed.
        /// </summary>
        Succeded,

        /// <summary>
        /// An Error occured during the execution of the job.
        /// </summary>
        Faulted
    }
}