using System.Reflection;
using System.Text;
using FirebaseAdmin;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.Dashboard;
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MyCSharp.HttpUserAgentParser.DependencyInjection;
using Newtonsoft.Json.Converters;
using Serilog;
using Serilog.Ui.MsSqlServerProvider;
using Serilog.Ui.Web;
using SimpleArchitecture;
using SimpleArchitecture.Auditing.Interceptors;
using SimpleArchitecture.Authentication.Configurations;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Authentication.Interfaces;
using SimpleArchitecture.Authentication.Middlewares;
using SimpleArchitecture.Authentication.Services;
using SimpleArchitecture.Authentication.Types;
using SimpleArchitecture.BackgroundJobs;
using SimpleArchitecture.BackgroundJobs.Activator;
using SimpleArchitecture.BackgroundJobs.Authorization;
using SimpleArchitecture.Common;
using SimpleArchitecture.Data;
using SimpleArchitecture.Data.Interfaces;
using SimpleArchitecture.Emailing.Configurations;
using SimpleArchitecture.Emailing.Enums;
using SimpleArchitecture.Emailing.Interfaces;
using SimpleArchitecture.Emailing.Services;
using SimpleArchitecture.EventHandling.Interceptors;
using SimpleArchitecture.ExceptionHandling.Middlewares;
using SimpleArchitecture.Internationalization.Interfaces;
using SimpleArchitecture.Internationalization.Middlewares;
using SimpleArchitecture.Internationalization.Services;
using SimpleArchitecture.IO.Filters;
using SimpleArchitecture.IO.Interfaces;
using SimpleArchitecture.IO.Services;
using SimpleArchitecture.Logging.Configurations;
using SimpleArchitecture.Logging.Filters;
using SimpleArchitecture.Logging.Middlewares;
using SimpleArchitecture.Notifications.Interfaces;
using SimpleArchitecture.Notifications.Services;
using SimpleArchitecture.Serialization.Configurations;
using SimpleArchitecture.Swagger.Enums;
using SimpleArchitecture.Swagger.Filters;
using SimpleArchitecture.TimeZones.Configurations;
using SimpleArchitecture.TimeZones.Filters;
using SimpleArchitecture.TimeZones.Interfaces;
using SimpleArchitecture.TimeZones.Services;
using SimpleArchitecture.Web.Configurations;
using SimpleArchitecture.Web.Interfaces;
using SimpleArchitecture.Web.Services;
using StackExchange.Profiling;
using AuthorizationMiddlewareResultHandler =
    SimpleArchitecture.Authentication.Middlewares.AuthorizationMiddlewareResultHandler;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

var builder = WebApplication.CreateBuilder(args);

var sqlServerConnection = builder.Configuration.GetConnectionString("SqlServer");

var sqlServerLoggingConnection = builder.Configuration.GetConnectionString("SqlServerLogging");

#region logging

var uiConfig = builder.Configuration.GetSection("SerilogUi").Get<SerilogUiConfig>();

builder
    .Host
    .UseSerilog((context, loggingConfig) => loggingConfig
        .ReadFrom
        .Configuration(context.Configuration));

builder.Services.AddSerilogUi(options => { options.UseSqlServer(sqlServerLoggingConnection!, uiConfig!.TableName); });

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

#endregion

#region Identity Configuration

builder.Services.AddIdentity<User, Role>(options =>
    {
        //identity configuration goes here
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@.$#+";
    })
    .AddSignInManager()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<SimpleArchitectureDbContext>()
    .AddDefaultTokenProviders();

#endregion

#region JWT

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped(services => services.GetRequiredService<IOptions<JwtConfig>>().Value);

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped(typeof(IClaimsProvider), typeof(ClaimsProvider));

builder
    .Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Constants.AuthenticationSchemes.JwtOrCookies;
        options.DefaultChallengeScheme = Constants.AuthenticationSchemes.JwtOrCookies;
    })
    .AddCookie(options =>
    {
        //cookie authentication options goes here
        //login, logout paths, and cookie expiration options
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secrets"]!)),
            RoleClaimType = nameof(ClaimType.Roles),
            NameClaimType = nameof(ClaimType.UserId),
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(Convert.ToInt32(builder.Configuration["JWT:ClockSkewInMinutes"]))
        };
    })
    .AddPolicyScheme(Constants.AuthenticationSchemes.JwtOrCookies,
        Constants.AuthenticationSchemes.JwtOrCookies, options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                var authorization = context.Request.Headers[HeaderNames.Authorization];
                if (!string.IsNullOrEmpty(authorization) && authorization.ToString().StartsWith("Bearer "))
                    return JwtBearerDefaults.AuthenticationScheme;

                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });

builder.Services.AddScoped<IExternalLoginIdTokenValidator, ExternalLoginIdTokenValidator>();

builder.Services.Configure<GoogleConfig>(builder.Configuration.GetSection("GoogleAuthSettings"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<GoogleConfig>>().Value);

#endregion

#region Otp

builder.Services.Configure<OtpConfig>(builder.Configuration.GetSection("Otp"));
builder.Services.AddScoped(services => services.GetRequiredService<IOptions<OtpConfig>>().Value);

#endregion

#region authorization

builder.Services.AddAuthorization(options => options.AddPolicies());

builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

#endregion

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

#region ef core config

builder.Services.SetupContext(sqlServerConnection!);

#endregion

#region fluent validation

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

// Add services to the container.

builder.Services.AddTransient<IContentTypeProvider, FileExtensionContentTypeProvider>();
builder.Services.AddTransient<IFileManager, FileManager>();

builder.Services.AddMediatR(config =>
{
    config.RegisterGenericHandlers = true;

    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<SimpleArchitectureDbContext>());

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<ActionAuditInterceptor>();
builder.Services.AddScoped<FireEventInterceptor>();

#endregion

#region fluentemail

builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));
builder.Services.AddScoped(services => services.GetRequiredService<IOptions<EmailConfig>>().Value);

var emailConfig = builder.Configuration.GetSection("EmailConfig").Get<EmailConfig>()!;

builder.Services
    .AddFluentEmail(emailConfig.DefaultFromEmail, emailConfig.DefaultFromName)
    .AddRazorRenderer()
    .AddSender(EmailSenderService.MailKit, emailConfig);

#endregion fluentemail

#region two factor auth

builder.Services.Configure<ProceedToTwoFactorAuthTokenConfig>(
    builder.Configuration.GetSection("ProceedToTwoFactorAuthToken"));
builder.Services.AddScoped(services =>
    services.GetRequiredService<IOptions<ProceedToTwoFactorAuthTokenConfig>>().Value);

builder.Services.Configure<TwoFactorAuthenticationConfig>(
    builder.Configuration.GetSection("TwoFactorAuthentication"));
builder.Services.AddScoped(services =>
    services.GetRequiredService<IOptions<TwoFactorAuthenticationConfig>>().Value);

#endregion

#region mapster

TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);

var config = TypeAdapterConfig.GlobalSettings;

config.Scan(Assembly.GetExecutingAssembly());

#endregion

#region fcm

// uncomment for firebase configuration, make sure you have the file in the right directory, and change the name in GoogleCredential.FromFile

// builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions
// {
//     Credential = GoogleCredential.FromFile("test-e94cc-firebase-adminsdk-2knqx-1773b5a27a.json")
// }));

// builder.Services.AddScoped<INotificationPusher, NotificationPusher>();

#endregion

builder.Services.AddScoped<IUserDeviceDetector, UserDeviceDetector>();
builder.Services.AddHttpUserAgentParser();

builder.Services
    .AddControllersWithViews()
    .AddNewtonsoftJson(opts => opts.SerializerSettings.Converters.Add(new StringEnumConverter()));

builder.Services.AddScoped<JsonSerializerConfigProvider>();

builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AuthorizeOperationFilter>();
    c.SwaggerDoc(nameof(ApiGroups.SimpleArchitecture),
        new OpenApiInfo { Title = "SimpleArchitecture", Version = "v1" });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
});

#region hangfire

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(sqlServerConnection);
    config.UseRecommendedSerializerSettings();
    config.UseSimpleAssemblyNameTypeSerializer();
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
});

builder.Services.AddHangfireServer();

#endregion

builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddExceptionHandler<ExceptionLoggingMiddleware>();
builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<LoggingMiddleware>();

#region Memory caching

builder.Services.AddMemoryCache();

#endregion

builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
builder.Services.AddScoped<AccountStatusMiddleware>();

builder.Services.AddScoped<LocalizationMiddleware>();

builder.Services.AddScoped<IBaseUrlProvider, BaseUrlProvider>();
builder.Services.AddScoped<IUserIpAddressProvider, UserIpAddressProvider>();

builder.Services.Configure<ProxyConfig>(builder.Configuration.GetSection("ProxyConfig"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<ProxyConfig>>().Value);

builder.Services.Configure<TimeZoneConfiguration>(builder.Configuration.GetSection("TimeZone"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<TimeZoneConfiguration>>().Value);
builder.Services.AddScoped<ITimeZoneProvider, TimeZoneProvider>();

builder.Services.AddScoped<IRequestLocaleProvider, RequestLocaleProvider>();

builder.Services.Configure<CorsConfiguration>(builder.Configuration.GetSection("CorsConfiguration"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<CorsConfiguration>>().Value);

#region miniprofiler

// todo: figure out how to authorize mini profiler in a ui or something
builder.Services.AddMiniProfiler(options =>
{
    async Task<bool> AuthorizeFunc(HttpRequest httpRequest)
    {
        var httpContext = httpRequest.HttpContext;

        var userId = httpContext.User.GetId();

        var userManager = httpContext.RequestServices
            .GetRequiredService<UserManager<User>>();

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null) return false;

        return await userManager.IsInRoleAsync(user, nameof(Roles.SystemAdministrator));
    }

    options.RouteBasePath = "/profiler";

    options.ColorScheme = ColorScheme.Dark;

    options.IgnorePath("/swagger");

    // options.ResultsAuthorizeAsync = AuthorizeFunc;
    //
    // options.ResultsListAuthorizeAsync = AuthorizeFunc;
}).AddEntityFramework();

#endregion

var app = builder.Build();

GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(app.Services));

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{nameof(ApiGroups.SimpleArchitecture)}/swagger.json", "SimpleArchitecture");
        c.DocumentTitle = "SimpleArchitecture";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiniProfiler();
app.UseMiddleware<LoggingMiddleware>();
app.UseExceptionHandler(_ => { });
app.UseMiddleware<LocalizationMiddleware>();

using var corsScope = app.Services.CreateScope();

var corsConfiguration = corsScope.ServiceProvider.GetRequiredService<CorsConfiguration>();

var allowedOrigins = corsConfiguration.AllowedOrigins.ToArray();

app.UseCors(
    options =>
    {
        if (app.Environment.IsDevelopment())
        {
            options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();

            return;
        }

        options.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    }
);

app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AccountStatusMiddleware>();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new IDashboardAuthorizationFilter[] { new HangfireDashboardAuthorization() }
});

app.UseSerilogUi(options =>
{
    options.Authorization.AuthenticationType = AuthenticationType.Jwt;

    options.Authorization.Filters = new[]
    {
        new LoggingUiAuthorizationFilter()
    };

    options.RoutePrefix = uiConfig!.RoutePrefix;
});

app.MapDefaultControllerRoute();
app.MapRazorPages();

await using var scope = app.Services.CreateAsyncScope();

await scope.ServiceProvider.SeedRoles();

await scope.ServiceProvider.CreateAppAdmin();

await scope.ServiceProvider.CreateSystemSupervisor();

var rootRouteGroup = app.MapGroup("")
    .AddEndpointFilter<TimeZoneActionFilter>()
    .AddEndpointFilter<RequiresBaseUrlFilter>();

rootRouteGroup.MapEndpoints();

JobsRegistry.RegisterSerilogCleanJob();

JobsRegistry.RegisterUserDevicesJob();

app.Run();