using CC98;
using CC98.Achievement;
using CC98.Achievement.Data;
using CC98.Achievement.Documentation;
using CC98.Achievement.Services;
using CC98.Authentication.OpenIdConnect;

using IdentityModel;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;

using Sakura.AspNetCore.Localization;
using Sakura.AspNetCore.Mvc;

using System.Text.Json.Serialization;
using CC98.Achievement.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<AchievementDbContext>(builder.Configuration.GetConnectionString("Achievement"));

// 本地化服务
builder.Services.AddLocalization()
	.AddDynamicLocalizer();

// AddMessage services to the container.
builder.Services.AddControllersWithViews()
	.AddMvcLocalization(options => options.ResourcesPath = "Resources")
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


// 身份认证
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
	.AddCookie(IdentityConstants.ApplicationScheme, options =>
	{
		options.Cookie.HttpOnly = true;
		options.Cookie.SameSite = SameSiteMode.Lax;
		options.Cookie.SecurePolicy = CookieSecurePolicy.None;

		options.LoginPath = "/Account/LogOn";
		options.LogoutPath = "/Account/LogOff";
		options.AccessDeniedPath = "/Home/AccessDenied";

	})
	.AddCookie(IdentityConstants.ExternalScheme)
	.AddCC98(CC98Defaults.AuthenticationScheme, options =>
	{
		options.ClientId = builder.Configuration["Authentication:CC98:ClientId"];
		options.ClientSecret = builder.Configuration["Authentication:CC98:ClientSecret"];
		options.Authority = builder.Configuration["Authentication:CC98:Authority"];
		options.ResponseType = OpenIdConnectResponseType.CodeIdTokenToken;
		options.SignInScheme = IdentityConstants.ExternalScheme;
		options.CallbackPath = "/signin-cc98";

		options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
	})

	// 第三方应用程序使用的客户端令牌验证
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
	{
		options.Authority = builder.Configuration["Authentication:IdentityServer:Authority"];
		options.TokenValidationParameters.ValidateAudience = false;
	});

// 外部登录系统
builder.Services.AddExternalSignInManager();

// 授权系统
builder.Services.AddAuthorization(options =>
{
	// 定义角色权限组。其中通用管理员组总是默认添加。
	void AddPolicyWithDefault(string name, params string[] roles)
	{
		// 通用管理员组
		var defaultAdminRoles = new[]
			{Policies.Roles.GeneralAdministrators, Policies.Roles.Administrators, Policies.Roles.Operators};

		options.AddPolicy(name, pb => pb.RequireRole(defaultAdminRoles.Concat(roles)));
	}

	AddPolicyWithDefault(Policies.Admin);
	AddPolicyWithDefault(Policies.Review, Policies.Roles.Reviewers);
	AddPolicyWithDefault(Policies.Edit, Policies.Roles.Editors);

	options.AddPolicy(Policies.ClientApp,
		pb => pb.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser());
});

// 数据保护功能
builder.Services.AddDataProtection();

// 操作消息
builder.Services.AddSingleton<IOperationMessageLevelClassMapper, SemanticUIMessageLevelClassMapper>();
builder.Services.AddSingleton<IOperationMessageHtmlGenerator, SemanticUIMessageHtmlGenerator>();
builder.Services.AddOperationMessages();

// TempData 增强
builder.Services.AddEnhancedTempData(options =>
{
	options.EnableHtmlContentSerialization();
});


// 分页功能
builder.Services.AddTransient<IPagerHtmlGenerator, SemanticUIPagerGenerator>();
builder.Services.AddBootstrapPagerGenerator(options => options.ConfigureDefault());



// 后台服务
builder.Services.AddSingleton<AchievementBackService>();

// 系统设置。
builder.Services.AddAppSetting<SystemSetting>(options =>
	{
		options.LoadMode = AppSettingLoadMode.Auto;
		options.SaveMode = AppSettingSaveMode.Auto;
	})
	.AddAccess(options =>
	{
		options.AppName = "CC98.Achievement";
		options.DataFormat = AppSettingFormats.Json;
	})
	.AddDbContext(options =>
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("CC98V2"));
	});

// 系统设置
builder.Services.Configure<DataUpdatePeriodSetting>(builder.Configuration.GetSection("DataUpdatePeriod"));

// 定时服务
builder.Services.AddHostedService<UpdateUserCountService>();

// API 文档
builder.Services.AddSwaggerGen(options =>
{

	options.SwaggerDoc("v1", new() { Title = "CC98 成就系统第三方接入文档", Version = "v1" });

	options.AddSecurityDefinition("CC98 登录系统", new()
	{
		Description = "使用 <a href\"https://openid.cc98.org\">CC98 登录系统</a>执行 API 身份授权。",
		Type = SecuritySchemeType.OpenIdConnect,
		OpenIdConnectUrl =
			new(
				new(builder.Configuration["Authentication:IdentityServer:Authority"] ??
					throw new InvalidOperationException("未配置 IdentityServer 授权机构地址。")),
				OidcConstants.Discovery.DiscoveryEndpoint)
	});

	var xmlFileList = new List<string>();

	// 添加文档文件
	foreach (var xmlFile in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
	{
		xmlFileList.Add(xmlFile);
		options.IncludeXmlComments(xmlFile);
	}

	options.SchemaFilter<EnumTypesSchemaFilter>(new object[] { xmlFileList.ToArray() });
	options.DocumentFilter<EnumTypesDocumentFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

// HTTP 方法重写
app.UseHttpMethodOverride();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapAreaControllerRoute(
	name: "api",
	areaName: "Api",
	pattern: "api/{controller}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{codeName?}");

app.MapSwagger();

app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("v1/swagger.json", "CC98 成就系统 API");
});

app.Run();
