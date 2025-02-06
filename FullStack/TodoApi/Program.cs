using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// DataBase
builder.Services.AddDbContext<ToDoDbContext>(
    options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("bkbbifeurt9knkw4wjh8"),
        new MySqlServerVersion(new Version(7, 0, 0)))
    );

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
}
);

var app = builder.Build();

// הפעלת Cors
app.UseCors("AllowAll");

// הפעלת Swagger
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(
    options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    options.RoutePrefix = string.Empty;
}
);
//}

// functions
// מסך ראשי
//app.MapGet("/", () => "FullStack API is running!");
app.MapGet("/home", () => Results.Ok("FullStack API is running!"));


// שליפת כל המשימות
app.MapGet("/items", async (ToDoDbContext db) => await db.Items.ToListAsync());

// שליפת משימה
app.MapGet("/items/{id}", async (int id, ToDoDbContext db) => await db.Items.FirstOrDefaultAsync(item => item.Id == id));

// הוספת משימה חדשה
app.MapPost("/items", async (ToDoDbContext db, Item item) =>
{
    item.IsComplete = false;
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

// עדכון משימה
app.MapPut("/items/{id}", async (int id, ToDoDbContext db, Item updatedItem) =>
{
    var item = await db.Items.FirstOrDefaultAsync(item => item.Id == id);
    if (item is null) return Results.NotFound();

    //item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// מחיקת משימה
app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FirstOrDefaultAsync(item => item.Id == id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
