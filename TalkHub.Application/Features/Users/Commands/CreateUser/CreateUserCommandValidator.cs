using FluentValidation;
using TalkHub.Application.Interfaces.IRepository;

namespace TalkHub.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
            .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự.")
            .MustAsync(async (username, cancellation) => !await _userRepository.ExistsAsync(username))
            .WithMessage("Tên đăng nhập đã tồn tại.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.")
            .MustAsync(async (email, cancellation) => !await _userRepository.ExistsByEmailAsync(email))
            .WithMessage("Email này đã được sử dụng.");

        RuleFor(x => x.PhoneNumber)
            .MustAsync(async (phone, cancellation) =>
            {
                if (string.IsNullOrEmpty(phone)) return true;
                return !await _userRepository.ExistsByPhoneAsync(phone);
            })
            .WithMessage("Số điện thoại này đã được sử dụng.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
            .Matches("[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ cái viết hoa.")
            .Matches("[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ cái viết thường.")
            .Matches("[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt (VD: @, #, $, !...).");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.");
    }
}