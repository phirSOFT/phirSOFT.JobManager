namespace phirSOFT.JobManager.Core
{
    /// <summary>
    ///     States of a job
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        ///     The job is currently executed.
        /// </summary>
        Running = 1,

        /// <summary>
        ///     The execution of the job is paused.
        /// </summary>
        Paused = 2,

        /// <summary>
        ///     The job sucessfully completed.
        /// </summary>
        Succeded = 3,

        /// <summary>
        ///     An Error occured during the execution of the job.
        /// </summary>
        Faulted = 4
    }
}