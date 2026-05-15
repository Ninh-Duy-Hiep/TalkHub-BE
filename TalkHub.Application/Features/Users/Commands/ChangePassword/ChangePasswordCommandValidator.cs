using FluentValidation;

namespace TalkHub.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Mật khẩu cũ không được để trống.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự.")
            .Matches("[A-Z]").WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ cái viết hoa.")
            .Matches("[a-z]").WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ cái viết thường.")
            .Matches("[0-9]").WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ số.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu mới phải chứa ít nhất 1 ký tự đặc biệt.")
            .NotEqual(x => x.OldPassword).WithMessage("Mật khẩu mới phải khác mật khẩu cũ.");
    }
}
