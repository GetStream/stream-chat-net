using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter push notification devices of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_devices/?language=csharp</remarks>
    public interface IDeviceClient
    {
        /// <summary>
        /// <para>Adds a new device.</para>
        /// Registering a device associates it with a user and tells the
        /// push provider to send new message notifications to the device.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_devices/?language=csharp</remarks>
        Task<ApiResponse> AddDeviceAsync(Device device);

        /// <summary>
        /// Provides a list of all devices associated with a user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_devices/?language=csharp</remarks>
        Task<GetDevicesResponse> GetDevicesAsync(string userId);

        /// <summary>
        /// <para>Removes a device.</para>
        /// Unregistering a device removes the device from the user and stops further new message notifications.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_devices/?language=csharp</remarks>
        Task<ApiResponse> RemoveDeviceAsync(string deviceId, string userId);
    }
}