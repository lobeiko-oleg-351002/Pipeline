using BllEntities;
using ServerInterface;

namespace Client.ServerManager.Interface
{
    public interface IAuthorizationManager
    {
        BllUser ConnectToServerAndSignIn(string ip, IClientCallBack clientCallBack, BllUser user);
        BllUser SignIn(BllUser User);
    }
}
