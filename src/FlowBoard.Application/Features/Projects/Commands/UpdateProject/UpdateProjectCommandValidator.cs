using FluentValidation;

namespace FlowBoard.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do projeto é obrigatório.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Color)
            .Matches("^#([A-Fa-f0-9]{6})$").WithMessage("Cor deve estar no formato hexadecimal (#RRGGBB).")
            .When(x => x.Color is not null);
    }
}
