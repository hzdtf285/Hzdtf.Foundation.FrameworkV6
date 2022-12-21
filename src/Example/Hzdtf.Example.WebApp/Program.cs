using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hzdtf.BasicFunction.Controller.Extensions.RoutePermission;
using Hzdtf.Example.WebApp.AppStart;
using Hzdtf.Quartz.Extensions;
using Hzdtf.Utility;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
App.CurrConfig = builder.Configuration;
App.IsReturnCulture = true;
// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddControllersAsServices()
    .AddDefaultJsonOptions();
builder.Services.AddControllers()
    .AddControllersAsServices();

builder.Services.AddSession();

builder.Services.AddHzdtfLog();

builder.Services.AddIdentityAuth<int>(options =>
{
    options.LocalAuth.LoginPath = "/login.html";
});

builder.Services.AddTheReuestOperation();
builder.Services.AddRequestLog();
builder.Services.AddApiExceptionHandle();
builder.Services.AddRoutePermission();
builder.Services.AddQuartz(() =>
{
    QuartzStaticConfig.JobHandleExceptionAssembly = "Hzdtf.Example.Service.Impl";
});

if (builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo()
        {
            Title = "样例系统接口",
            Version = "v1"
        });

        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Hzdtf.Utility.xml"));
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Hzdtf.BasicController.xml"));
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Hzdtf.BasicFunction.Model.xml"));
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Hzdtf.BasicFunction.Controller.xml"));
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Hzdtf.Workflow.Model.xml"));
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Hzdtf.Workflow.Controller.xml"));
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Hzdtf.Example.Controller.xml"));
    });
}

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(conBuilder =>
{
    DependencyInjection.RegisterComponents(conBuilder);
});

var app = builder.Build();
App.Instance = app.Services;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
if (builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "样例系统 API");
    });
}

app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseTheReuestOperation();
app.UseCulture();
app.UseRoutePermission<RoutePermissionMiddleware>();
app.UseRequestLog();
app.UseIdentityAuth<int>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


OtherConfig.Init();

app.Run();
