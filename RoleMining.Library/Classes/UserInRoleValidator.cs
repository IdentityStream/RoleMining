using FluentValidation;
using System.Collections.Generic;

namespace RoleMining.Library.Classes
{
    internal class UserInRoleValidator : AbstractValidator<IEnumerable<UserInRole>>
    {
        public UserInRoleValidator()
        {
            RuleFor(RuleFor => RuleFor).NotEmpty().WithMessage("{PropertyName} cannot be empty");
            RuleForEach(UserAccess => UserAccess).SetValidator(new SingleUserInRoleValidator());
        }
    }
    internal class SingleUserInRoleValidator : AbstractValidator<UserInRole>
    {
        public SingleUserInRoleValidator()
        {
            RuleFor(UserAccess => UserAccess.UserID).NotEmpty().WithMessage("{PropertyName} cannot be empty");
            RuleFor(UserAccess => UserAccess.RoleID).NotEmpty().WithMessage("{PropertyName} cannot be empty");
        }
    }
}
