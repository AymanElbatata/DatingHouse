using AYMDatingCore.BLL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AYMDatingCore.Helpers
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task GetLikeNotification(string userRecieverId)
        {
            await Clients.User(userRecieverId).SendAsync("ReceiveLikeNotification",1);
        }
        public async Task GetViewNotification(string userRecieverId)
        {
            await Clients.User(userRecieverId).SendAsync("ReceiveViewNotification");
        }
        public async Task GetMessageNotification(string userRecieverId)
        {
            await Clients.User(userRecieverId).SendAsync("ReceiveMessageNotification");
        }
        public async Task GetFavoriteNotification(string userRecieverId)
        {
            await Clients.User(userRecieverId).SendAsync("ReceiveFavoriteNotification");
        }
        public async Task GetBlockNotification(string userRecieverId)
        {
            await Clients.User(userRecieverId).SendAsync("ReceiveBlockNotification");
        }
    }


}
