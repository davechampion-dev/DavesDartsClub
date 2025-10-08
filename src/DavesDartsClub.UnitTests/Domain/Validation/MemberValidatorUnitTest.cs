using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavesDartsClub.UnitTests.Domain.Validation;

public class MemberValidatorUnitTest
{
    private readonly MemberValidator _memberValidator;
    public MemberValidatorUnitTest()
    {
        _memberValidator = new MemberValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponseWithNoErrors_Given_AValidMember()
    {
        //Arrange
        var validMember = new Member
        {
            MemberId = Guid.NewGuid(),
            MemberName = "Edd the Duck"
        };

        //Act
        var response = _memberValidator.Validate(validMember);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }
}