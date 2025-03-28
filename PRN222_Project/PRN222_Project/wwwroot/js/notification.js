(function () {
    // Khai báo các biến toàn cục
    var connectionNotification = null;
    var sendButton = null;
    var notificationInput = null;
    var messageList = null;
    var notificationCounter = null;

    // Hàm khởi tạo
    function initializeNotification() {
        // Lấy các phần tử DOM
        sendButton = document.getElementById("sendButton");
        notificationInput = document.getElementById("notificationInput");
        messageList = document.getElementById("messageList");
        notificationCounter = document.getElementById("notificationCounter");

        // Kiểm tra các phần tử tồn tại
        if (!sendButton || !notificationInput || !messageList || !notificationCounter) {
            console.error("Không tìm thấy một hoặc nhiều phần tử cần thiết");
            return;
        }

        // Tạo kết nối SignalR
        connectionNotification = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/notification")
            .withAutomaticReconnect()
            .build();

        // Đăng ký sự kiện nhận thông báo
        connectionNotification.on("ReceiveNotification", function (message, counter) {
            // Thêm thông báo mới vào đầu danh sách
            addNewNotification(message, counter);
        });

        // Đăng ký sự kiện load danh sách thông báo ban đầu
        connectionNotification.on("LoadNotification", function (messages, counter) {
            renderNotifications(messages, counter);
        });

        // Khởi động kết nối
        startConnection();

        // Thiết lập sự kiện cho nút gửi
        setupSendButton();
    }

    // Bắt đầu kết nối SignalR
    function startConnection() {
        connectionNotification.start()
            .then(function () {
                console.log("SignalR Connected");
                // Yêu cầu tải tin nhắn ban đầu
                connectionNotification.invoke("GetNotifications");

                // Kích hoạt nút gửi
                if (sendButton) {
                    sendButton.disabled = false;
                }
            })
            .catch(function (err) {
                console.error("Lỗi kết nối SignalR:", err);
                // Thử kết nối lại sau 5 giây
                setTimeout(startConnection, 5000);
            });
    }

    // Thêm thông báo mới
    function addNewNotification(message, counter) {
        if (!messageList || !notificationCounter) return;

        // Tạo phần tử thông báo mới
        var div = document.createElement("div");
        div.className = "dropdown-item";
        div.textContent = message;

        // Thêm vào đầu danh sách
        if (messageList.firstChild) {
            messageList.insertBefore(div, messageList.firstChild);
        } else {
            messageList.appendChild(div);
        }

        // Cập nhật số lượng thông báo
        notificationCounter.textContent = counter > 0 ? counter : "";
    }

    // Render danh sách thông báo ban đầu
    function renderNotifications(messages, counter) {
        if (!messageList || !notificationCounter) return;

        // Làm sạch danh sách thông báo
        messageList.innerHTML = "";

        // Cập nhật số lượng thông báo
        notificationCounter.textContent = counter > 0 ? counter : "";

        // Hiển thị thông báo
        if (messages.length === 0) {
            var noMessage = document.createElement("div");
            noMessage.className = "dropdown-item text-muted";
            noMessage.textContent = "Không có thông báo";
            messageList.appendChild(noMessage);
        } else {
            // Hiển thị các thông báo
            messages.slice().reverse().forEach(function (message) {
                var div = document.createElement("div");
                div.className = "dropdown-item";
                div.textContent = message;
                messageList.appendChild(div);
            });
        }
    }

    // Thiết lập sự kiện cho nút gửi
    function setupSendButton() {
        if (!sendButton || !notificationInput) return;

        // Vô hiệu hóa nút ban đầu
        sendButton.disabled = true;

        // Kích hoạt/vô hiệu hóa nút dựa trên nội dung input
        notificationInput.addEventListener('input', function () {
            sendButton.disabled = this.value.trim() === '';
        });

        // Sự kiện click nút gửi
        sendButton.addEventListener("click", function (event) {
            event.preventDefault();

            var message = notificationInput.value.trim();
            if (!message) return;

            // Gửi thông báo
            connectionNotification.invoke("SendMessage", message)
                .then(function () {
                    console.log("Thông báo gửi thành công");
                    notificationInput.value = "";
                    sendButton.disabled = true;
                })
                .catch(function (err) {
                    console.error("Lỗi gửi thông báo:", err);
                    alert("Không thể gửi thông báo. Vui lòng thử lại.");
                });
        });

        // Hỗ trợ gửi bằng phím Enter
        notificationInput.addEventListener("keypress", function (event) {
            if (event.key === "Enter" && !sendButton.disabled) {
                sendButton.click();
            }
        });
    }

    // Chạy hàm khởi tạo khi trang tải xong
    document.addEventListener('DOMContentLoaded', initializeNotification);
})();