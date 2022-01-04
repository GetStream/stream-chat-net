using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamChat
{
    public interface IClient
    {
        IUsers Users { get; }
        IPermissions Permissions { get; }

        string CreateToken(string userId, DateTime? expiration = null, DateTime? issuedAt = null);

        Task UpdateAppSettings(AppSettings settings);
        Task<AppSettingsWithDetails> GetAppSettings();

        Task<ChannelTypeWithStringCommands> CreateChannelType(ChannelTypeWithStringCommands channelType);
        Task<ChannelTypeWithCommands> GetChannelType(string type);
        Task<Dictionary<string, ChannelTypeWithCommands>> ListChannelTypes();
        Task<ChannelTypeWithStringCommands> UpdateChannelType(string type, ChannelTypeWithStringCommands channelType);
        Task DeleteChannelType(string type);

        Task AddDevice(Device d);
        Task<List<Device>> GetDevices(string userID);
        Task RemoveDevice(string deviceID, string userID);

        Task<RateLimitsMap> GetRateLimits(GetRateLimitsOptions opts);

        IChannel Channel(string channelType, string channelID = "", GenericData data = null);

        Task<List<ChannelState>> QueryChannels(QueryChannelsOptions opts);

        Task<MessageSearchResponse> Search(SearchOptions opts);
        Task<Message> GetMessage(string messageID);
        Task<Message> UpdateMessage(MessageInput msg);
        Task<MessagePartialUpdateResponse> UpdateMessagePartial(string messageId, MessagePartialUpdateRequest partialUpdateRequest);
        Task<Message> DeleteMessage(string messageID, bool hardDelete = false);


        Task FlagUser(string flaggedUserID, string flaggerUserID);
        Task UnflagUser(string flaggedUserID, string flaggerUserID);
        Task FlagMessage(string flaggedMessageID, string flaggerUserID);
        Task UnflagMessage(string flaggedMessageID, string flaggerUserID);

        Task<string> ExportChannels(List<ExportChannelRequest> reqs);
        Task<ExportChannelsStatusResponse> GetExportChannelsStatus(string taskId);

        Task<TaskStatus> GetTaskStatus(string taskId);
        Task<string> DeleteChannels(string[] cids, bool hardDelete = false);

        bool VerifyWebhook(string requestBody, string xSignature);
    }
}
