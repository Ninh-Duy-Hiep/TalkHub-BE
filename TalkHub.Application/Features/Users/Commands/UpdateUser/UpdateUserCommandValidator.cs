using FluentValidation;
using TalkHub.Application.Interfaces.IRepository;

namespace TalkHub.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.")
            .MustAsync(async (command, email, cancellation) => 
                !await _userRepository.ExistsByEmailAsync(email, command.Id))
            .WithMessage("Email này đã được sử dụng bởi một người dùng khác.");

        RuleFor(x => x.PhoneNumber)
            .MustAsync(async (command, phone, cancellation) =>
            {
                if (string.IsNullOrEmpty(phone)) return true;
                return !await _userRepository.ExistsByPhoneAsync(phone, command.Id);
            })
            .WithMessage("Số điện thoại này đã được sử dụng bởi một người dùng khác.");
    }
}
