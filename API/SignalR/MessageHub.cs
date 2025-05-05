using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IUnitOfWork unitOfWork, 
    IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];
        if (Context.User == null || string.IsNullOrEmpty(otherUser))
            throw new Exception("Cannot join group");

        // 1) Tạo tên group theo 2 username (luôn nhất quán thứ tự)
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        // 2) Thêm connection hiện tại vào group đó
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        // 3) Thông báo cho tất cả thành viên trong group về việc cập nhật group (danh sách connection)
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        // 4) Lấy lịch sử tin nhắn giữa 2 user và trả về cho client vừa kết nối
        var messages = await unitOfWork.MessageRepository
                        .GetMessageThread(Context.User.GetUsername(), otherUser!);
        // Nếu có thay đổi (ví dụ cập nhật DateRead), lưu vào DB
        if (unitOfWork.HasChanges())
            await unitOfWork.Complete();

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Xóa connection khỏi group trong database
        var group = await RemoveFromMessageGroup();
        // Thông báo cho còn lại trong group về cập nhật group
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="createMessageDto"></param>
    /// <returns></returns>
    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("could not get user");
        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot message yourself");

        // Lấy sender/recipient từ DB
        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient == null || sender == null)
            throw new HubException("Cannot send message at this time");

        // Tạo entity Message
        var message = new Message {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName!,
            RecipientUsername = recipient.UserName!,
            Content = createMessageDto.Content
        };

        // Xác định group chat
        var groupName = GetGroupName(sender.UserName!, recipient.UserName!);
        var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);

        // Nếu recipient đang trong cùng group (đang chat mở), đánh dấu đã đọc
        if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            // Nếu recipient online khác group, gửi notification qua PresenceHub
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName!);
            if (connections?.Count > 0)
            {
                await presenceHub.Clients.Clients(connections)
                    .SendAsync("NewMessageReceived", 
                        new { username = sender.UserName, knownAs = sender.KnownAs });
            }
        }

        // Thêm message vào repository
        unitOfWork.MessageRepository.AddMessage(message);

        // Lưu thay đổi và broadcast message mới tới group
        if (await unitOfWork.Complete())
        {
            await Clients.Group(groupName)
                .SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
    }


    /// <summary>
    /// Lấy hoặc tạo mới entity Group (tương ứng với một cuộc trò chuyện). Tạo Connection (chứa ConnectionId của SignalR và Username).
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    private async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
        var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

        if (group == null)
        {
            group = new Group { Name = groupName };
            unitOfWork.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await unitOfWork.Complete()) return group;

        throw new HubException("Failed to join group");
    }

    /// <summary>
    /// Tìm Group dựa vào ConnectionId, xóa kết nối khỏi entity.
    /// </summary>
    /// <returns></returns>
    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection != null && group != null)
        {
            unitOfWork.MessageRepository.RemoveConnection(connection);
            if (await unitOfWork.Complete()) return group;
        }

        throw new Exception("Failed to remove from group");
    }

    /// <summary>
    /// Đảm bảo tên group không phụ thuộc vào thứ tự gọi: luôn xếp thứ tự từ điển (ordinal).
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    private string GetGroupName(string caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
