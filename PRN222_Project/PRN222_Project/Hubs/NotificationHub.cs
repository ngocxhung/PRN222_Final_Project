using Microsoft.AspNetCore.SignalR;

namespace SignalRSample.Hubs
{
    public class NotificationHub : Hub
    {
        // Danh sách thông báo được lưu trữ tĩnh
        private static List<string> _messages = new List<string>();
        private static int _notificationCounter = 0;

        public async Task SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            // Giới hạn độ dài thông báo
            message = message.Length > 500 ? message.Substring(0, 500) : message;

            // Thêm thông báo mới
            _messages.Add(message);
            _notificationCounter++;

            // Giới hạn số lượng thông báo
            if (_messages.Count > 50)
                _messages.RemoveAt(0);

            // Gửi thông báo tới tất cả client
            await Clients.All.SendAsync("ReceiveNotification", message, _notificationCounter);
        }

        public async Task GetNotifications()
        {
            // Trả về danh sách thông báo hiện tại
            await Clients.Caller.SendAsync("LoadNotification", _messages, _notificationCounter);
        }
    }
}