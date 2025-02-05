using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>();

// הוספת שירות CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// הוספת שירות Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// הפעלת מדיניות CORS
app.UseCors("AllowAll");

// הפעלת Swagger
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// מסך ראשי
app.MapGet("/", () => "FullStack API is running!");

// Route לשליפת כל המשימות
app.MapGet("/items", async (ToDoDbContext db) => await db.Items.ToListAsync());

// Route להוספת משימה חדשה
app.MapPost("/items", async (ToDoDbContext db, Item item) =>
{
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

// Route לעדכון משימה
app.MapPut("/items/{id}", async (int id, ToDoDbContext db, Item updatedItem) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Route למחיקת משימה
app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
