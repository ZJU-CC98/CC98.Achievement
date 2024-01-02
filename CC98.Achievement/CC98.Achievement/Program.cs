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

// ���ػ�����
builder.Services.AddLocalization()
	.AddDynamicLocalizer();

// AddMessage services to the container.
builder.Services.AddControllersWithViews()
	.AddMvcLocalization(options => options.ResourcesPath = "Resources")
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


// �����֤
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

	// ������Ӧ�ó���ʹ�õĿͻ���������֤
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
	{
		options.Authority = builder.Configuration["Authentication:IdentityServer:Authority"];
		options.TokenValidationParameters.ValidateAudience = false;
	});

// �ⲿ��¼ϵͳ
builder.Services.AddExternalSignInManager();

// ��Ȩϵͳ
builder.Services.AddAuthorization(options =>
{
	// �����ɫȨ���顣����ͨ�ù���Ա������Ĭ����ӡ�
	void AddPolicyWithDefault(string name, params string[] roles)
	{
		// ͨ�ù���Ա��
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

// ���ݱ�������
builder.Services.AddDataProtection();

// ������Ϣ
builder.Services.AddSingleton<IOperationMessageLevelClassMapper, SemanticUIMessageLevelClassMapper>();
builder.Services.AddSingleton<IOperationMessageHtmlGenerator, SemanticUIMessageHtmlGenerator>();
builder.Services.AddOperationMessages();

// TempData ��ǿ
builder.Services.AddEnhancedTempData(options =>
{
	options.EnableHtmlContentSerialization();
});


// ��ҳ����
builder.Services.AddTransient<IPagerHtmlGenerator, SemanticUIPagerGenerator>();
builder.Services.AddBootstrapPagerGenerator(options => options.ConfigureDefault());



// ��̨����
builder.Services.AddSingleton<AchievementBackService>();

// ϵͳ���á�
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

// ϵͳ����
builder.Services.Configure<DataUpdatePeriodSetting>(builder.Configuration.GetSection("DataUpdatePeriod"));

// ��ʱ����
builder.Services.AddHostedService<UpdateUserCountService>();

// API �ĵ�
builder.Services.AddSwaggerGen(options =>
{

	options.SwaggerDoc("v1", new() { Title = "CC98 �ɾ�ϵͳ�����������ĵ�", Version = "v1" });

	options.AddSecurityDefinition("CC98 ��¼ϵͳ", new()
	{
		Description = "ʹ�� <a href\"https://openid.cc98.org\">CC98 ��¼ϵͳ</a>ִ�� API �����Ȩ��",
		Type = SecuritySchemeType.OpenIdConnect,
		OpenIdConnectUrl =
			new(
				new(builder.Configuration["Authentication:IdentityServer:Authority"] ??
					throw new InvalidOperationException("δ���� IdentityServer ��Ȩ������ַ��")),
				OidcConstants.Discovery.DiscoveryEndpoint)
	});

	var xmlFileList = new List<string>();

	// ����ĵ��ļ�
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

// HTTP ������д
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
	options.SwaggerEndpoint("v1/swagger.json", "CC98 �ɾ�ϵͳ API");
});

app.Run();
