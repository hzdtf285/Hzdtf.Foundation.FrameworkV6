using System;
using System.Net.Http;
using Autofac;
using FoxUC.Autofac.Extensions;
using FoxUC.BasicFunction.Service.Impl;
using FoxUC.BasicFunction.Service.Impl.Expand.Attachment;
using FoxUC.Logger.Contract;
using FoxUC.Logger.Integration.ENLog;
using FoxUC.Quartz.Extensions.Scheduler;
using FoxUC.Utility;
using FoxUC.Utility.ApiPermission;
using FoxUC.Utility.Config.AssemblyConfig;
using FoxUC.Utility.Data;
using FoxUC.Utility.Localization;
using FoxUC.Utility.TheOperation;

namespace FoxUC.Example.WebApp.AppStart
{
    /// <summary>
    /// 依赖注入
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="builder">内容生成</param>
        public static void RegisterComponents(ContainerBuilder builder)
        {
            var assemblyConfigLocalMember = new AssemblyConfigLocalMember();
            assemblyConfigLocalMember.ProtoAssemblyConfigReader = new AssemblyConfigJson();
            var assemblyConfig = assemblyConfigLocalMember.Reader();

            builder.UnifiedRegisterAssemblysForWeb(new BuilderParam()
            {
                AssemblyServices = assemblyConfig.Services,
                IsLoadAutoMapperConfig = assemblyConfig.IsLoadAutoMapperConfig,
                RegisteringServiceAction = () =>
                {
                    //builder.RegisterType<WorkflowConfigCache>().As<IWorkflowConfigReader>().AsSelf().PropertiesAutowired();
                    // builder.RegisterType<WorkflowInitSequenceService>().As<IWorkflowFormService>().AsSelf().PropertiesAutowired();

                    builder.RegisterType<AutofacInstance>().As<IInstance>().AsSelf().PropertiesAutowired().SingleInstance();
                    builder.RegisterType<IntegrationNLog>().As<ILogable>().AsSelf().PropertiesAutowired().SingleInstance();
                    builder.RegisterType<RoutePermissionCache>().As<IReader<RoutePermissionInfo[]>>().AsSelf().PropertiesAutowired().SingleInstance();
                    builder.RegisterType<CultureLibraryCache>().As<ICultureLibrary>().AsSelf().PropertiesAutowired().SingleInstance();
                }
            });
            builder.RegisterBuildCallback(container =>
            {
                var attachmentService = container.Resolve<AttachmentService>();
                AttachmentOwnerLocalMember attachmentOwnerLocalMember = container.Resolve<AttachmentOwnerLocalMember>();
                attachmentOwnerLocalMember.ProtoAttachmentOwnerReader = container.Resolve<AttachmentOwnerJson>();

                attachmentService.AttachmentOwnerReader = attachmentOwnerLocalMember;

                App.GetEventIdFunc = () =>
                {
                    var theOper = container.Resolve<ITheOperation>();
                    return theOper != null ? theOper.EventId : null;
                };

                var sch = container.Resolve<ISchedulerWrap>();
                sch.StartAsync();
            });
        }
    }
}