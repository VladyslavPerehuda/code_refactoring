using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IChatService
    {
        [OperationContract]
        bool Connect(string userName);

        [OperationContract]
        void Disconnect(string userName);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string fromUser, string message);

        [OperationContract(IsOneWay = true)]
        void SendPrivateMessage(string fromUser, string toUser, string message);
    }

    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string fromUser, string message);

        [OperationContract(IsOneWay = true)]
        void UpdateUserList(string[] users);
    }
}
