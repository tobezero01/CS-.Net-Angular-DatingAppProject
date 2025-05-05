using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    void AddMessage(Message message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    void DeleteMessage(Message message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Message?> GetMessage(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageParams"></param>
    /// <returns></returns>
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentUsername"></param>
    /// <param name="recipientUsername"></param>
    /// <returns></returns>
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    void AddGroup(Group group);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    void RemoveConnection(Connection connection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    Task<Connection?> GetConnection(string connectionId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    Task<Group?> GetMessageGroup(string groupName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    Task<Group?> GetGroupForConnection(string connectionId);
}
