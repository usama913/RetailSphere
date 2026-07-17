using FluentAssertions;
using RetailSphere.SharedKernel;
using Xunit;

namespace RetailSphere.UnitTests.SharedKernel;

public sealed class ResultTests
{
    [Fact]
    public void Success_result_has_no_error()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_result_carries_the_given_error()
    {
        var error = Error.Validation("Test.Invalid", "Something was invalid.");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Accessing_value_of_a_failed_generic_result_throws()
    {
        var result = Result.Failure<int>(Error.NotFound("Test.NotFound", "Not found."));

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Successful_generic_result_exposes_its_value()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }
}
