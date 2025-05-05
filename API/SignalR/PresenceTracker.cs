namespace API;

public class PresenceTracker
{
    // Dictionary lưu trữ danh sách connectionId theo từng username đang online
    private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

    /// <summary>
    /// Gọi khi một user kết nối mới (mỗi tab hoặc thiết bị sẽ có một connectionId riêng)
    /// </summary>
    /// <param name="username">Tên người dùng đăng nhập</param>
    /// <param name="connectionId">ID kết nối của SignalR</param>
    /// <returns>
    /// Trả về true nếu đây là kết nối đầu tiên của user (user vừa online), ngược lại false
    /// </returns>
    public Task<bool> UserConnected(string username, string connectionId)
    {
        var isOnline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                // Nếu đã có user này trong dictionary, thêm connectionId mới
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                // Lần đầu user kết nối, khởi tạo list và đánh dấu user online
                OnlineUsers.Add(username, new List<string> { connectionId });
                isOnline = true;
            }
        }

        // Trả về kết quả (true nếu user vừa chuyển trạng thái từ offline sang online)
        return Task.FromResult(isOnline);
    }

    /// <summary>
    /// Gọi khi một connection của user bị ngắt (đóng tab, đóng trình duyệt,…) 
    /// </summary>
    /// <param name="username">Tên người dùng</param>
    /// <param name="connectionId">ID kết nối cần xóa</param>
    /// <returns>
    /// Trả về true nếu đây là connection cuối cùng của user (user offline hoàn toàn), ngược lại false
    /// </returns>
    public Task<bool> UserDisconnected(string username, string connectionId)
    {
        var isOffline = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username)) 
                return Task.FromResult(isOffline);

            // Xóa connectionId khỏi danh sách
            OnlineUsers[username].Remove(connectionId);

            if (OnlineUsers[username].Count == 0)
            {
                // Nếu không còn connection nào, xóa user khỏi dictionary và đánh dấu offline
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }

        // Trả về kết quả (true nếu user vừa chuyển trạng thái từ online sang offline)
        return Task.FromResult(isOffline);
    }

    /// <summary>
    /// Lấy danh sách tất cả username đang online, đã sắp xếp theo thứ tự chữ cái
    /// </summary>
    /// <returns>Mảng string chứa tên các user đang online</returns>
    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers
                .OrderBy(k => k.Key)
                .Select(k => k.Key)
                .ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    /// <summary>
    /// Lấy danh sách tất cả connectionId của một user (để gửi notification cụ thể)
    /// </summary>
    /// <param name="username">Tên người dùng</param>
    /// <returns>Danh sách connectionId (List&lt;string&gt;)</returns>
    public static Task<List<string>> GetConnectionsForUser(string username)
    {
        List<string> connectionIds;

        if (OnlineUsers.TryGetValue(username, out var connections))
        {
            lock (connections)
            {
                // Copy ra list mới để tránh xung đột khi sửa chung
                connectionIds = new List<string>(connections);
            }
        }
        else
        {
            // Nếu user không online hoặc không tồn tại key
            connectionIds = new List<string>();
        }

        return Task.FromResult(connectionIds);
    }
}
