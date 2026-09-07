using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ExpenseTracker")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/ping", () => "pong")
    .WithName("Ping");

// Create: bind the JSON body straight to an Expense, save it, and return
// 201 Created with a Location header pointing at the new resource.
app.MapPost("/api/expenses", async (Expense expense, ExpenseTrackerDbContext db) =>
{
    db.Expenses.Add(expense);
    await db.SaveChangesAsync(); // writes the INSERT and populates expense.Id
    return Results.Created($"/api/expenses/{expense.Id}", expense);
})
    .WithName("CreateExpense");

// Read: "date" is an optional query string param (?date=2026-09-05).
// Build the query in steps so filtering only happens when a date was given.
app.MapGet("/api/expenses", async (DateOnly? date, ExpenseTrackerDbContext db) =>
{
    var query = db.Expenses.AsQueryable();
    if (date is not null)
    {
        query = query.Where(e => e.Date == date);
    }
    return await query.OrderByDescending(e => e.Date).ToListAsync();
})
    .WithName("GetExpenses");

// Delete: {id} in the route binds to the int id parameter.
// FindAsync looks the row up by primary key (fast, no full table scan).
app.MapDelete("/api/expenses/{id}", async (int id, ExpenseTrackerDbContext db) =>
{
    var expense = await db.Expenses.FindAsync(id);
    if (expense is null)
    {
        return Results.NotFound(); // nothing to delete
    }
    db.Expenses.Remove(expense);
    await db.SaveChangesAsync();
    return Results.NoContent(); // 204: success, nothing to return
})
    .WithName("DeleteExpense");

// Summary: total spent on a given day. Sum runs in the database (SQL SUM),
// not pulled into memory first, so it stays cheap as the table grows.
app.MapGet("/api/expenses/summary", async (DateOnly date, ExpenseTrackerDbContext db) =>
{
    var total = await db.Expenses
        .Where(e => e.Date == date)
        .SumAsync(e => e.Amount);
    return Results.Ok(new { date, total });
})
    .WithName("GetExpensesSummary");

app.Run();
