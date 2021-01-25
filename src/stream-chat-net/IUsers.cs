using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamChat
{
    public interface IUsers
    {
        Task<IEnumerable<User>> UpsertMany(IEnumerable<User> users);
        Task<User> Upsert(User user);

        Task<User> UpdatePartial(UserPartialRequest update);
        Task<IEnumerable<User>> UpdateManyPartial(IEnumerable<UserPartialRequest> updates);

        Task<User> Delete(string id, bool markMessagesDeleted = false, bool hardDelete = false);
        Task<User> Deactivate(string id, bool markMessagesDeleted = false);
        Task<User> Reactivate(string id, bool restoreMessages = false);

        Task<ExportedUser> Export(string id);

        Task Ban(string targetUserID, string id, string reason, int timeoutMinutes = 0);
        Task Ban(string targetUserID, string id, string reason, Channel channel, int timeoutMinutes = 0);
        Task Unban(string targetUserID, Channel channel = null);

        Task<MuteResponse> Mute(string targetID, string id);
        Task Unmute(string targetID, string id);

        Task MarkAllRead(string id);

        Task<IEnumerable<User>> Query(QueryUserOptions opts);

    }
}
