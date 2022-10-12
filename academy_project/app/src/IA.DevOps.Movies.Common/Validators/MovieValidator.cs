using FluentValidation;
using IA.DevOps.Movies.Contracts.Forms;
using IA.DevOps.Movies.Data.Db.Configurations;

namespace IA.DevOps.Movies.Common.Validators
{
    public class MovieValidator : AbstractValidator<MovieForm>
    {
        public MovieValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .NotNull()
                .MaximumLength(DatabaseDefaults.DefaultStringSize);

            RuleFor(x => x.ReleasedYear)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .GreaterThan(0);

            RuleFor(x => x.Rating)
                .NotEmpty()
                .LessThanOrEqualTo(10)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Genre)
                .NotEmpty()
                .NotNull()
                .MaximumLength(DatabaseDefaults.DefaultStringSize);

            RuleFor(x => x.Overview)
                .NotEmpty();
        }
    }
}