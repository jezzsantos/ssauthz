using Services.MessageContracts;

namespace Services.AuthZ.Services
{
#if TESTINGONLY
    /// <summary>
    /// Defines a testing service
    /// </summary>
    public interface ITestingOnlyService
    {
        /// <summary>
        /// Resets the web role
        /// </summary>
        ResetWebRoleResponse Get(ResetWebRole request);
    }
#endif
}
