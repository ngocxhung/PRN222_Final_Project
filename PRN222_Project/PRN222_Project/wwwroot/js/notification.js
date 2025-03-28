(function () {
    // Khai báo các biến toàn cục
    var connectionNotification = null;
    var sendButton = null;
    var notificationInput = null;
    var messageList = null;
    var notificationCounter = null;
    var departmentSelect = null;
    var sendTypeSelect = null;

    // Hàm khởi tạo
    function initializeNotification() {
        // Lấy các phần tử DOM
        sendButton = document.getElementById("sendButton");
        notificationInput = document.getElementById("notificationInput");
        messageList = document.getElementById("messageList");
        notificationCounter = document.getElementById("notificationCounter");
        departmentSelect = document.getElementById("departmentSelect");
        sendTypeSelect = document.getElementById("sendTypeSelect");

        // Kiểm tra các phần tử cần thiết cho layout (messageList và notificationCounter luôn có trong layout)
        if (!messageList || !notificationCounter) {
            console.error("Không tìm thấy messageList hoặc notificationCounter trong layout:", {
                messageList, notificationCounter
            });
            return;
        }

        // Kiểm tra nếu kết nối đã tồn tại thì không khởi tạo lại
        if (connectionNotification && connectionNotification.state === signalR.HubConnectionState.Connected) {
            console.log("SignalR connection already exists and is connected.");
            // Tải lại thông báo nếu cần
            connectionNotification.invoke("GetNotificationsForDepartment")
                .catch(function (err) {
                    console.error("Error reloading initial notifications:", err.toString());
                });
            return;
        }

        // Tạo kết nối SignalR
        connectionNotification = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/notification")
            .configureLogging(signalR.LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        // Đăng ký sự kiện nhận thông báo
        connectionNotification.on("ReceiveNotification", function (message, counter) {
            console.log("Received notification:", { message, counter });
            addNewNotification(message, counter);
        });

        // Đăng ký sự kiện load danh sách thông báo ban đầu
        connectionNotification.on("LoadNotification", function (messages, counter) {
            console.log("Load notifications:", { messages, counter });
            renderNotifications(messages, counter);
        });

        // Khởi động kết nối
        startConnection();

        // Thiết lập sự kiện cho nút gửi (chỉ áp dụng trên trang có form gửi thông báo)
        if (sendButton && notificationInput && sendTypeSelect && departmentSelect) {
            setupSendButton();
            setupSendTypeChange();
        }
    }

    // Bắt đầu kết nối SignalR
    function startConnection() {
        console.log("Starting SignalR connection...");
        connectionNotification.start()
            .then(function () {
                console.log("SignalR Connected successfully");
                if (sendButton) {
                    sendButton.disabled = false;
                }
                // Yêu cầu tải tin nhắn ban đầu
                console.log("Invoking GetNotificationsForDepartment...");
                connectionNotification.invoke("GetNotificationsForDepartment")
                    .catch(function (err) {
                        console.error("Error getting initial notifications:", err.toString());
                    });
            })
            .catch(function (err) {
                console.error("SignalR connection failed:", err.toString());
                setTimeout(startConnection, 5000);
            });
    }

    // Thiết lập sự kiện thay đổi kiểu gửi
    function setupSendTypeChange() {
        sendTypeSelect.addEventListener('change', function () {
            console.log("Send type changed to:", this.value);
            departmentSelect.disabled = this.value === 'all';
        });
    }

    // Thêm thông báo mới
    function addNewNotification(message, counter) {
        if (!messageList || !notificationCounter) return;

        console.log("Adding new notification:", { message, counter });
        var div = document.createElement("div");
        div.className = "dropdown-item";
        div.textContent = message;

        if (messageList.firstChild) {
            messageList.insertBefore(div, messageList.firstChild);
        } else {
            messageList.appendChild(div);
        }

        notificationCounter.textContent = counter > 0 ? counter : "";
    }

    // Render danh sách thông báo ban đầu
    function renderNotifications(messages, counter) {
        if (!messageList || !notificationCounter) return;

        console.log("Rendering notifications:", { messages, counter });
        messageList.innerHTML = "";
        notificationCounter.textContent = counter > 0 ? counter : "";

        if (messages.length === 0) {
            var noMessage = document.createElement("div");
            noMessage.className = "dropdown-item text-muted";
            noMessage.textContent = "Không có thông báo";
            messageList.appendChild(noMessage);
        } else {
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
        if (!sendButton || !notificationInput || !sendTypeSelect || !departmentSelect) return;

        sendButton.disabled = true;

        notificationInput.addEventListener('input', function () {
            sendButton.disabled = this.value.trim() === '';
        });

        sendButton.addEventListener("click", function (event) {
            event.preventDefault();

            var message = notificationInput.value.trim();
            if (!message) return;

            var sendType = sendTypeSelect.value;
            var departmentId = sendType === 'department' ? parseInt(departmentSelect.value) : null;

            console.log("Sending notification:", { message, sendType, departmentId });

            var sendMethod = sendType === 'all' ?
                connectionNotification.invoke("SendMessageToAll", message) :
                connectionNotification.invoke("SendMessageToDepartment", message, departmentId);

            sendMethod
                .then(function () {
                    console.log("Thông báo gửi thành công");
                    notificationInput.value = "";
                    sendButton.disabled = true;
                })
                .catch(function (err) {
                    console.error("Lỗi gửi thông báo:", err.toString());
                    alert("Không thể gửi thông báo. Vui lòng thử lại.");
                });
        });

        notificationInput.addEventListener("keypress", function (event) {
            if (event.key === "Enter" && !sendButton.disabled) {
                sendButton.click();
            }
        });
    }

    // Chạy hàm khởi tạo khi trang tải xong
    document.addEventListener('DOMContentLoaded', initializeNotification);

    // Lắng nghe sự kiện popstate để xử lý khi người dùng chuyển trang (dùng history API)
    window.addEventListener('popstate', function () {
        console.log("Page changed via popstate, reinitializing notifications...");
        initializeNotification();
    });
})();