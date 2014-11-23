using Common.Configuration;
using Common.Storage;

namespace Common.Services.Workflow
{
    /// <summary>
    ///     Defines a common class for workflow managers
    /// </summary>
    public abstract class WorkflowManager
    {
        /// <summary>
        ///     Gets or sets the <see cref="IConfigurationSettings" />
        /// </summary>
        public IConfigurationSettings Configuration { get; set; }
    }

    /// <summary>
    ///     Defines a common class for workflow managers with storage
    /// </summary>
    public abstract class WorkflowManager<TDto> : WorkflowManager where TDto : class
    {
        /// <summary>
        ///     Gets or sets the <see cref="IStorageProvider{TDto}" />
        /// </summary>
        public IStorageProvider<TDto> Storage { get; set; }
    }
}