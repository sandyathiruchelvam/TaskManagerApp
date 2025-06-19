var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Add Session services BEFORE building the app
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Enable Session middleware
app.UseSession();

app.UseAuthorization();

// ✅ Only ONE default route needed, extra removed
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ✅ Additional custom routes (optional, useful for clean URLs)
app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });




app.MapControllerRoute(
    name: "taskdashboard",
    pattern: "dashboard",
    defaults: new { controller = "Task", action = "Dashboard" });

app.Run();
