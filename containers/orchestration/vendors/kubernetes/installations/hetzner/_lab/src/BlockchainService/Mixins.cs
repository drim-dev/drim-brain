using MediatR;

namespace BlockchainService;

public sealed partial class WithdrawRequest : IRequest<WithdrawReply>;
