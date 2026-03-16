using BloodBankManager.Data;
using BloodBankManager.Models;
using BloodBankManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure logging
builder.Services.AddLogging();

// Configure Entity Framework Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=.;Database=BloodBankDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<BloodBankContext>(options =>
    options.UseSqlServer(connectionString)
);

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<BloodBankContext>()
.AddDefaultTokenProviders();

// Register custom services
builder.Services.AddScoped<IBloodCompatibilityService, BloodCompatibilityService>();
builder.Services.AddScoped<IBloodExpiryService, BloodExpiryService>();

// Register email service (used for contact form / automatic emailing)
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

// Serve static files and set index.html as default
app.UseDefaultFiles();
app.UseStaticFiles();

var options = new DefaultFilesOptions();
options.DefaultFileNames.Clear();
options.DefaultFileNames.Add("register.htm");

app.UseDefaultFiles(options);
app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Create and apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("Database migrated successfully.");

        // Seed roles
        if (!await roleManager.RoleExistsAsync("Staff"))
        {
            await roleManager.CreateAsync(new IdentityRole("Staff"));
        }
        if (!await roleManager.RoleExistsAsync("Patient"))
        {
            await roleManager.CreateAsync(new IdentityRole("Patient"));
        }
        Console.WriteLine("Roles seeded successfully.");

        // Seed blood types if they don't exist
        if (!dbContext.BloodTypes.Any())
        {
            var bloodTypes = new[]
            {
                new BloodType { TypeName = BloodTypeEnum.O_NEGATIVE, Description = "O âm" },
                new BloodType { TypeName = BloodTypeEnum.O_POSITIVE, Description = "O dương" },
                new BloodType { TypeName = BloodTypeEnum.A_NEGATIVE, Description = "A âm" },
                new BloodType { TypeName = BloodTypeEnum.A_POSITIVE, Description = "A dương" },
                new BloodType { TypeName = BloodTypeEnum.B_NEGATIVE, Description = "B âm" },
                new BloodType { TypeName = BloodTypeEnum.B_POSITIVE, Description = "B dương" },
                new BloodType { TypeName = BloodTypeEnum.AB_NEGATIVE, Description = "AB âm" },
                new BloodType { TypeName = BloodTypeEnum.AB_POSITIVE, Description = "AB dương" }
            };
            dbContext.BloodTypes.AddRange(bloodTypes);
            dbContext.SaveChanges();
            Console.WriteLine("Blood types seeded successfully.");
        }

        // Seed sample blood inventory if it doesn't exist
        if (!dbContext.BloodInventories.Any())
        {
            var bloodInventories = new[]
            {
                new BloodInventory { BloodTypeId = 1, TotalUnits = 25, AvailableUnits = 20, ReservedUnits = 5, TotalVolume = 1125, LowStockThreshold = 500 },
                new BloodInventory { BloodTypeId = 2, TotalUnits = 30, AvailableUnits = 25, ReservedUnits = 5, TotalVolume = 1350, LowStockThreshold = 500 },
                new BloodInventory { BloodTypeId = 3, TotalUnits = 15, AvailableUnits = 12, ReservedUnits = 3, TotalVolume = 675, LowStockThreshold = 400 },
                new BloodInventory { BloodTypeId = 4, TotalUnits = 20, AvailableUnits = 18, ReservedUnits = 2, TotalVolume = 900, LowStockThreshold = 450 }
            };
            dbContext.BloodInventories.AddRange(bloodInventories);
            dbContext.SaveChanges();
            Console.WriteLine("Sample blood inventory seeded successfully.");
        }

        // Seed sample patients if they don't exist
        if (!dbContext.Patients.Any())
        {
            var patients = new[]
            {
                new Patient
                {
                    Name = "Nguyễn Văn A",
                    PatientCode = "BN-001",
                    BloodTypeId = 2, // O+
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Gender = "Nam",
                    Hospital = "Bệnh viện Trung ương",
                    Ward = "Phòng 101",
                    AdmissionDate = DateTime.UtcNow.AddDays(-5),
                    PhoneNumber = "0912345678",
                    Email = "nguyenvana@email.com",
                    MedicalCondition = "Phẫu thuật tim"
                },
                new Patient
                {
                    Name = "Trần Thị B",
                    PatientCode = "BN-002",
                    BloodTypeId = 4, // A+
                    DateOfBirth = new DateTime(1985, 8, 22),
                    Gender = "Nữ",
                    Hospital = "Bệnh viện Trung ương",
                    Ward = "Phòng 205",
                    AdmissionDate = DateTime.UtcNow.AddDays(-3),
                    PhoneNumber = "0987654321",
                    Email = "tranthib@email.com",
                    MedicalCondition = "Truyền máu"
                },
                new Patient
                {
                    Name = "Phạm Văn C",
                    PatientCode = "BN-003",
                    BloodTypeId = 1, // O-
                    DateOfBirth = new DateTime(1995, 3, 10),
                    Gender = "Nam",
                    Hospital = "Bệnh viện Trung ương",
                    Ward = "Phòng 301",
                    AdmissionDate = DateTime.UtcNow.AddDays(-1),
                    PhoneNumber = "0909876543",
                    Email = "phamvanc@email.com",
                    MedicalCondition = "Chấn thương"
                }
            };
            dbContext.Patients.AddRange(patients);
            dbContext.SaveChanges();
            Console.WriteLine("Sample patients seeded successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error migrating database: {ex.Message}");
    }
}

app.Run();
