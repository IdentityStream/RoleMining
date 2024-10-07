using FluentValidation;
using RoleMining.Library.Classes;
using System.Collections.Generic;

namespace RoleMining.Library.Validation
{
    internal class UserAccessValidator : AbstractValidator<IEnumerable<UserAccess>>
    {
        public UserAccessValidator()
        {
            RuleFor(RuleFor => RuleFor).NotEmpty().WithMessage("{PropertyName} cannot be empty");
            RuleForEach(UserAccess => UserAccess).SetValidator(new SingleUserAccessValidator());
        }
    }

    internal class SingleUserAccessValidator : AbstractValidator<UserAccess>
    {
        public SingleUserAccessValidator()
        {
            RuleFor(UserAccess => UserAccess.UserID).NotEmpty();
            RuleFor(UserAccess => UserAccess.AccessID).NotEmpty();
        }
    }
}
