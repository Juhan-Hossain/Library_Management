using LibraryManagement.Application.Common.Behaviors;

namespace LibraryManagement.Application.UnitTests.Behaviors;

public sealed class ValidationBehaviorTests
{
    public sealed record Ping(string Name) : IRequest<string>;

    public sealed class PingValidator : AbstractValidator<Ping>
    {
        public PingValidator() => RuleFor(p => p.Name).NotEmpty();
    }

    [Fact]
    public async Task Throws_validation_exception_and_skips_handler_when_invalid()
    {
        var behavior = new ValidationBehavior<Ping, string>(new[] { new PingValidator() });
        var handlerCalled = false;

        var act = () => behavior.Handle(new Ping(""), () => { handlerCalled = true; return Task.FromResult("pong"); }, CancellationToken.None);

        (await act.Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        handlerCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Calls_next_when_valid()
    {
        var behavior = new ValidationBehavior<Ping, string>(new[] { new PingValidator() });

        var result = await behavior.Handle(new Ping("x"), () => Task.FromResult("pong"), CancellationToken.None);

        result.Should().Be("pong");
    }

    [Fact]
    public async Task Calls_next_when_no_validators_registered()
    {
        var behavior = new ValidationBehavior<Ping, string>(Array.Empty<IValidator<Ping>>());

        var result = await behavior.Handle(new Ping(""), () => Task.FromResult("pong"), CancellationToken.None);

        result.Should().Be("pong");
    }
}