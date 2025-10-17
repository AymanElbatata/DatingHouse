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
        public async Task SendMessageToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", message);
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
