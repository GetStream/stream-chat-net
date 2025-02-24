using StreamChat.Clients;
using StreamChat.Models;

namespace DocsExamples;

/// <summary>
/// Code examples for <see href="https://getstream.io/chat/docs/dotnet-csharp/exporting_channels/"/>
/// </summary>
internal class ChannelExporting
{
    private readonly IUserClient _userClient;
    private readonly ITaskClient _taskClient;

    public ChannelExporting()
    {
        var factory = new StreamClientFactory("{{ api_key }}", "{{ api_secret }}");
        _userClient = factory.GetUserClient();
        _taskClient = factory.GetTaskClient();
    }

    public async Task ExportUsersAsync()
    {
        var exportResponse = await _userClient.ExportUsersAsync(new[] { "user-id-1", "user-id-2" });
        var taskId = exportResponse.TaskId;
    }

    public async Task RetrievingTaskStatusAsync()
    {
        var taskId = string.Empty;
        
        // ITaskClient can provide the status of the export operation
        var taskStatus = await _taskClient.GetTaskStatusAsync(taskId);
        if (taskStatus.Status == AsyncTaskStatus.Completed)
        {
            // The export operation is completed
            // Result object contains the export file URL
            var exportedFileUrl = taskStatus.Result.Values.First().ToString();
        }
    }
}