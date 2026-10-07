using FluentValidation;
using LocalAiLibrary.AiLibrary;
using LocalAiLibrary.AiLibrary.AiDbService;
using LocalAiLibrary.AiLibrary.AiSessionService;
using LocalAiLibrary.AiLibrary.AITools;
using LocalAiLibrary.AiLibrary.CategoryClassification;
using LocalAiLibrary.AiLibrary.CategoryClassification.AiCategoryDbService;
using LocalAiLibrary.AiLibrary.CategoryClassification.AiCategoryService;
using LocalAiLibrary.AiLibrary.CategoryClassification.Models;
using LocalAiLibrary.AiLibrary.ChatService;
using LocalAiLibrary.AiLibrary.ChatService.Models;
using LocalAiLibrary.AiLibrary.EmailChatService;
using LocalAiLibrary.AiLibrary.Network;
using LocalAiLibrary.AiLibrary.Orchestration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Email;
using Utilities.Email.EmailOperations;
using Utilities.Email.Models;
using Utilities.Logger;
using Utilities.Network;
using Utilities.Repo;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddHttpContextAccessor();
services.AddDistributedMemoryCache();
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

services.Configure<AiConfig>(config.GetSection("AI-Config"));
services.Configure<AiChatCategoryConfig>(config.GetSection("AI-ChatCategory"));
services.Configure<EmailConfig>(config.GetSection("Email-Config"));

services.AddValidatorsFromAssembly(typeof(AiManager).Assembly);

services.AddTransient<IAiManager, AiManager>();
services.AddTransient<IChatSessionService, ChatSessionService>();
services.AddTransient<IAiCommunicationManager, AiCommunicationManager>();
services.AddTransient<IEmail, Email>();
services.AddTransient<IUtilLogger, Logger>();
services.AddTransient<INetwork, Network>();
services.AddTransient<IRepo, Repository>();
services.AddTransient<IAiNetwork, AiNetwork>();
services.AddTransient<IAiChatCategoryRepo, AiChatCategoryRepo>();
services.AddTransient<IManageChatEntryClassificationService, ManageChatEntryClassificationService>();
services.AddTransient<IAiCategoryManager, AiCategoryManager>();
services.AddTransient<IOutstandingChatEmails, OutStandingChatEmails>();
services.AddTransient<IAiRepo, AiRepoManager>();
services.AddTransient<IOrchestrationPlanManagement, OrchestrationPlanManagement>();
services.AddTransient<IToolSelection, ToolSelection>();
services.AddTransient<IPromptHelper, PromptHelper>();
services.AddTransient<ISessionStore, SessionStore>();
services.AddTransient<IEmailDelete, EmailDelete>();
services.AddTransient<IEmailFetch, EmailFetch>();
services.AddTransient<IEmailManager, EmailManager>();

var provider = services.BuildServiceProvider();
var chatClassificationService = provider.GetRequiredService<IAiManager>();
await chatClassificationService.ProcessChatEmails();