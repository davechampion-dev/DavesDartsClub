using Bogus;
using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.UnitTests.Fakers;

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
        var memberFaker = new MemberFaker();
        var validMember = memberFaker.GenerateOne(); 

        //Act
        var response = _memberValidator.Validate(validMember);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }
}