//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IMessageAdminService
    {
        Task<AMessage_MessageList> GetMessagesAsync(MCommunication_MessageMailbox mailbox, MCommunication_MessageStatus status, int recordCount);
        Task<AMessage_Message> GetMessageAsync(long messageId);
        Task<long> SendOutboundMessageAsync(string userId, string subject, string text, long? replyToMessageId, long? orderId);
        Task AcknowledgeMessageAsync(long messageId);
    }
}
