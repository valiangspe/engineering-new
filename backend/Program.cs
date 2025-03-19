using backend.Migrations;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SupportReportAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SupportReportAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);
// var connString = "server=172.17.0.1;database=engineer;user=gspe;password=gspe-intercon";
// var connString = "server=host.docker.internal;database=engineer;user=gspe;password=gspe-intercon";
var connString = "server=127.0.0.1;database=engineer;user=root;password=";

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonDateTimeConverter());

    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connString, ServerVersion.AutoDetect(connString)));
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//     new MySqlServerVersion(new Version(8, 0, 23))));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Allowall",
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                      });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseCors("Allowall");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


// EngineerSupports
app.MapPost("/login", async (LoginBody? loginBody) =>
{
    var client = new HttpClient();

    var loginUrl = "https://authserver-backend.iotech.my.id/login";
    var loginData = new
    {
        username = loginBody.Username,
        password = loginBody.Password
    };

    var jsonContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

    var response = await client.PostAsync(loginUrl, jsonContent);

    if (response.StatusCode != System.Net.HttpStatusCode.OK)
    {
        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new Exception(errorMessage);
    }

    var token = await response.Content.ReadAsStringAsync();

    return token;
});



// EngineerSupports
app.MapGet("/engineerSupports", async (AppDbContext db) =>
    await db.EngineerSupports.ToListAsync());

app.MapPost("/engineerSupports", async (EngineerSupport engineerSupport, AppDbContext db) =>
{
    if (engineerSupport.Id == 0)
    {
        db.EngineerSupports.Add(engineerSupport);
    }
    else
    {
        db.EngineerSupports.Update(engineerSupport);
    }
    await db.SaveChangesAsync();
    return Results.Ok(engineerSupport);
});

// SupportDetails
app.MapGet("/supportDetails", async (AppDbContext db) =>
    await db.SupportDetails.ToListAsync());

app.MapPost("/supportDetails", async (SupportDetail supportDetail, AppDbContext db) =>
{
    if (supportDetail.Id == 0)
    {
        db.SupportDetails.Add(supportDetail);
    }
    else
    {
        db.SupportDetails.Update(supportDetail);
    }
    await db.SaveChangesAsync();
    return Results.Ok(supportDetail);
});

// OutstandingPostPOs
app.MapGet("/outstandingPostPOs", async (AppDbContext db) =>
    await db.OutstandingPostPOs.ToListAsync());

app.MapPost("/outstandingPostPOs", async (OutstandingPostPO outstandingPostPO, AppDbContext db) =>
{
    if (outstandingPostPO.Id == 0)
    {
        db.OutstandingPostPOs.Add(outstandingPostPO);
    }
    else
    {
        db.OutstandingPostPOs.Update(outstandingPostPO);
    }
    await db.SaveChangesAsync();
    return Results.Ok(outstandingPostPO);
});

// DonePOs
app.MapGet("/donePOs", async (AppDbContext db) =>
    await db.DonePOs.ToListAsync());

app.MapPost("/donePOs", async (DonePO donePO, AppDbContext db) =>
{
    if (donePO.Id == 0)
    {
        db.DonePOs.Add(donePO);
    }
    else
    {
        db.DonePOs.Update(donePO);
    }
    await db.SaveChangesAsync();
    return Results.Ok(donePO);
});

// EngineeringDetailProblems
app.MapGet("/engineeringDetailProblems", async (AppDbContext db) =>
    await db.EngineeringDetailProblems
    .Include(p => p.Items)
    .Include(p => p.Approvals)

    .ToListAsync());

app.MapGet("/engineeringDetailProblems/{id}", async (AppDbContext db, int id) =>
     await db.EngineeringDetailProblems
     .Where(e => e.Id == id)
        .Include(p => p.Items)
        .Include(p => p.Approvals)


     .FirstOrDefaultAsync());

app.MapGet("/engineeringDetailProblems/{id}/photo", async (AppDbContext db, int id) =>
{
    var ecnFound = db.EngineeringDetailProblems.Where(e => e.Id == id).FirstOrDefault();
    if (ecnFound == null || string.IsNullOrEmpty(ecnFound.ApprovalFileName))
        return Results.NotFound("File not found.");

    var filePath = $"./files/ecn_doc_{id}";
    if (!File.Exists(filePath))
        return Results.NotFound("File not found.");

    return Results.File(await File.ReadAllBytesAsync(filePath), FileHelper.GetContentType(filePath), ecnFound.ApprovalFileName);
});


// string GetContentType(string filePath)
// {
//     var ext = filePath.Split(".").LastOrDefault();
//     return ext switch
//     {
//         ".txt" => "text/plain",
//         ".pdf" => "application/pdf",
//         ".jpg" => "image/jpeg",
//         ".png" => "image/png",
//         ".zip" => "application/zip",
//         _ => "application/octet-stream",
//     };
// }


app.MapPost("/engineeringDetailProblems", async (EngineeringDetailProblem engineeringDetailProblem, AppDbContext db) =>
{
    if (engineeringDetailProblem.Id == 0)
    {
        db.EngineeringDetailProblems.Add(engineeringDetailProblem);
    }
    else
    {
        db.EngineeringDetailProblems.Update(engineeringDetailProblem);
    }
    await db.SaveChangesAsync();
    return Results.Ok(engineeringDetailProblem);
});

// bomapprovals
app.MapGet("/bomapprovals", async (AppDbContext db) =>
    await db.BomApprovals
    .Include(p => p.Pics)
    .ToListAsync());

app.MapGet("/bomapprovals/{id}", async (AppDbContext db, int id) =>
     await db.BomApprovals
     .Where(e => e.ExtBomLeveledId == id)
        .Include(p => p.Pics)
     .FirstOrDefaultAsync());



app.MapPost("/bomapprovals", async (BomApproval bomApproval, AppDbContext db) =>
{
    if (bomApproval.Id == 0)
    {
        db.BomApprovals.Add(bomApproval);
    }
    else
    {
        db.BomApprovals.Update(bomApproval);
    }

    await db.SaveChangesAsync();

    Console.WriteLine(bomApproval.ExtBomLeveledId);
    Console.WriteLine(bomApproval.Pics?.Count);

    // If bom approval has PIC, snpshot
    if ((bomApproval.Pics?.Count ?? 0) > 0)
    {
        try
        {
            var response = await new HttpClient().GetAsync($"https://ppic-backend.iotech.my.id/snapshot-bom/{bomApproval.ExtBomLeveledId}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e}");
        }

    }

    return Results.Ok(bomApproval);
});

app.MapPost("/engineeringDetailProblems/{id}/photo", async (EngDetailProblemPhoto photo, AppDbContext db, int id) =>
{
    var found = db.EngineeringDetailProblems.Where(e => e.Id == id).FirstOrDefault();

    Console.WriteLine($"found id {id}");
    if (found != null)
    {
        SaveBase64ToFile(photo.Photo, $"./files/ecn_doc_{id}");
    }

    return Results.Ok("Ok");
});


app.MapGet("/configurations", async (AppDbContext db) =>
    await db.Configurations.ToListAsync());

app.MapPost("/configurations", async (Configuration configuration, AppDbContext db) =>
{
    if (configuration.Id == 0)
    {
        db.Configurations.Add(configuration);
    }
    else
    {
        db.Configurations.Update(configuration);
    }
    await db.SaveChangesAsync();
    return Results.Ok(configuration);
});

// 4. Update Program.cs with new endpoints
app.MapGet("/api/dashboard/metrics", async (AppDbContext db) =>
    await db.DashboardMetrics.FirstOrDefaultAsync());
app.MapGet("/api/dashboard/metrics/arr", async (AppDbContext db) =>
   new List<DashboardMetrics>
        {
            await db.DashboardMetrics.FirstOrDefaultAsync()
        });

app.MapPost("/api/dashboard/metrics/arr", async (DashboardMetrics metrics, AppDbContext db) =>
{
    if (metrics.Id == 0)
    {
        db.DashboardMetrics.Add(metrics);
    }
    else
    {
        db.DashboardMetrics.Update(metrics);
    }
    await db.SaveChangesAsync();
    return Results.Ok(metrics);
});

app.MapGet("/api/deptconfigs", async (AppDbContext db) =>
    await db.EngDeptConfigs.ToListAsync());


app.MapPost("/api/deptconfigs", async (List<EngDeptConfig> deptConfig, AppDbContext db) =>
{

    db.EngDeptConfigs.UpdateRange(deptConfig);
    await db.SaveChangesAsync();
    return Results.Ok(deptConfig);
});


// for support engineering document
// API untuk mengambil semua SupportEngineeringDocuments
app.MapGet("/api/support-engineering-documents", async (AppDbContext db) =>
{
    var documents = await db.SupportEngineeringDocuments
        .GroupBy(d => new { d.JobId, d.JobName })
        .Select(g => new
        {
            JobId = g.Key.JobId,
            JobName = g.Key.JobName,
            SupportTableIds = g.Select(d => d.SupportTableId).Where(id => id.HasValue).Select(id => id.Value).ToList()
        })
        .ToListAsync();

    return Results.Ok(documents);
});

// API untuk menambahkan SupportEngineeringDocument baru dengan kemampuan menerima banyak SupportTableId
app.MapPost("/api/support-engineering-documents", async (AppDbContext db, [FromBody] SupportEngineeringDocumentDto dto) =>
{
    // Memeriksa apakah data valid
    if (dto == null || dto.SupportTableIds == null || dto.SupportTableIds.Length == 0)
        return Results.BadRequest("Invalid input data.");

    // Menambahkan SupportTableId untuk setiap id yang diterima
    foreach (var supportTableId in dto.SupportTableIds)
    {
        var newDocument = new SupportEngineeringDocument
        {
            JobId = dto.JobId,
            JobName = dto.JobName,
            SupportTableId = supportTableId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        db.SupportEngineeringDocuments.Add(newDocument);
    }

    await db.SaveChangesAsync();

    return Results.Ok(dto);
});



// API untuk memperbarui SupportTableId untuk SupportEngineeringDocument yang ada
app.MapPut("/api/support-engineering-documents/{id}/supporttableid", async (AppDbContext db, int id, [FromBody] int[] supportTableIds) =>
{
    var document = await db.SupportEngineeringDocuments
        .FirstOrDefaultAsync(d => d.Id == id);

    if (document == null)
        return Results.NotFound();

    // Memperbarui SupportTableId
    if (supportTableIds != null && supportTableIds.Length > 0)
    {
        document.SupportTableId = supportTableIds.Last();  // Jika banyak, hanya mengambil yang terakhir
        document.UpdatedAt = DateTime.Now;

        await db.SaveChangesAsync();
        return Results.Ok(document);
    }

    return Results.BadRequest("SupportTableId tidak valid.");
});


// API untuk menghapus SupportEngineeringDocument berdasarkan ID
app.MapDelete("/api/support-engineering-documents/{id}", async (AppDbContext db, int id) =>
{
    var document = await db.SupportEngineeringDocuments
        .FirstOrDefaultAsync(d => d.Id == id);

    if (document == null)
        return Results.NotFound();

    db.SupportEngineeringDocuments.Remove(document);
    await db.SaveChangesAsync();

    return Results.Ok(document);
});


// app.MapGet("/api/dashboard/activities", async (AppDbContext db, string? from, string? to, int? taskId, int? extInquiryId, bool? withUserNames, bool? excel, HttpContext httpContext, int? userId) =>
// {
//     DateTime? fromDate = null;
//     DateTime? toDate = null;

//     if (from != null && from != "")
//     {
//         fromDate = DateTime.Parse(from);
//     }
//     if (to != null && to != "")
//     {
//         toDate = DateTime.Parse(to);
//     }

//     // Query dasar untuk mendapatkan semua aktivitas dan task yang terkait
//     var activitiesQuery = db.EngineeringActivities
//             .Include(a => a.Tasks)
//                 .ThenInclude(t => t.InCharges)
//             .AsQueryable();

//     // Filter berdasarkan `taskId` jika diberikan
//     if (taskId != null)
//     {
//         activitiesQuery = activitiesQuery.Where(a => a.Tasks.Any(t => t.Id == taskId));
//     }

//     // Filter berdasarkan tanggal jika `fromDate` dan/atau `toDate` diberikan
//     if (fromDate != null)
//     {
//         activitiesQuery = activitiesQuery.Where(a => a.ToCache >= fromDate);
//     }
//     if (toDate != null)
//     {
//         activitiesQuery = activitiesQuery.Where(a => a.FromCache <= toDate);
//     }

//     // Filter berdasarkan `extInquiryId` jika diberikan
//     if (extInquiryId != null && extInquiryId != 0)
//     {
//         activitiesQuery = activitiesQuery.Where(a => a.ExtInquiryId == extInquiryId);
//     }

//     // Eksekusi query dan filter berdasarkan `userId` jika diberikan
//     var activities = (await activitiesQuery.ToListAsync()).Where(a =>
//     {
//         if (userId != null)
//         {
//             return a.Tasks?.Any(t => t.InCharges.Any(ic => ic.ExtUserId == userId)) ?? false;
//         }
//         return true;
//     }).ToList();

//     // Sertakan nama pengguna jika `withUserNames` diatur ke `true`
//     if (withUserNames == true)
//     {
//         var users = await Fetcher.fetchUsersAsync();
//         activities.ForEach(a =>
//         {
//             a.Tasks.ForEach(t =>
//             {
//                 t.InCharges.ForEach(c =>
//                 {
//                     var foundUser = users.FirstOrDefault(u => u.Id == c.ExtUserId);
//                     c.PicName = foundUser?.Name ?? "";
//                 });
//             });
//         });
//     }

//     // Jika opsi `excel` aktif, ekspor data ke Excel
//     if (excel == true)
//     {
//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Activities");

//         var pos = await Fetcher.fetchCrmPurchaseOrdersAsync();
//         var inqs = await Fetcher.fetchCrmInquiriesAsync();

//         worksheet.Cell(1, 1).Value = "Task Name";
//         worksheet.Cell(1, 2).Value = "Type";
//         worksheet.Cell(1, 3).Value = "PO";
//         worksheet.Cell(1, 4).Value = "Inq";
//         worksheet.Cell(1, 5).Value = "Quo";
//         worksheet.Cell(1, 6).Value = "In Charge";
//         worksheet.Cell(1, 7).Value = "Hours";
//         worksheet.Cell(1, 8).Value = "Start Date";
//         worksheet.Cell(1, 9).Value = "End Date";

//         int row = 2;
//         foreach (var activity in activities)
//         {
//             var foundPO = pos.FirstOrDefault(p => p.Id == activity.ExtPurchaseOrderId);
//             var foundInq = inqs.FirstOrDefault(p => p.Id == activity.ExtInquiryId);

//             foreach (var task in activity.Tasks.Where(t => t.DeletedAt == null).ToList())
//             {
//                 worksheet.Cell(row, 1).Value = task.Description;
//                 worksheet.Cell(row, 2).Value = $"{activity.Type}";
//                 worksheet.Cell(row, 3).Value = $"{foundPO?.purchaseOrderNumber} ({foundPO?.Account?.Name})";
//                 worksheet.Cell(row, 4).Value = $"{foundInq?.InquiryNumber} ({foundInq?.Account?.Name})";
//                 worksheet.Cell(row, 5).Value = foundInq?.Quotation?.Name;
//                 worksheet.Cell(row, 6).Value = string.Join(", ", task.InCharges?.Select(i => i.PicName) ?? []);
//                 worksheet.Cell(row, 7).Value = task.Hours;
//                 worksheet.Cell(row, 8).Value = activity.FromCache?.ToString("yyyy-MM-dd");
//                 worksheet.Cell(row, 9).Value = activity.ToCache?.ToString("yyyy-MM-dd");

//                 row++;
//             }
//         }

//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         stream.Position = 0;

//         httpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=activities.xlsx");
//         return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "activities.xlsx");
//     }

//     return Results.Ok(activities);
// });

// membuka task by id
app.MapGet("/api/dashboard/activities/task/{taskId:int}", async (
    AppDbContext db,
    int taskId,
    bool? withUserNames,
    HttpContext httpContext) =>
{
    // Query aktivitas yang terkait dengan Task ID tertentu
    var activitiesQuery = db.EngineeringActivities
        .Include(a => a.Tasks)
        .ThenInclude(t => t.InCharges)
        .Where(a => a.Tasks.Any(t => t.Id == taskId))
        .AsQueryable();

    // Ambil data dari database
    var activities = await activitiesQuery.ToListAsync();

    // Konversi ke DTO
    var filteredActivities = activities.Select(activity => new ActivityDto
    {
        Id = activity.Id,
        Description = activity.Description,
        Type = activity.Type,
        ExtInquiryId = activity.ExtInquiryId,
        ExtPurchaseOrderId = activity.ExtPurchaseOrderId,
        FromCache = activity.FromCache,
        ToCache = activity.ToCache,
        ExtJobId = activity.ExtJobId,
        ExtPanelCodeId = activity.ExtPanelCodeId,
        Tasks = activity.Tasks
            .Where(t => t.Id == taskId && t.DeletedAt == null)
            .Select(task => new TaskDto
            {
                Id = task.Id,
                Description = task.Description,
                From = task.From,
                To = task.To,
                Hours = task.Hours,
                Remark = task.Remark,
                CompletedDatePic = task.CompletedDatePic,
                CompletedDateSpv = task.CompletedDateSpv,
                CompletedDateManager = task.CompletedDateManager,
                CompletedByPicId = task.CompletedByPicId,
                CompletedBySpvId = task.CompletedBySpvId,
                CompletedByManagerId = task.CompletedByManagerId,
                InCharges = task.InCharges.Select(inCharge => new InChargeDto
                {
                    Id = inCharge.Id,
                    ExtUserId = inCharge.ExtUserId,
                    PicName = inCharge.PicName // PicName akan di-update di langkah berikut
                }).ToList()
            }).ToList()
    }).ToList();

    // Jika withUserNames == true, tambahkan nama pengguna ke setiap InCharge
    if (withUserNames == true)
    {
        var users = await Fetcher.fetchUsersAsync();
        foreach (var activity in filteredActivities)
        {
            foreach (var task in activity.Tasks)
            {
                foreach (var inCharge in task.InCharges)
                {
                    var foundUser = users.FirstOrDefault(u => u.Id == inCharge.ExtUserId);
                    inCharge.PicName = foundUser?.Name ?? ""; // Tambahkan nama pengguna
                }
            }
        }
    }

    // Return hasil dalam format JSON
    return Results.Ok(filteredActivities);
});

// batas


// yang di bawah ini backeup nya yang activity
app.MapGet("/api/dashboard/activities", async (
    AppDbContext db,
    string? from,
    string? to,
    int? extInquiryId,
    int? taskId,
    int? supportTableId, // Tambahan filter SupportTableId
    bool? withUserNames,
    bool? excel,
    HttpContext httpContext,
    int? userId) =>
{
    DateTime? fromDate = string.IsNullOrEmpty(from) ? null : DateTime.Parse(from);
    DateTime? toDate = string.IsNullOrEmpty(to) ? null : DateTime.Parse(to);

    var activitiesQuery = db.EngineeringActivities
        .Include(a => a.Tasks)
            .ThenInclude(t => t.InCharges)
        .AsQueryable();

    if (fromDate != null)
        activitiesQuery = activitiesQuery.Where(a => a.ToCache >= fromDate);
    if (toDate != null)
        activitiesQuery = activitiesQuery.Where(a => a.FromCache <= toDate);
    if (extInquiryId != null && extInquiryId != 0)
        activitiesQuery = activitiesQuery.Where(a => a.ExtInquiryId == extInquiryId);
    if (taskId != null)
        activitiesQuery = activitiesQuery.Where(a => a.Tasks.Any(t => t.Id == taskId));
    if (supportTableId != null)
        activitiesQuery = activitiesQuery.Where(a => a.SupportTableId == supportTableId);

    var activities = (await activitiesQuery.ToListAsync()).Where(a =>
        userId == null || a.Tasks.Any(t => t.InCharges.Any(ic => ic.ExtUserId == userId))).ToList();

    if (withUserNames == true)
    {
        var users = await Fetcher.fetchUsersAsync();
        activities.ForEach(a => a.Tasks.ForEach(t => t.InCharges.ForEach(c =>
        {
            var foundUser = users.FirstOrDefault(u => u.Id == c.ExtUserId);
            c.PicName = foundUser?.Name ?? "";
        })));
    }

    return Results.Ok(activities);
});
// app.MapGet("/api/dashboard/activities", async (AppDbContext db, string? from, string? to, int? extInquiryId, bool? withUserNames, bool? excel, HttpContext httpContext, int? userId) =>
// {

//     DateTime? fromDate = null;
//     DateTime? toDate = null;

//     if (from != null && from != "")
//     {
//         fromDate = DateTime.Parse(from);
//     }
//     if (to != null && to != "")
//     {
//         toDate = DateTime.Parse(to);
//     }


//     var activitiesQuery = db.EngineeringActivities
//             .Include(a => a.Tasks)
//                 .ThenInclude(t => t.InCharges)
//                 .AsQueryable();


//     if (fromDate != null)
//     {
//         activitiesQuery = activitiesQuery.Where(a => a.ToCache >= fromDate);
//     }

//     if (toDate != null)
//     {
//         activitiesQuery = activitiesQuery.Where(a => a.FromCache <= toDate);

//     }
//     if (extInquiryId != null && extInquiryId != 0)
//     {
//         activitiesQuery = activitiesQuery.Where(a => a.ExtInquiryId == extInquiryId);
//     }




//     var activities = (await activitiesQuery.ToListAsync())
//         .Where(a =>
//         {

//             if (userId != null)
//             {
//                 return (a.Tasks?.FirstOrDefault(t => t.InCharges.FirstOrDefault(ic => ic.ExtUserId == userId) != null) != null);
//             }
//             else
//             {
//                 return true;
//             }
//         }).ToList();


//     if (withUserNames == true)
//     {

//         var users = await Fetcher.fetchUsersAsync();

//         // Console.WriteLine(JsonSerializer.Serialize(users.Select(u => u.Name).ToList()));

//         activities.ForEach(a =>
//         {
//             a.Tasks.ForEach(t =>
//             {
//                 t.InCharges.ForEach(c =>
//                 {
//                     var foundUser = users.FirstOrDefault(u => u.Id == c.ExtUserId);

//                     c.PicName = foundUser?.Name ?? "";
//                 });
//             });
//         });
//     }
//     Console.WriteLine(excel);
//     if (excel == true)
//     {

//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Activities");

//         var pos = await Fetcher.fetchCrmPurchaseOrdersAsync();
//         var inqs = await Fetcher.fetchCrmInquiriesAsync();


//         // Fill in some sample data or real data
//         worksheet.Cell(1, 1).Value = "Task Name";
//         worksheet.Cell(1, 2).Value = "Type";
//         worksheet.Cell(1, 3).Value = "PO";
//         worksheet.Cell(1, 4).Value = "Inq";
//         worksheet.Cell(1, 5).Value = "Quo";

//         worksheet.Cell(1, 6).Value = "In Charge";
//         worksheet.Cell(1, 7).Value = "Hours";

//         worksheet.Cell(1, 8).Value = "Start Date";
//         worksheet.Cell(1, 9).Value = "End Date";

//         int row = 2; // Starting row for data
//         foreach (var activity in activities)
//         {
//             var foundPO = pos.FirstOrDefault(p => p.Id == activity.ExtPurchaseOrderId);
//             var foundInq = inqs.FirstOrDefault(p => p.Id == activity.ExtInquiryId);



//             foreach (var task in activity.Tasks.Where(t => t.DeletedAt == null).ToList())
//             {
//                 worksheet.Cell(row, 1).Value = task.Description;
//                 worksheet.Cell(row, 2).Value = $"{activity.Type}";
//                 worksheet.Cell(row, 3).Value = $"{foundPO?.purchaseOrderNumber} ({foundPO?.Account?.Name})";
//                 worksheet.Cell(row, 4).Value = $"{foundInq?.InquiryNumber} ({foundInq?.Account?.Name})";
//                 worksheet.Cell(row, 5).Value = $"{foundInq?.Quotation?.Name}";
//                 worksheet.Cell(row, 6).Value = string.Join(", ", task.InCharges?.Select(i => i.PicName) ?? []);
//                 worksheet.Cell(row, 7).Value = $"{task.Hours}";

//                 worksheet.Cell(row, 8).Value = activity.FromCache?.ToString("yyyy-MM-dd");
//                 worksheet.Cell(row, 9).Value = activity.ToCache?.ToString("yyyy-MM-dd");

//                 row++;
//             }
//         }

//         // Save the Excel file to a memory stream
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         stream.Position = 0;

//         // Set the response headers and return the file as a FileResult
//         httpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=activities.xlsx");
//         return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "activities.xlsx");
//     }



//     return Results.Ok(activities);



// });
// Batas activity


app.MapGet("/api/dashboard/activities/{id}", async (AppDbContext db, int id) =>
    await db.EngineeringActivities
        .Include(a => a.Tasks)
            .ThenInclude(t => t.InCharges)
        .Where(a => a.Id == id)
        .Select(a => new ActivityDto
        {
            Id = a.Id,
            Description = a.Description,
            Type = a.Type,
            ExtInquiryId = a.ExtInquiryId,
            ExtPurchaseOrderId = a.ExtPurchaseOrderId,
            FromCache = a.FromCache,
            ToCache = a.ToCache,
            ExtJobId = a.ExtJobId,
            ExtPanelCodeId = a.ExtPanelCodeId,
            SupportTableId = a.SupportTableId, // Pastikan ini ada di DTO
            Tasks = a.Tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Description = t.Description,
                From = t.From,
                To = t.To,
                Hours = t.Hours,
                Remark = t.Remark,
                InCharges = t.InCharges.Select(i => new InChargeDto
                {
                    Id = i.Id,
                    ExtUserId = i.ExtUserId,
                    PicName = i.PicName
                }).ToList()
            }).ToList()
        }).FirstOrDefaultAsync());

app.MapGet("/api/dashboard/activities/test", async (AppDbContext db) =>
{
    var res = JsonSerializer.Serialize(new List<EngineeringActivity>() {
    new EngineeringActivity() {
        Type= EngineeringActivityType.PrePO
    }
}, new JsonSerializerOptions
{
    Converters =
    {
        new JsonStringEnumConverter()
    }
});

    Console.WriteLine(res);

    return res;
});
app.MapPost("/api/dashboard/activities", async (EngineeringActivity activity, AppDbContext db) =>
{
    // **Perbaikan: Pastikan SupportTableId mendapatkan nilai dari selectedSupportDocId**
    if (activity.SupportTableId == null && activity.GetType().GetProperty("selectedSupportDocId") != null)
    {
        var selectedSupportDocId = (int?)activity.GetType().GetProperty("selectedSupportDocId")?.GetValue(activity);
        if (selectedSupportDocId != null)
        {
            activity.SupportTableId = selectedSupportDocId;
        }
    }

    // Perbaikan logika sorting tanggal
    var tasksSortedFrom = activity.Tasks?.Where(t => t.From != null && t.DeletedAt == null)?.ToList();
    var tasksSortedTo = activity.Tasks?.Where(t => t.From != null && t.DeletedAt == null)?.ToList();

    tasksSortedFrom?.Sort((a, b) => a.From?.CompareTo(b.From) ?? 0);
    activity.FromCache = tasksSortedFrom?.First()?.From;

    tasksSortedTo?.Sort((a, b) => b.To?.CompareTo(a.To) ?? 0);
    activity.ToCache = tasksSortedTo?.First()?.To;

    db.Update(activity);
    await db.SaveChangesAsync();
    return Results.Ok(activity);
});


// app.MapPost("/api/dashboard/activities", async (EngineeringActivity activity, AppDbContext db, HttpContext httpContext) =>
// {
//     var currentUser = await Fetcher.fetchUsersAsync();  // Ambil pengguna yang sedang login, atau gunakan userId yang relevan
//     var loggedInUserId = currentUser?.FirstOrDefault()?.Id;  // Ambil ID user yang login dari `httpContext` atau fetcher

//     foreach (var task in activity.Tasks)
//     {
//         if (task.CompletedDatePic.HasValue)
//         {
//             task.CompletedByPicId = loggedInUserId;  // Simpan user ID yang melakukan PIC done
//         }
//         if (task.CompletedDateSpv.HasValue)
//         {
//             task.CompletedBySpvId = loggedInUserId;  // Simpan user ID yang melakukan SPV done
//         }
//         if (task.CompletedDateManager.HasValue)
//         {
//             task.CompletedByManagerId = loggedInUserId;  // Simpan user ID yang melakukan Manager done
//         }
//     }

//     db.Update(activity);
//     await db.SaveChangesAsync();

//     return Results.Ok(activity);
// });



// GET: api/UserRole
app.MapGet("/api/userroles", async (AppDbContext db) =>
    await db.UserRoles.ToListAsync());



// get by user id 
// GET: api/UserRole/byUserId/5
app.MapGet("/api/userroles/byUserId/{userId}", async (AppDbContext db, int userId) =>
{
    var userRoles = await db.UserRoles
        .Where(ur => ur.UserId == userId)
        .ToListAsync();

    if (userRoles == null || !userRoles.Any())
    {
        return Results.NotFound(new { message = "User roles not found for this userId" });
    }

    return Results.Ok(userRoles);
});

// GET: api/UserRole/5
app.MapGet("/api/userroles/{id}", async (AppDbContext db, int id) =>
{
    var userRole = await db.UserRoles.FindAsync(id);

    if (userRole == null)
    {
        return Results.NotFound(new { message = "User role not found" });
    }

    return Results.Ok(userRole);
});

// POST: api/UserRole
app.MapPost("/api/userroles", async (AppDbContext db, UserRole userRole) =>
{
    db.UserRoles.Add(userRole);
    try
    {
        await db.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        // Mengganti StatusCode dengan Problem untuk menampilkan pesan error
        // return Results.Problem("Error creating user role", statusCode: 500, detail: ex.Message);
    }

    return Results.Created($"/api/userroles/{userRole.Id}", userRole);
});


// PUT: api/UserRole/5
app.MapPut("/api/userroles/{id}", async (AppDbContext db, int id, UserRole userRole) =>
{
    if (id != userRole.Id)
    {
        return Results.BadRequest(new { message = "UserRole ID mismatch" });
    }

    db.Entry(userRole).State = EntityState.Modified;

    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!db.UserRoles.Any(e => e.Id == id))
        {
            return Results.NotFound(new { message = "UserRole not found" });
        }
        else
        {
            // Mengganti StatusCode dengan Problem untuk menampilkan pesan error
            // return Results.Problem("Error updating user role", statusCode: 500);
        }
    }

    return Results.NoContent();
});

// DELETE: api/UserRole/5
app.MapDelete("/api/userroles/{id}", async (AppDbContext db, int id) =>
{
    var userRole = await db.UserRoles.FindAsync(id);
    if (userRole == null)
    {
        return Results.NotFound(new { message = "UserRole not found" });
    }

    db.UserRoles.Remove(userRole);
    try
    {
        await db.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        // Mengganti StatusCode dengan Problem untuk menampilkan pesan error
        // return Results.Problem("Error deleting user role", statusCode: 500, detail: ex.Message);
    }

    return Results.NoContent();
});


// ended endpoint




app.MapGet("/api/dashboard/ecn-data", async (AppDbContext db) =>
    await db.ECNData.OrderBy(e => e.Month).ToListAsync());

app.MapGet("/api/dashboard/ccn-data", async (AppDbContext db) =>
    await db.CCNData.OrderBy(c => c.Month).ToListAsync());

app.MapGet("/populate", async (AppDbContext dbContext) =>
{
    if (!dbContext.DashboardMetrics.Any())
    {
        dbContext.DashboardMetrics.Add(new DashboardMetrics
        {
            NumberOfManpowers = 50,
            NumberOfECNsThisMonth = 12,
            NumberOfCCNsThisMonth = 8,
            LastDetectedProcess = DateTime.Now.AddDays(60),
            TotalProducts = 150,
            TotalBOMs = 75,
            TotalSLDs = 50,
            TotalDrawings = 200
        });

        dbContext.EngineeringActivities.AddRange(
            new EngineeringActivity { PIC = "Agus", Customer = "Moratel", Description = "Completed initial design for project X", Date = DateTime.Today },
            new EngineeringActivity { PIC = "Hendi", Customer = "Telkomsigma", Description = "Reviewed project status and deliverables", Date = DateTime.Today }
            // Add more sample activities...
        );

        var months = new[] { "February", "March", "April", "May", "June" };
        var random = new Random();

        for (int i = 0; i < 5; i++)
        {
            dbContext.ECNData.Add(new ECNData { Month = months[i], Count = random.Next(1, 20) });
            dbContext.CCNData.Add(new CCNData { Month = months[i], Count = random.Next(1, 20) });
        }

        dbContext.SaveChanges();
    }
});


//panel process endpoint
// Create a new PanelProcess
app.MapPost("/panelprocess", async (PanelProcess panelProcess, AppDbContext db) =>
{
    db.PanelProcesses.Add(panelProcess);
    await db.SaveChangesAsync();
    return Results.Created($"/panelprocess/{panelProcess.Id}", panelProcess);
});

// Get all PanelProcesses
app.MapGet("/panelprocess", async (AppDbContext db) =>
    await db.PanelProcesses.ToListAsync());

// Get a single PanelProcess by ID
app.MapGet("/panelprocess/{id}", async (AppDbContext db, int id) =>
{
    var panelProcess = await db.PanelProcesses.FindAsync(id);
    return panelProcess is not null ? Results.Ok(panelProcess) : Results.NotFound();
});

// Update a PanelProcess by ID
app.MapPut("/panelprocess/{id}", async (int id, PanelProcess panelProcess, AppDbContext db) =>
{
    if (id != panelProcess.Id) return Results.BadRequest("ID mismatch.");

    var existingPanelProcess = await db.PanelProcesses.FindAsync(id);
    if (existingPanelProcess is null) return Results.NotFound("PanelProcess not found.");

    existingPanelProcess.PanelType = panelProcess.PanelType;
    existingPanelProcess.ProcessName = panelProcess.ProcessName;
    existingPanelProcess.Minutes = panelProcess.Minutes;

    await db.SaveChangesAsync();
    return Results.Ok(existingPanelProcess);
});

// Delete a PanelProcess by ID
app.MapDelete("/panelprocess/{id}", async (AppDbContext db, int id) =>
{
    var panelProcess = await db.PanelProcesses.FindAsync(id);
    if (panelProcess is null) return Results.NotFound("PanelProcess not found.");

    db.PanelProcesses.Remove(panelProcess);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapGet("/api/generations", async (AppDbContext db, int? inquiryId) =>
{
    var generationQ = db.InquiryAIGenerations.AsQueryable();

    if (inquiryId != null)
    {
        generationQ = generationQ.Where(i => i.InquiryId == inquiryId);
    }

    return generationQ.ToList();
});


app.MapPost("/generations", async (AppDbContext db, InquiryAIGeneration inq) =>
{
    db.InquiryAIGenerations.Add(inq);
    await db.SaveChangesAsync();
    return Results.Ok(inq);
});



// notifikasi
app.MapPost("/api/notifications", async (Notification notification, AppDbContext db) =>
{
    notification.CreatedAt = DateTime.Now;
    db.Notifications.Add(notification);
    await db.SaveChangesAsync();
    return Results.Ok(notification);
});

app.MapGet("/api/notifications", async (AppDbContext db) =>
{
    var notifications = await db.Notifications
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();
    return Results.Ok(notifications);
});
app.MapGet("/api/notifications/active", async (AppDbContext db, string role) =>
{
    var notifications = await db.Notifications
        .Where(n => n.Role == role && n.Status == "OnGoing")
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();
    return Results.Ok(notifications);
});
app.MapPut("/api/notifications/done", async (int taskId, string role, AppDbContext db) =>
{
    var notification = await db.Notifications
        .FirstOrDefaultAsync(n => n.TaskId == taskId && n.Role == role && n.Status == "OnGoing");

    if (notification == null)
        return Results.NotFound(new { message = "Notification not found" });

    notification.IsRead = true;

    string nextRole = role switch
    {
        "pic" => "spv",
        "spv" => "manager",
        _ => null
    };

    if (nextRole != null)
    {
        // Tambahkan notifikasi baru untuk role berikutnya
        var newNotification = new Notification
        {
            Title = "Task Update",
            Message = $"Task dengan ID {taskId} siap untuk dilakukan done oleh {nextRole.ToUpper()}.",
            TaskId = taskId,
            Role = nextRole,
            CreatedAt = DateTime.Now,
            Status = "OnGoing"
        };
        db.Notifications.Add(newNotification);
    }
    else
    {
        // Tandai notifikasi sebagai selesai jika role terakhir menyelesaikan
        notification.Status = "Completed";
    }

    await db.SaveChangesAsync();
    return Results.Ok(notification);
});
app.MapPut("/api/notifications/undone", async (int taskId, string role, AppDbContext db) =>
{
    var notification = await db.Notifications
        .FirstOrDefaultAsync(n => n.TaskId == taskId && n.Role == role);

    if (notification == null)
        return Results.NotFound(new { message = "Notification not found" });

    notification.IsRead = false;
    notification.Status = "OnGoing";

    await db.SaveChangesAsync();
    return Results.Ok(notification);
});
app.MapDelete("/api/notifications/{id}", async (int id, AppDbContext db) =>
{
    var notification = await db.Notifications.FindAsync(id);
    if (notification == null)
        return Results.NotFound(new { message = "Notification not found" });

    db.Notifications.Remove(notification);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapGet("/api/notifications/task/{taskId}", async (AppDbContext db, int taskId) =>
{
    var notifications = await db.Notifications
        .Where(n => n.TaskId == taskId)
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();
    return Results.Ok(notifications);
});

app.MapPost("/api/tasks", async (SupportReportAPI.Models.Task task, AppDbContext db) =>
{
    task.CreatedAt = DateTime.Now;
    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    // Tambahkan notifikasi untuk PIC
    var notification = new Notification
    {
        Title = "Task Baru",
        Message = $"Task dengan ID {task.Id} baru saja dibuat dan perlu dilakukan done oleh PIC.",
        TaskId = (int)task.Id,
        Role = "pic",
        CreatedAt = DateTime.Now,
        Status = "OnGoing"
    };

    db.Notifications.Add(notification);
    await db.SaveChangesAsync();

    return Results.Ok(task);
});


app.MapGet("/api/notifications/dashboard", async (AppDbContext db) =>
{
    var notifications = await db.Notifications
        .Where(n => n.Status == "OnGoing")
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();

    return Results.Ok(notifications);
});
app.MapGet("/api/notifications/{id}", async (AppDbContext db, int id) =>
{
    var notification = await db.Notifications.FindAsync(id);
    if (notification == null)
        return Results.NotFound(new { message = "Notification not found" });

    return Results.Ok(notification);
});

// support endpoint
var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadDirectory))
{
    Directory.CreateDirectory(uploadDirectory);
}

// Get all SupportTables
app.MapGet("/supporttables", async (AppDbContext db) =>
    await db.SupportTables.ToListAsync());

// Get a SupportTable by ID
app.MapGet("/supporttables/{id:int}", async (AppDbContext db, int id) =>
{
    var supportTable = await db.SupportTables.FindAsync(id);
    return supportTable is not null ? Results.Ok(supportTable) : Results.NotFound();
});
app.MapGet("/supporttables/download/{fileName}", async (string fileName, HttpContext httpContext) =>
{
    var filePath = Path.Combine("uploads", fileName); // Sesuaikan path folder penyimpanan
    if (!File.Exists(filePath))
    {
        return Results.NotFound(new { message = "File not found." });
    }

    var contentType = SupportReportAPI.Helpers.FileHelper.GetContentType(filePath);
    return Results.File(await File.ReadAllBytesAsync(filePath), contentType, fileName);
});

// Create a new SupportTable
app.MapPost("/supporttables", async (HttpContext context, AppDbContext db) =>
{
    var form = await context.Request.ReadFormAsync();
    var name = form["name"].ToString();
    var file = form.Files["file"];

    if (string.IsNullOrEmpty(name) || file == null)
    {
        return Results.BadRequest("Name and file are required.");
    }

    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    var filePath = Path.Combine("uploads", fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var supportTable = new SupportTable
    {
        Name = name,
        FilePath = fileName
    };

    db.SupportTables.Add(supportTable);
    await db.SaveChangesAsync();

    return Results.Created($"/supporttables/{supportTable.Id}", supportTable);
});


// Update a SupportTable by ID
app.MapPut("/supporttables/{id}", async (int id, SupportTable supportTable, AppDbContext db) =>
{
    if (id != supportTable.Id)
    {
        return Results.BadRequest(new { message = "ID mismatch." });
    }

    var existingSupport = await db.SupportTables.FindAsync(id);
    if (existingSupport is null) return Results.NotFound("Support Table not found.");

    existingSupport.Name = supportTable.Name;

    await db.SaveChangesAsync();
    return Results.Ok(existingSupport);
});

// Delete a SupportTable by ID
app.MapDelete("/supporttables/{id:int}", async (AppDbContext db, int id) =>
{
    var supportTable = await db.SupportTables.FindAsync(id);
    if (supportTable is null) return Results.NotFound("SupportTable not found.");

    // Hapus file jika ada
    if (!string.IsNullOrEmpty(supportTable.FilePath))
    {
        var fullPath = Path.Combine(uploadDirectory, supportTable.FilePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    db.SupportTables.Remove(supportTable);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Upload File untuk SupportTable
app.MapPost("/supporttables/{id:int}/upload", async (int id, HttpContext context, AppDbContext db) =>
{
    var supportTable = await db.SupportTables.FindAsync(id);
    if (supportTable == null) return Results.NotFound("SupportTable not found.");

    var file = context.Request.Form.Files.FirstOrDefault();
    if (file == null) return Results.BadRequest("No file uploaded.");

    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    var filePath = Path.Combine(uploadDirectory, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    supportTable.FilePath = fileName;
    await db.SaveChangesAsync();

    return Results.Ok(new { Message = "File uploaded successfully", FilePath = fileName });
});

// Download File untuk SupportTable
app.MapGet("/supporttables/{id:int}/download", async (int id, AppDbContext db) =>
{
    var supportTable = await db.SupportTables.FindAsync(id);
    if (supportTable == null || string.IsNullOrEmpty(supportTable.FilePath))
        return Results.NotFound("File not found.");

    var filePath = Path.Combine("uploads", supportTable.FilePath);
    if (!File.Exists(filePath))
        return Results.NotFound("File not found.");

    return Results.File(await File.ReadAllBytesAsync(filePath), FileHelper.GetContentType(filePath), supportTable.FilePath);
});

// // Fungsi untuk mendapatkan Content-Type berdasarkan ekstensi file
// string GetContentType(string filePath)
// {
//     var ext = Path.GetExtension(filePath).ToLowerInvariant();
//     return ext switch
//     {
//         ".txt" => "text/plain",
//         ".pdf" => "application/pdf",
//         ".jpg" => "image/jpeg",
//         ".png" => "image/png",
//         ".zip" => "application/zip",
//         ".doc" => "application/msword",
//         ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//         ".xls" => "application/vnd.ms-excel",
//         ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//         _ => "application/octet-stream",
//     };
// });



// EndOfStreamException notifikasi 

app.Run();

static void SaveBase64ToFile(string base64String, string filePath)
{
    try
    {
        // Strip off the metadata (if any) from the base64 string (e.g., "data:image/png;base64,")
        string cleanBase64 = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;

        // Convert Base64 string to byte array
        byte[] fileBytes = Convert.FromBase64String(cleanBase64);

        // Write the byte array to the specified file
        File.WriteAllBytes(filePath, fileBytes);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

namespace SupportReportAPI.Models
{
    public class BaseModel
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

    }

    public class EngineerSupport : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? Engineer { get; set; }
        public string? Project { get; set; }
        public string? Requester { get; set; }
        public string? SupportDates { get; set; }
    }

    public class SupportDetail : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? No { get; set; }
        public DateTime? Tgl { get; set; }
        public string? PoNumber { get; set; }
        public string? Cust { get; set; }
        public string? ProjectName { get; set; }
        public string? Engineering { get; set; }
        public string? DetailProblem { get; set; }
        public string? Pn { get; set; }
        public string? Description { get; set; }
        public int? Qty { get; set; }
        public string? Uom { get; set; }
    }

    public class OutstandingPostPO : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? Customer { get; set; }
        public string? DoneOuts { get; set; }
        public string? Project { get; set; }
        public int? DaysDeadline { get; set; }
        public int? OthersOuts { get; set; }
        public int? OthersProcess { get; set; }
        public int? PostPoOuts { get; set; }
        public int? PostPoProcess { get; set; }
        public int? PrePoOuts { get; set; }
        public int? PrePoProcess { get; set; }
        public int? TotalOuts { get; set; }
        public int? TotalProcess { get; set; }
    }

    public class DonePO : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? JobCategory { get; set; }
        public string? Detail { get; set; }
        public int? Outs { get; set; }
        public double? TProcess { get; set; }
    }

    public class Configuration : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ByValuePercentage { get; set; }
        public double? Value { get; set; }
        public double? Percentage { get; set; }

    }

    public class EngineeringDetailProblem : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? No { get; set; }
        public DateTime? Tgl { get; set; }
        public string? PoNumber { get; set; }
        public string? Cust { get; set; }
        public string? ProjectName { get; set; }
        public string? Engineering { get; set; }
        public string? DetailProblem { get; set; }
        public string? Pn { get; set; }
        public string? Description { get; set; }
        public int? Qty { get; set; }
        public string? Uom { get; set; }
        public double? Cost { get; set; }

        public int? ExtJobId { get; set; }
        public int? ExtPurchaseOrderId { get; set; }
        public int? ExtItemId { get; set; }
        public int? Type { get; set; } // 0 = penambahan, 1 = pengurangan
        public int? ExtUserId { get; set; }
        public List<EngineeringDetailProblemItem>? Items { get; set; }
        public int? TypeEcnCcn { get; set; } // 0 = ecn, 1 = ccn
        public List<EngineeringDetailProblemApproval>? Approvals { get; set; }
        public int? ExtPanelCodeId { get; set; }

        // Approval
        public int? ApprovalExtUserId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? Status { get; set; } // 0 = outs, 1 = accepted, 2 = rejected
        public string? ApprovalRemark { get; set; }
        public string? ApprovalFileName { get; set; }
        public bool? HasPo { get; set; }

    }

    public class EngineeringDetailProblemItem : BaseModel
    {
        [Key]
        public int? Id { get; set; }

        public int? ExtItemId { get; set; }
        public double? Qty { get; set; }
        public int? EngineeringDetailProblemId { get; set; }
        public EngineeringDetailProblem? EngineeringDetailProblem { get; set; }
        public DateTime? SnapshotDate { get; set; }
        public Double? SnapshotPrice { get; set; }
        public int? TypeIncreaseDecrease { get; set; } // 0 = increase, 1 = decrease
    }
    public class EngineeringDetailProblemApproval : BaseModel
    {
        [Key]
        public int? Id { get; set; }

        public int? ExtUserId { get; set; }
        public int? EngineeringDetailProblemId { get; set; }
        public EngineeringDetailProblem? EngineeringDetailProblem { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? Status { get; set; } // 0 = outs, 1 = accepted, 2 = rejected

    }

    public class BomApproval : BaseModel
    {
        [Key]
        public int? Id { get; set; }

        public int? ExtUserId { get; set; }
        public int? ExtBomLeveledId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? Remark { get; set; }
        public int? Status { get; set; } // 0 = outs, 1 = accepted, 2 = rejected
        public List<BomApprovalPic>? Pics { get; set; }
    }

    public class BomApprovalPic : BaseModel
    {
        [Key]
        public int? Id { get; set; }

        public int? ExtUserId { get; set; }
        public int? ExtBomLeveledId { get; set; }
        public BomApproval? BomApproval { get; set; }
        public int? BomApprovalId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? Remark { get; set; }
        public int? Status { get; set; } // 0 = outs, 1 = accepted, 2 = rejected

    }





    public class DashboardMetrics : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public int? NumberOfManpowers { get; set; }
        public int? NumberOfECNsThisMonth { get; set; }
        public int? NumberOfCCNsThisMonth { get; set; }
        public DateTime? LastDetectedProcess { get; set; }
        public int? TotalProducts { get; set; }
        public int? TotalBOMs { get; set; }
        public int? TotalSLDs { get; set; }
        public int? TotalDrawings { get; set; }
    }

    public enum EngineeringActivityType
    {
        PrePO = 0,
        PostPO = 1,
        Others = 2
    }

    public class EngineeringActivity : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? PIC { get; set; }
        public string? Customer { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public List<Task>? Tasks { get; set; }
        public EngineeringActivityType Type { get; set; }
        public DateTime? FromCache { get; set; }
        public DateTime? ToCache { get; set; }
        public int? ExtJobId { get; set; }
        public int? ExtInquiryId { get; set; }
        public int? ExtPanelCodeId { get; set; }
        public int? ExtPurchaseOrderId { get; set; }
        
        [JsonPropertyName("selectedSupportDocId")]
        public int? SupportTableId { get; set; } 
    }
    public class Task : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? Description { get; set; }
        public EngineeringActivity? EngineeringActivity { get; set; }
        public int? EngineeringActivityId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<InCharge>? InCharges { get; set; }
        public double? Hours { get; set; }

        // Field existing untuk completed date
        public DateTime? CompletedDateSpv { get; set; }
        public DateTime? CompletedDatePic { get; set; }
        public DateTime? CompletedDateManager { get; set; }

        // Field baru untuk menyimpan user ID yang melakukan "done"
        public int? CompletedByPicId { get; set; }
        public int? CompletedBySpvId { get; set; }
        public int? CompletedByManagerId { get; set; }

        // Field existing lainnya
        public string? Remark { get; set; }
        public int? ExtPanelCodeId { get; set; }

        // Field baru untuk soft delete
        public DateTime? DeletedAt { get; set; }
    }

    public class InCharge : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? PIC { get; set; }
        public string? Customer { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public Task? Task { get; set; }
        public int? TaskId { get; set; }

        public int? ExtUserId { get; set; }

        [NotMapped]
        public string? PicName { get; set; }
    }


    public class ECNData : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? Month { get; set; }
        public int? Count { get; set; }
    }

    public class CCNData : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? Month { get; set; }
        public int? Count { get; set; }
    }

    public class EngDeptConfig : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public int? DepartmentId { get; set; }
    }
    public class InquiryAIGeneration : BaseModel
    {
        [Key]
        public int? Id { get; set; }
        public string? FileName { get; set; }
        public string? Model { get; set; }
        public int? InquiryId { get; set; }
        public string? Content { get; set; }
    }

        public class SupportTable : BaseModel
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; }

            public string? FilePath { get; set; } // Path untuk menyimpan file
        }

        public class SupportEngineeringDocument : BaseModel
        {
            [Key]
            public int Id { get; set; }  // Tidak nullable, karena ini adalah kunci utama

            public int? SupportTableId { get; set; }  // Bisa nullable jika diperlukan

            public int JobId { get; set; }  // ID dari pekerjaan yang diambil

            public string JobName { get; set; }  // Nama pekerjaan (job)

            public DateTime CreatedAt { get; set; }  // Tanggal dan waktu pembuatan

            public DateTime UpdatedAt { get; set; }  // Tanggal dan waktu pembaruan terakhir
        }




    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<EngineerSupport> EngineerSupports { get; set; }
        public DbSet<SupportDetail> SupportDetails { get; set; }
        public DbSet<OutstandingPostPO> OutstandingPostPOs { get; set; }
        public DbSet<DonePO> DonePOs { get; set; }
        public DbSet<EngineeringDetailProblem> EngineeringDetailProblems { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<DashboardMetrics> DashboardMetrics { get; set; }
        public DbSet<EngineeringActivity> EngineeringActivities { get; set; }
        public DbSet<ECNData> ECNData { get; set; }
        public DbSet<CCNData> CCNData { get; set; }
        public DbSet<EngDeptConfig> EngDeptConfigs { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<InCharge> InCharges { get; set; }
        public DbSet<BomApproval> BomApprovals { get; set; }
        public DbSet<BomApprovalPic> BomApprovalPics { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<PanelProcess> PanelProcesses { get; set; }
        public DbSet<InquiryAIGeneration> InquiryAIGenerations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SupportTable> SupportTables { get; set; }
        public DbSet<SupportEngineeringDocument> SupportEngineeringDocuments { get; set; }


        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseModel && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseModel)entry.Entity).CreatedAt = DateTime.Now;
                }

                ((BaseModel)entry.Entity).UpdatedAt = DateTime.Now;
            }
        }
    }
}
public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "O"; // "O" stands for Round-trip date/time pattern (ISO 8601)

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var value = reader.GetString();
        return DateTime.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("o"));
    }
}



class LoginBody
{
    public string Username { get; set; }
    public string Password { get; set; }
}
class EngDetailProblemPhoto { public string? Photo { get; set; } }

class Fetcher
{
    public static async Task<List<AuthserverUser>> fetchUsersAsync()
    {
        var response = await new HttpClient().GetAsync($"https://authserver-backend.iotech.my.id/users/view");

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception(errorMessage);
        }

        var resp = await response.Content.ReadAsStringAsync();

        // Console.WriteLine(resp);

        return JsonSerializer.Deserialize<List<AuthserverUser>>(resp, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<List<CrmPurchaseOrder>> fetchCrmPurchaseOrdersAsync()
    {
        var response = await new HttpClient().GetAsync($"https://crm-local.iotech.my.id/api/external/purchase-orders/ppic");

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception(errorMessage);
        }

        var resp = await response.Content.ReadAsStringAsync();

        // Console.WriteLine($"PO: {resp}" );

        return JsonSerializer.Deserialize<List<CrmPurchaseOrder>>(resp, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }


    public static async Task<List<Inquiry>> fetchCrmInquiriesAsync()
    {
        var response = await new HttpClient().GetAsync($"https://backend-crm.iotech.my.id/api/v1/external/inquiries");

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception(errorMessage);
        }

        var resp = await response.Content.ReadAsStringAsync();

        // Console.WriteLine($"Inq: {resp}" );

        return JsonSerializer.Deserialize<List<Inquiry>>(resp, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }






}


class AuthserverUser
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}

public class CrmPurchaseOrder
{
    public int? Id { get; set; }
    public string? purchaseOrderNumber { get; set; }
    public Account? Account { get; set; }

}


public class Account
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}

public class Inquiry
{
    public int? Id { get; set; }
    public string? InquiryNumber { get; set; }
    public string? Title { get; set; }
    public string? Project { get; set; }
    public Account? Account { get; set; }
    // public string? Sales { get; set; }
    public Quotation? Quotation { get; set; }
}

public class Quotation
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}

public class UserRole
{
    public int Id { get; set; }        // Primary key
    public int UserId { get; set; }     // UserId untuk relasi ke tabel pengguna
    public string Role { get; set; }    // Role yang dimiliki pengguna (PIC, SPV, Manager)
}
public class PanelProcess
{
    [Key]
    public int Id { get; set; } // Primary key for identification
    public string PanelType { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public double Minutes { get; set; }
}

public class Notification : BaseModel
{
    [Key]
    public int Id { get; set; } // Primary Key
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int TaskId { get; set; } // Relasi ke Task
    public string Role { get; set; } = string.Empty; // Role terkait notifikasi (PIC, SPV, Manager)
    public bool IsRead { get; set; } = false; // Status apakah notifikasi sudah dibaca
    public string Status { get; set; } = "OnGoing"; // Status notifikasi (OnGoing, Completed)
}


// dto tambahan 
public class InChargeDto
{
    public int? Id { get; set; }
    public int? ExtUserId { get; set; }
    public string? PicName { get; set; }
}

public class TaskDto
{
    public int? Id { get; set; }
    public string? Description { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public double? Hours { get; set; }
    public string? Remark { get; set; }
    public DateTime? CompletedDatePic { get; set; }
    public DateTime? CompletedDateSpv { get; set; }
    public DateTime? CompletedDateManager { get; set; }
    public int? CompletedByPicId { get; set; }
    public int? CompletedBySpvId { get; set; }
    public int? CompletedByManagerId { get; set; }
    public List<InChargeDto>? InCharges { get; set; }
}

public class ActivityDto
{
    public int? Id { get; set; }
    public string? Description { get; set; }
    public EngineeringActivityType Type { get; set; }
    public int? ExtInquiryId { get; set; }
    public int? ExtPurchaseOrderId { get; set; }
    public DateTime? FromCache { get; set; }
    public DateTime? ToCache { get; set; }
    public List<TaskDto>? Tasks { get; set; }
    public int? ExtJobId { get; set; }

    public int? ExtPanelCodeId { get; set; }
    public int? SupportTableId { get; set; } 
}
public class SupportEngineeringDocumentDto
{
    public int JobId { get; set; }  // ID dari pekerjaan yang diambil
    public string JobName { get; set; }  // Nama pekerjaan (job)
    public int[] SupportTableIds { get; set; }  // Array of SupportTableId
}
