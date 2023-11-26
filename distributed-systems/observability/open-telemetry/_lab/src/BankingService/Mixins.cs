using MediatR;

namespace BankingService;

public sealed partial class CreateWithdrawalRequest : IRequest<CreateWithdrawalReply>;

public sealed partial class ListWithdrawalsRequest : IRequest<ListWithdrawalsReply>;
