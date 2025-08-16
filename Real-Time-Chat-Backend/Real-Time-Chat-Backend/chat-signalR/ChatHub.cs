using Microsoft.AspNetCore.SignalR;
using Real_Time_Chat_Backend.Services;

namespace Real_Time_Chat_Backend.chat_signalR
{
    public class ChatHub : Hub
    {
        private readonly FreeAIService _freeAIService;

        public ChatHub(FreeAIService freeAIService)
        {
            _freeAIService = freeAIService;
        }

        // Method to send a message and get AI response
        public async Task SendMessage(string user, string message)
        {
            // Send the user's message to all clients
           // await Clients.All.SendAsync("ReceiveMessage", user, message);
            
            // Get AI response from free AI service
            string aiResponse = await _freeAIService.GetChatResponseAsync(message);
            await Clients.All.SendAsync("ReceiveMessage", "AI Assistant", aiResponse);
        }

        // (Optional) Method to send a message to a specific group (chat room)
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }

        // (Optional) Join/Leave Group functionality for chat rooms:
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveNotification", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveNotification", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}

