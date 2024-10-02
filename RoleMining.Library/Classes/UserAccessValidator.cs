using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace RoleMining.Library.Classes
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
