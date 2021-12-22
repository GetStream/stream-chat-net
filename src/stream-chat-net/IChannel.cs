using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamChat
{
    public interface IChannel
    {
        string Type { get; }
        string ID { get; }

        Task<ChannelState> Create(string createdBy, IEnumerable<string> members = null);

        Task<Message> SendMessage(MessageInput msg, string userID, bool skipPush = false);
        Task<Message> SendMessage(MessageInput msg, string userID, string parentID, bool skipPush = false);

        Task<Event> SendEvent(Event evt, string userID);

        Task<ReactionResponse> SendReaction(string messageID, Reaction reaction, string userID, bool skipPush = false);
        Task<ReactionResponse> DeleteReaction(string messageID, string reactionType, string userID);
        Task<List<Reaction>> GetReactions(string messageID, int offset = 0, int limit = 50);

        Task<ChannelState> Query(ChannelQueryParams queryParams);

        Task<UpdateChannelResponse> Update(GenericData customData, MessageInput msg = null, bool skipPush = false);
        Task<PartialUpdateChannelResponse> PartialUpdate(PartialUpdateChannelRequest partialUpdateChannelRequest);
        Task Delete();
        Task<TruncateResponse> Truncate();
        Task<TruncateResponse> Truncate(TruncateOptions truncateOptions);

        Task AddMembers(IEnumerable<string> userIDs, MessageInput msg = null, AddMemberOptions options = null);
        Task RemoveMembers(IEnumerable<string> userIDs, MessageInput msg = null);
        Task AddModerators(IEnumerable<string> userIDs);
        Task DemoteModerators(IEnumerable<string> userIDs);

        Task<Event> MarkRead(string userID, string messageID = "");
        Task<List<Message>> GetReplies(string parentID, MessagePaginationParams pagination);

        Task BanUser(string targetID, string id, string reason, int timeoutMinutes = 0);
        Task UnbanUser(string targetID);
        Task<UpdateChannelResponse> AssignRoles(AssignRoleRequest roleRequest);

        Task Hide(string userID, bool clearHistory = false);
        Task Show(string userID);
    }
}
