// <copyright file="ValidationBehavior.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CleanArchitecture.Application.Common.Behaviors
{
  using FluentValidation;
  using MediatR;

  public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
  : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
  {
    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
      if (!validators.Any())
      {
        return await next();
      }

      var context = new ValidationContext<TRequest>(request);
      var validationErrors = validators
          .Select(v => v.Validate(context))
          .SelectMany(result => result.Errors)
          .Where(failure => failure != null);

      if (validationErrors.Any())
      {
        throw new CleanArchitecture.Application.Common.Exceptions.ValidationException(validationErrors);
      }

      return await next();
    }
  }
}