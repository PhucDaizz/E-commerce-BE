using AutoMapper;
using Ecommerce.Application.DTOS.ChatMessage;
using Ecommerce.Application.DTOS.Conversation;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ECommerce.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IAuthService _authService;
        private readonly ILogger<ChatHub> _logger;
        private static readonly ConcurrentDictionary<string, string> OnlineAdmins = new ConcurrentDictionary<string, string>();  // Key: AdminUserId, Value: ConnectionId

        public ChatHub(IConversationRepository conversationRepository, 
                        IMapper mapper, 
                        IChatMessageRepository chatMessageRepository, 
                        ILogger<ChatHub> logger, 
                        IAuthRepository authRepository,
                        IAuthService authService)
        {
            _conversationRepository = conversationRepository;
            _mapper = mapper;
            _chatMessageRepository = chatMessageRepository;
            _authRepository = authRepository;
            _authService = authService;
            _logger = logger;
        }


        [Authorize]
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var isAdmin = Context.User.IsInRole("Admin");

            if (Context.User == null || string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized or invalid connection attempt.");
                Context.Abort(); // Ngắt kết nối WebSocket
                return;
            }

            // admin 
            if (isAdmin)
            {
                OnlineAdmins.TryAdd(userId, Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, "AdminsGroup");
                _logger.LogInformation($"Admin connected: {userId}, ConnectionId: {Context.ConnectionId}");

                // Gửi danh sách cuộc hội thoại đang chờ cho admin
                var pendingConversations = await _conversationRepository.GetPendingConversationsForAdminAsync();
                var pendingConversationsDto = _mapper.Map<IEnumerable<ListConversationsDTO>>(pendingConversations);
                await Clients.Caller.SendAsync("ReceivePendingConversations", pendingConversationsDto);

                // Tải lịch sử chat cho các cuộc hội thoại đang hoạt động
                var activeConversationsForThisAdmin = await _conversationRepository.GetActiveConversationsByAdminAsync(userId);

                if (activeConversationsForThisAdmin.Any())
                {
                    _logger.LogInformation("Admin {UserId} has {Count} active conversations to rejoin.", userId, activeConversationsForThisAdmin.Count());
                    var activeConversationInfos = new List<object>();
                    foreach (var conversation in activeConversationsForThisAdmin)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, conversation.ConversationId.ToString());

                        var lastMessage = await _chatMessageRepository.GetLastMessagePreviewAsync(conversation.ConversationId);

                        activeConversationInfos.Add(new
                        {
                            ConversationId = conversation.ConversationId,
                            ClientUserName = conversation.ClientUserName ?? "Unknown",
                            ClientUserId = conversation.ClientUserId,
                            LastMessageContent = lastMessage?.MessageContent ?? "No messages yet",
                            LastMessageUserId = lastMessage?.SenderUserId,
                            LastMessageTime = lastMessage?.SentTimeUtc.ToString("g") ?? "N/A",
                            IsReadByAdmin = lastMessage?.IsReadByAdmin ?? false,
                        });
                    }
                    await Clients.Caller.SendAsync("ReceiveActiveConversationList", activeConversationInfos);
                }

            }
            // client
            else
            {
                var openConversation = await _conversationRepository.GetConversationPedingOrActiveAsync(userId);
                if(openConversation != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, openConversation.ConversationId.ToString());

                    var historyChat = await _chatMessageRepository.GetChatHistoryAsync(openConversation.ConversationId);
                    var historyChatDto = _mapper.Map<IEnumerable<ChatMessageDTO>>(historyChat);
                    await Clients.Caller.SendAsync("LoadChatHistory", openConversation.ConversationId, historyChatDto);


                    if (openConversation.Status == ConversationStatus.Active && openConversation.AdminUserId != null)
                    {
                        await Clients.Caller.SendAsync("AdminJoined", 
                                                            openConversation.ConversationId, 
                                                            openConversation.AdminUserName, 
                                                            openConversation.AdminUserId);

                        if (OnlineAdmins.TryGetValue(openConversation.AdminUserId, out string adminConnectionId))
                        {
                            await Clients.Client(adminConnectionId).SendAsync("ClientReconnected", openConversation.ConversationId, userId);
                        }
                    }
                    else if (openConversation.Status == ConversationStatus.Pending)
                    {
                        await Clients.Caller.SendAsync("ChatStillPending", openConversation.ConversationId);
                    }
                }
                else
                {
                    await Clients.Caller.SendAsync("NoActiveOrPendingConversationFound");
                }
                _logger.LogInformation("Client connected: {UserId}, ConnectionId: {ConnectionId}", userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            var user = Context.User;


            if (user == null || string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("OnDisconnectedAsync: User or UserId is null/empty. Exception: {Exception}", exception?.Message);
                await base.OnDisconnectedAsync(exception);
                return;
            }

            bool isAdmin = user.IsInRole("Admin");
            if (isAdmin)
            {
                if (OnlineAdmins.TryGetValue(userId, out string? storedConnectionId) && storedConnectionId == Context.ConnectionId)
                {
                    OnlineAdmins.TryRemove(userId, out _);
                    _logger.LogInformation("Admin removed from OnlineAdmins: {UserId}", userId);
                }
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminsGroup");
                _logger.LogInformation("Admin disconnected: {UserId}, ConnectionId: {ConnectionId}. Exception: {Exception}", userId, Context.ConnectionId, exception?.Message);
            }

            // Client
            else 
            {
                var activeConversation = await _conversationRepository.GetActiveConversationByClientForDisconnectAsync(userId);

                if (activeConversation != null && !string.IsNullOrEmpty(activeConversation.AdminUserId))
                {
                    if (OnlineAdmins.TryGetValue(activeConversation.AdminUserId, out string? adminConnectionId) && !string.IsNullOrEmpty(adminConnectionId))
                    {
                        await Clients.Client(adminConnectionId).SendAsync("ClientDisconnected", activeConversation.ConversationId, userId);
                        _logger.LogInformation("Notified Admin {AdminUserId} about Client {ClientUserId} disconnection for Conversation {ConversationId}",
                            activeConversation.AdminUserId, userId, activeConversation.ConversationId);
                    }
                }
                _logger.LogInformation("Client disconnected: {UserId}, ConnectionId: {ConnectionId}. Exception: {Exception}", userId, Context.ConnectionId, exception?.Message);
            }
            await base.OnDisconnectedAsync(exception);
        }


        public async Task ClientRequestChat(string initialMessage)
        {
            var clientUserId = Context.UserIdentifier;
            var user = Context.User;

            if (user == null || string.IsNullOrEmpty(clientUserId) || user.IsInRole("Admin"))
            {
                _logger.LogWarning("ClientRequestChat: Invalid request from UserId {UserId}", clientUserId);
                await Clients.Caller.SendAsync("ChatError", "Invalid request or you are Admin."); 
                return;
            }

            var client = await _authRepository.GetInforAsync(clientUserId);

            var existingConversation = await _conversationRepository.GetPendingOrActiveConversationByClientAsync(clientUserId);
            Conversations conversationToProcess;
            Guid conversationId;

            if (existingConversation != null)
            {
                conversationId = existingConversation.ConversationId;
                conversationToProcess = existingConversation;

                if (existingConversation.Status == ConversationStatus.Active && !string.IsNullOrEmpty(existingConversation.AdminUserId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
                    var history = await _chatMessageRepository.GetChatHistoryAsync(conversationId);
                    await Clients.Caller.SendAsync("LoadChatHistory", conversationId, _mapper.Map<IEnumerable<ChatMessageDTO>>(history));
                
                    var adminUserName = "Admin";
                    await Clients.Caller.SendAsync("AdminJoined", conversationId, adminUserName ?? "Admin", existingConversation.AdminUserId);


                    if (OnlineAdmins.TryGetValue(existingConversation.AdminUserId, out string? adminConnectionId) && !string.IsNullOrEmpty(adminConnectionId))
                    {
                        await Clients.Client(adminConnectionId).SendAsync("ClientReconnected", conversationId, clientUserId);
                    }

                    _logger.LogInformation("ClientRequestChat: Client {ClientUserId} rejoining active conversation {ConversationId}", clientUserId, conversationId);
                    return;
                }
                else if (existingConversation.Status == ConversationStatus.Pending)
                {
                    _logger.LogInformation("ClientRequestChat: Client {ClientUserId} rejoining PENDING conversation {ConversationId}", clientUserId, conversationId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

                    // Load lại lịch sử chat pending cho client
                    var history = await _chatMessageRepository.GetChatHistoryAsync(conversationId);
                    await Clients.Caller.SendAsync("LoadChatHistory", conversationId, _mapper.Map<IEnumerable<ChatMessageDTO>>(history));
                    await Clients.Caller.SendAsync("ChatStillPending", conversationId);
                    return; 
                }
            }
            else
            {
                var newConversation = new Conversations
                {
                    ConversationId = Guid.NewGuid(),
                    ClientUserId = clientUserId,
                    Status = ConversationStatus.Pending,
                    StartTimeUtc = DateTime.UtcNow,
                    LastActivityTimeUtc = DateTime.UtcNow
                };

                await _conversationRepository.AddAsync(newConversation);
                conversationId = newConversation.ConversationId;
                conversationToProcess = newConversation;
                _logger.LogInformation("ClientRequestChat: New conversation {ConversationId} created for Client {ClientUserId}", conversationId, clientUserId);
            }


            if (!string.IsNullOrEmpty(initialMessage))
            {
                var chatMessage = new ChatMessage
                {
                    ConversationId = conversationId,
                    SenderUserId = clientUserId,
                    MessageContent = initialMessage,
                    SentTimeUtc = DateTime.UtcNow
                };
                await _chatMessageRepository.AddAsync(chatMessage);
                conversationToProcess.LastActivityTimeUtc = DateTime.UtcNow;
                await _conversationRepository.UpdateAsync(conversationToProcess); 
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
            await Clients.Caller.SendAsync("ChatRequestSent", conversationId);
            var userInfo = await _authService.GetInforAsync(clientUserId);

            var conversationDto = _mapper.Map<ListConversationsDTO>(conversationToProcess);
            conversationDto.UserName = userInfo.UserName;
            conversationDto.InitialMessage = initialMessage;

            _logger.LogInformation("ClientRequestChat: Sending NewChatRequest to AdminsGroup. ConversationDto: {@ConversationDto}", conversationDto);
            _logger.LogInformation("ClientRequestChat: AdminsGroup members count: {Count}", OnlineAdmins.Count);
            if (OnlineAdmins.IsEmpty)
            {
                _logger.LogWarning("ClientRequestChat: No admins online to receive NewChatRequest for conversation {ConversationId}", conversationId);
            }
            else
            {
                _logger.LogInformation("ClientRequestChat: Online admins: {AdminIds}", string.Join(", ", OnlineAdmins.Keys));
            }
            await Clients.Group("AdminsGroup").SendAsync("NewChatRequest", conversationDto);
        }


        [Authorize(Roles = "Admin")]
        public async Task AdminAcceptChat(Guid conversationId)
        {
            var adminUserId = Context.UserIdentifier;
            var adminName = "Admin";

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if (conversation == null)
            {
                _logger.LogWarning("AdminAcceptChat: Conversation {ConversationId} not found. Admin: {AdminUserId}", conversationId, adminUserId);
                await Clients.Caller.SendAsync("ChatError", "Cuộc hội thoại không tồn tại.");
                return;
            }

            if (conversation.Status == ConversationStatus.Active && !string.IsNullOrEmpty(conversation.AdminUserId) && conversation.AdminUserId != adminUserId)
            {
                var admin = await _authRepository.GetInforAsync(conversation.AdminUserId);
                _logger.LogInformation("AdminAcceptChat: Conversation {ConversationId} already taken by {CurrentAdmin}. Requester: {AdminUserId}", conversationId, conversation.AdminUserId, adminUserId);
                await Clients.Caller.SendAsync("ChatError", $"Cuộc hội thoại đã được admin {admin.UserName ?? "khác"} nhận.");
                return;
            }

            if (conversation.Status == ConversationStatus.Closed)
            {
                _logger.LogInformation("AdminAcceptChat: Conversation {ConversationId} is closed. Admin: {AdminUserId}", conversationId, adminUserId);
                await Clients.Caller.SendAsync("ChatError", "Cuộc hội thoại này đã đóng.");
                return;
            }

            // Cập nhật thông tin cuộc trò chuyện
            await _conversationRepository.AcceptChatAsync(conversationId, adminUserId);

            // Thêm admin vào nhóm trò chuyện
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

            // Gửi thông báo tới các nhóm
            await Clients.Group(conversationId.ToString()).SendAsync("AdminJoined", conversationId, adminName, adminUserId);
            await Clients.GroupExcept("AdminsGroup", Context.ConnectionId).SendAsync("ChatAcceptedByOther", conversationId, adminName, adminUserId);
            await Clients.Caller.SendAsync("ChatAcceptedByYou", conversationId);

            // Lấy lịch sử chat và gửi về cho admin
            var history = await _chatMessageRepository.GetChatHistoryAsync(conversationId);
            var mappedHistory = _mapper.Map<IEnumerable<ChatMessageDTO>>(history);
/*            await Clients.Caller.SendAsync("LoadChatHistory", conversationId, mappedHistory);
*/            await Clients.Group(conversationId.ToString()).SendAsync("LoadChatHistory", conversationId, mappedHistory);

            _logger.LogInformation("AdminAcceptChat: Admin {AdminUserId} accepted Conversation {ConversationId}", adminUserId, conversationId);

            // Gửi cuộc trò chuyện này sang danh sách đang hoạt động
            var lastMessage = history
                ?.OrderByDescending(x => x.SentTimeUtc)
                .FirstOrDefault();

            var activeConversationInfo = new
            {
                ConversationId = conversation.ConversationId,
                ClientUserName = conversation.ClientUserName ?? "Unknown",
                ClientUserId = conversation.ClientUserId,
                LastMessageContent = lastMessage?.MessageContent ?? "No messages yet",
                LastMessageTime = lastMessage?.SentTimeUtc.ToString("g") ?? "N/A",
                IsReadByAdmin = lastMessage?.IsReadByAdmin ?? false,
            };
            await Clients.Caller.SendAsync("AddToActiveConversationList", activeConversationInfo);
        }


        [Authorize(Roles = "Admin")]
        public async Task AdminRequestChatHistory(Guid conversationId)
        {
            var adminUserId = Context.UserIdentifier;

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null || conversation.AdminUserId != adminUserId || conversation.Status != ConversationStatus.Active)
            {
                _logger.LogWarning("Admin {AdminId} requested history for invalid/unauthorized conversation {ConversationId}", adminUserId, conversationId);
                await Clients.Caller.SendAsync("ChatError", "Không thể tải lịch sử cho cuộc trò chuyện này.");
                return;
            }

            var history = await _chatMessageRepository.GetChatHistoryAsync(conversationId);
            var historyDto = _mapper.Map<IEnumerable<ChatMessageDTO>>(history);
            await Clients.Caller.SendAsync("LoadChatHistory", conversationId, historyDto);
            _logger.LogInformation("Admin {AdminId} requested and received history for conversation {ConversationId}", adminUserId, conversationId);

        }

        [Authorize]
        public async Task MarkMessagesAsRead(string conversationId)
        {
            var userId = Context.UserIdentifier;
            await Clients.Group(conversationId).SendAsync("ReceiveReadReceipt", conversationId, userId, DateTime.UtcNow);
            await _chatMessageRepository.MarkMessagesAsReadAsync(Guid.Parse(conversationId), userId, Context.User.IsInRole("Admin"));
        }


        public async Task SendMessage(Guid conversationId, string messageContent)
        {
            var senderUserId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(senderUserId) || string.IsNullOrWhiteSpace(messageContent))
            {
                await Clients.Caller.SendAsync("ChatError", "Nội dung tin nhắn không hợp lệ.");
                return;
            }

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null)
            {
                _logger.LogWarning("SendMessage: Conversation {ConversationId} not found for User {UserId}", conversationId, senderUserId);
                await Clients.Caller.SendAsync("ChatError", "Cuộc hội thoại không tồn tại.");
                return;
            }

            bool isSenderClient = conversation.ClientUserId == senderUserId;
            bool isSenderAssignedAdmin = conversation.AdminUserId == senderUserId;

            if (!isSenderClient && !isSenderAssignedAdmin)
            {
                _logger.LogWarning("SendMessage: User {UserId} not authorized for Conversation {ConversationId}", senderUserId, conversationId);
                await Clients.Caller.SendAsync("ChatError", "Bạn không có quyền gửi tin nhắn vào cuộc hội thoại này.");
                return;
            }

            if (conversation.Status != ConversationStatus.Active)
            {
                _logger.LogInformation("SendMessage: Attempt to send message to non-active Conversation {ConversationId} by User {UserId}. Status: {Status}",
                    conversationId, senderUserId, conversation.Status);
                await Clients.Caller.SendAsync("ChatError", "Cuộc hội thoại này chưa được kích hoạt hoặc đã đóng.");
                return;
            }


            var isAdmin = Context.User.IsInRole("Admin");
            var senderInfor = await _authRepository.GetInforAsync(senderUserId);

            var message = new ChatMessage
            {
                ConversationId = conversationId,
                SenderUserId = senderUserId,
                MessageContent = messageContent,
                SentTimeUtc = DateTime.UtcNow,
                IsReadByClient = !isAdmin,
                IsReadByAdmin = isAdmin
            };

            await _chatMessageRepository.AddAsync(message);
            await _conversationRepository.UpdateLastActivityTimeAsync(conversationId);

            var messageDto = _mapper.Map<ChatMessageDTO>(message);
            messageDto.SenderName = senderInfor?.UserName ?? "User";

            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", messageDto);
            _logger.LogInformation("SendMessage: Message sent in Conversation {ConversationId} by User {UserId}", conversationId, senderUserId);
        }

        [Authorize(Roles = "Admin")]
        public async Task AdminCloseChat(Guid conversationId)
        {
            var adminUserId = Context.UserIdentifier;
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if (conversation == null)
            {
                _logger.LogWarning("AdminCloseChat: Conversation {ConversationId} not found. Admin: {AdminUserId}", conversationId, adminUserId);
                await Clients.Caller.SendAsync("ChatError", "Cuộc hội thoại không tồn tại.");
                return;
            }

            if (conversation.AdminUserId != adminUserId && !string.IsNullOrEmpty(conversation.AdminUserId))
            {
                _logger.LogWarning("AdminCloseChat: Admin {AdminUserId} not authorized to close Conversation {ConversationId} (assigned to {AssignedAdminId})",
                adminUserId, conversationId, conversation.AdminUserId);
                await Clients.Caller.SendAsync("ChatError", "Bạn không phải admin phụ trách cuộc hội thoại này.");
                return;
            }

            if (conversation.Status == ConversationStatus.Closed)
            {
                _logger.LogInformation("AdminCloseChat: Conversation {ConversationId} already closed. Admin: {AdminUserId}", conversationId, adminUserId);
                await Clients.Caller.SendAsync("ChatInfo", "Cuộc hội thoại này đã được đóng trước đó.");
                return;
            }

            var updateResult = await _conversationRepository.CloseChatAsync(conversationId);
            if (!updateResult)
            {
                await Clients.Caller.SendAsync("ChatError", "Không thể đóng cuộc hội thoại này.");
                return;
            }

            var admin = await _authRepository.GetInforAsync(adminUserId);
            await Clients.Group(conversationId.ToString()).SendAsync("ChatClosed", conversationId, $"Cuộc hội thoại đã được đóng bởi {admin.UserName}.");
            _logger.LogInformation("AdminCloseChat: Conversation {ConversationId} closed by Admin {AdminUserId}", conversationId, adminUserId);

        }
    }
}
