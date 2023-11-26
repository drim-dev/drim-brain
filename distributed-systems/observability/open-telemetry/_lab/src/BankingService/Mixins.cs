using MediatR;

namespace BankingService;

public sealed partial class CreateWithdrawalRequest : IRequest<CreateWithdrawalReply>;
