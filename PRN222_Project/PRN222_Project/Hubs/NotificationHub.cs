using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PRN222_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PRN222_Project.Hubs
{
    public class NotificationMessage
    {
        public string Message { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly QuanLyNhanSuContext _context;
        private static readonly List<NotificationMessage> _notifications = new List<NotificationMessage>();

        public NotificationHub(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        public async Task GetNotificationsForDepartment()
        {
            try
            {
                if (!Context.User.Identity.IsAuthenticated)
                {
                    throw new HubException("User is not authenticated.");
                }

                var userIdClaim = Context.User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new HubException("UserId not found in claims.");
                }

                var userId = int.Parse(userIdClaim);
                var user = await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new HubException("User not found.");
                }

                var notifications = _notifications
                    .Where(n => user.DepartmentId == null || n.DepartmentId == null || n.DepartmentId == user.DepartmentId)
                    .Select(n => n.Message)
                    .ToList();

                int counter = notifications.Count;
                await Clients.Caller.SendAsync("LoadNotification", notifications, counter);
            }
            catch (Exception ex)
            {
                throw new HubException($"Error in GetNotificationsForDepartment: {ex.Message}");
            }
        }

        public async Task SendMessageToAll(string message)
        {
            try
            {
                var role = Context.User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "Admin")
                {
                    throw new HubException("You do not have permission to send notifications.");
                }

                var notification = new NotificationMessage
                {
                    Message = message,
                    DepartmentId = null,
                    CreatedAt = DateTime.Now
                };
                _notifications.Add(notification);

                await Clients.All.SendAsync("ReceiveNotification", message, _notifications.Count);
            }
            catch (Exception ex)
            {
                throw new HubException($"Error in SendMessageToAll: {ex.Message}");
            }
        }

        public async Task SendMessageToDepartment(string message, int departmentId)
        {
            try
            {
                var role = Context.User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "Admin")
                {
                    throw new HubException("You do not have permission to send notifications.");
                }

                var notification = new NotificationMessage
                {
                    Message = message,
                    DepartmentId = departmentId,
                    CreatedAt = DateTime.Now
                };
                _notifications.Add(notification);

                var userConnections = await _context.Users
                    .Where(u => u.DepartmentId == departmentId)
                    .Select(u => u.Id.ToString())
                    .ToListAsync();

                await Clients.Users(userConnections).SendAsync("ReceiveNotification", message, _notifications.Count(n => n.DepartmentId == null || n.DepartmentId == departmentId));
            }
            catch (Exception ex)
            {
                throw new HubException($"Error in SendMessageToDepartment: {ex.Message}");
            }
        }
    }
}