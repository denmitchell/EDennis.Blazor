using EDennis.BlazorHits;
using EDennis.BlazorHits.Services;
using EDennis.BlazorUtils;
using EDennis.BlazorUtils.Security.SimpleAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);


// Conditionally add support for faking a user, which must be registered
// in AppUser table

#if DEBUG
var fakeUser = builder.Configuration["FakeUser"];
if (fakeUser != null)
    builder.Services.AddFakeUserAuthentication();
else
{
#endif

    builder.Configuration.AddJsonEnvironmentVariable(
        $"{typeof(Program).Assembly.GetName().Name}.Configuration");

    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

#if DEBUG
}
#endif

//Add simple security based upon AppUser and AppRoles tables
// (includes registering the DbContext, AppUserService and AppRoleService
builder.AddUserRolesSecurity<AppUserRolesContext>();

//Add CRUD services
builder.AddCrudServices<HitsContext>()
    .AddCrudService<ArtistService, Artist>()
    .AddCrudService<SongService, Song>();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

ServiceInspector.ServiceDescriptors = builder.Services.Select(s => new ServiceDescriptor(s.ServiceType, s.ImplementationType ?? s.ServiceType, s.Lifetime));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//app.UseAppUserRoles<AppUserRolesContext>();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
