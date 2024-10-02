using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

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
