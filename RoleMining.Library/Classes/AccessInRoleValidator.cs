using FluentValidation;
using System.Collections.Generic;

namespace RoleMining.Library.Classes
{
    internal class AccessInRoleValidator : AbstractValidator<IEnumerable<AccessInRole>>
    {
        public AccessInRoleValidator()
        {
            RuleForEach(UserAccess => UserAccess).SetValidator(new SingleAccessInRoleValidator());
        }
    }

    internal class SingleAccessInRoleValidator : AbstractValidator<AccessInRole>
    {
        public SingleAccessInRoleValidator()
        {
            RuleFor(UserAccess => UserAccess.RoleID).NotEmpty().WithMessage("{PropertyName} cannot be empty");
            RuleFor(UserAccess => UserAccess.AccessID).NotEmpty().WithMessage("{PropertyName} cannot be empty");
        }
    }
}
