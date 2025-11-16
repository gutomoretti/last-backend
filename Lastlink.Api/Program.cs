using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Lastlink.Api.Products;
using Lastlink.Application.Common.Exceptions;
using Lastlink.Application.Products.Commands.CreateProduct;
using Lastlink.Application.Products.Commands.DeleteProduct;
using Lastlink.Application.Products.Commands.UpdateProduct;
using Lastlink.Application.Products.Queries.GetProductById;
using Lastlink.Application.Products.Queries.GetProducts;
using Lastlink.Infrastructure.DependencyInjection;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "AllowAngularDevClient";

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateProductCommand).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Lastlink API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors(CorsPolicy);

var group = app.MapGroup("/products")
    .WithTags("Products");

group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
    {
        var result = await mediator.Send(new GetProductsQuery(), cancellationToken);
        return Results.Ok(result);
    })
    .WithName("GetProducts");

group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
    {
        try
        {
            var product = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
            return Results.Ok(product);
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    })
    .WithName("GetProductById");

group.MapPost("/", async (CreateProductRequest request, IMediator mediator, IValidator<CreateProductCommand> validator, CancellationToken cancellationToken) =>
    {
        var command = new CreateProductCommand(request.Name, request.Category, request.UnitCost);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(ToValidationDictionary(validationResult));
        }

        var product = await mediator.Send(command, cancellationToken);
        return Results.Created($"/products/{product.Id}", product);
    })
    .WithName("CreateProduct");

group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, IMediator mediator, IValidator<UpdateProductCommand> validator, CancellationToken cancellationToken) =>
    {
        var command = new UpdateProductCommand(id, request.Name, request.Category, request.UnitCost);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(ToValidationDictionary(validationResult));
        }

        try
        {
            var product = await mediator.Send(command, cancellationToken);
            return Results.Ok(product);
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    })
    .WithName("UpdateProduct");

group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
    {
        try
        {
            await mediator.Send(new DeleteProductCommand(id), cancellationToken);
            return Results.NoContent();
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    })
    .WithName("DeleteProduct");

app.Run();

static IDictionary<string, string[]> ToValidationDictionary(ValidationResult validationResult)
{
    return validationResult.Errors
        .GroupBy(error => error.PropertyName)
        .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
}
