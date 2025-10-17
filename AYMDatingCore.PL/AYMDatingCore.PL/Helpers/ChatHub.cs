using AYMDatingCore.BLL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AYMDatingCore.Helpers
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendMessageToGroup(string groupName, string message, string SenderUserName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", message , SenderUserName );
        }
        public async Task SendAudioToGroup(string groupName, string senderUserName, string base64Audio)
        {
            // Send to all clients in the group
            await Clients.Group(groupName).SendAsync("ReceiveAudioFromGroup", senderUserName, base64Audio);
        }
        //public async Task GetLikeNotification(string userRecieverId)
        //{
        //    await Clients.User(userRecieverId).SendAsync("ReceiveLikeNotification");
        //}
        //public async Task GetViewNotification(string userRecieverId)
        //{
        //    await Clients.User(userRecieverId).SendAsync("ReceiveViewNotification");
        //}
        //public async Task GetMessageNotification(string userRecieverId)
        //{
        //    await Clients.User(userRecieverId).SendAsync("ReceiveMessageNotification");
        //}
        //public async Task GetFavoriteNotification(string userRecieverId)
        //{
        //    await Clients.User(userRecieverId).SendAsync("ReceiveFavoriteNotification");
        //}
        //public async Task GetBlockNotification(string userRecieverId)
        //{
        //    await Clients.User(userRecieverId).SendAsync("ReceiveBlockNotification");
        //}
    }
}
