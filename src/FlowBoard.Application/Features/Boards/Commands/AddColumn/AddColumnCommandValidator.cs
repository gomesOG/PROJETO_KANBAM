using FluentValidation;

namespace FlowBoard.Application.Features.Boards.Commands.AddColumn;

public class AddColumnCommandValidator : AbstractValidator<AddColumnCommand>
{
    public AddColumnCommandValidator()
    {
        RuleFor(x => x.BoardId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da coluna é obrigatório.")
            .MaximumLength(60).WithMessage("Nome deve ter no máximo 60 caracteres.");

        RuleFor(x => x.Color)
            .Matches("^#([A-Fa-f0-9]{6})$").WithMessage("Cor deve estar no formato hexadecimal (#RRGGBB).")
            .When(x => x.Color is not null);

        RuleFor(x => x.CardLimit)
            .GreaterThan(0).WithMessage("Limite de cards deve ser maior que zero.")
            .When(x => x.CardLimit.HasValue);
    }
}
