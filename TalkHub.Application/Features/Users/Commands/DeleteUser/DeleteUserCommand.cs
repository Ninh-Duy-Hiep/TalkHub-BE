using MediatR;
using TalkHub.Application.Interfaces.IRepository;

namespace TalkHub.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Unit>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");
        
        await _userRepository.DeleteAsync(user);
        
        return Unit.Value;
    }
}
