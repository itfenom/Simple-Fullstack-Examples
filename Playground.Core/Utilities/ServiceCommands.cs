namespace Playground.Core.Utilities
{
    /// <summary>
    /// This class represents a set of commands that can be sent from the data viewer to one of the
    /// processing services.
    /// </summary>
    /// <remarks>Commands must have a value between 128 - 256. See https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller.executecommand?view=netframework-4.7.1#System_ServiceProcess_ServiceController_ExecuteCommand_System_Int32_</remarks>
    public enum ServiceCommands
    {
        /// <summary>
        /// Represents a command that will cause a service to shut down when it has a break in processing.
        /// </summary>
        ShutDownWhenPossible = 128
    };
}
