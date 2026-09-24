# .NET Unit Testing Skill

Use this skill when creating or reviewing .NET unit tests.

## Framework

Prefer the testing framework already used by the repository.

## Test Structure

Use Arrange-Act-Assert.

## Naming

Use descriptive behavior-oriented names.

Example:

Should_Reject_Loan_When_CreditScore_Is_Below_Threshold

## Test Categories

Consider:

- Happy path
- Boundary values
- Invalid input
- Exceptions
- Null values
- Dependency failures
- Authorization failures
- Business-rule combinations

## Mocking

Mock external dependencies rather than the class under test.

Avoid unnecessary mocks.

## Parameterized Tests

Use parameterized tests when multiple inputs exercise the same behavior.

## Assertions

Assertions must validate business behavior, not merely execution.

## Maintainability

Avoid testing private implementation details.
