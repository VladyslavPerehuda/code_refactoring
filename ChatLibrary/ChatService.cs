using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        private readonly Dictionary<string, IClientCallback> _clients = new Dictionary<string, IClientCallback>();


        public bool Connect(string userName)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            if (_clients.ContainsKey(userName))
                return false;

            _clients[userName] = callback;
            UpdateUsers();
            return true;
        }

        public void Disconnect(string userName)
        {
            if (_clients.ContainsKey(userName))
            {
                _clients.Remove(userName);
                UpdateUsers();
            }
        }

        public void SendMessage(string fromUser, string message)
        {
            foreach (var client in _clients.Values)
            {
                client.ReceiveMessage(fromUser, message);
            }
        }

        public void SendPrivateMessage(string fromUser, string toUser, string message)
        {
            if (_clients.ContainsKey(toUser))
            {
                _clients[toUser].ReceiveMessage(fromUser, $"(Приватно): {message}");
            }
        }

        private void UpdateUsers()
        {
            var users = _clients.Keys.ToArray();
            foreach (var client in _clients.Values)
            {
                client.UpdateUserList(users);
            }
        }
    }
}
