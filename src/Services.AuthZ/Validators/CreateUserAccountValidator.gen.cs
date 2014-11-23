﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Services.MessageContracts;
using ServiceStack.FluentValidation;

namespace Services.AuthZ.Validators
{
    /// <summary>
    /// A validator that validates the <see cref="CreateUserAccount"/> contract.
    /// </summary>
    internal partial class CreateUserAccountValidator : AbstractValidator<CreateUserAccount>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CreateUserAccountValidator"/> class.
        /// </summary>
        public CreateUserAccountValidator()
        {
            // Rules defined by request contract
            RuleFor(dto => dto.Forenames).NotNull();
            RuleFor(dto => dto.Surname).NotNull();
            RuleFor(dto => dto.Email).NotNull();
            RuleFor(dto => dto.Address).NotNull();

            // Additional custom rules
            this.AddRules();
        }

        /// <summary>
        /// Adds custom rules for the validator
        /// </summary>
        partial void AddRules();
    }
}