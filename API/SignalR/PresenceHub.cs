using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    /// <summary>
    /// Được gọi khi một client kết nối tới PresenceHub.
    /// </summary>
    /// <returns>Task đại diện cho thao tác bất đồng bộ.</returns>
    public override async Task OnConnectedAsync()
    {
        // Đảm bảo kết nối đến từ một người dùng đã xác thực
        if (Context.User == null)
            throw new HubException("Không thể lấy thông tin người dùng hiện tại");

        // Đăng ký kết nối mới; trả về true nếu đây là kết nối đầu tiên của người dùng
        var isOnline = await tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

        // Nếu đây là kết nối đầu tiên, thông báo cho các client khác rằng người dùng đã online
        if (isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        // Lấy danh sách người dùng đang online hiện tại
        var currentUsers = await tracker.GetOnlineUsers();

        // Gửi danh sách người dùng online về cho client vừa kết nối
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    /// <summary>
    /// Được gọi khi một client ngắt kết nối khỏi PresenceHub.
    /// </summary>
    /// <param name="exception">Ngoại lệ xảy ra khi ngắt kết nối (nếu có).</param>
    /// <returns>Task đại diện cho thao tác bất đồng bộ.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Đảm bảo ngữ cảnh ngắt kết nối có người dùng hợp lệ
        if (Context.User == null)
            throw new HubException("Không thể lấy thông tin người dùng hiện tại");

        // Hủy đăng ký kết nối; trả về true nếu đây là kết nối cuối cùng của người dùng
        var isOffline = await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

        // Nếu không còn kết nối nào khác, thông báo cho các client rằng người dùng đã offline
        if (isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

        // Gọi hàm cơ sở để hoàn tất quá trình ngắt kết nối
        await base.OnDisconnectedAsync(exception);
    }
}
