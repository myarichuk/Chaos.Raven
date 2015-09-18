using Nancy.Hosting.Self;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Chaos.Raven.Service
{
    class Program
    {
        static IDocumentStore documentStore;
        static NancyHost serviceControllerHost;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "It will be disposed via TopShelf's WhenShutdown() method")]
        static void Main()
        {
            var databaseName = ConfigurationManager.AppSettings["DatabaseName"];
            var databaseHostname = ConfigurationManager.AppSettings["DatabaseHostname"];
            var serviceControllerPort = int.Parse(ConfigurationManager.AppSettings["ControllerServicePort"]);

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            documentStore = new DocumentStore
            {
                Url = databaseHostname,
                DefaultDatabase = databaseName
            };
            documentStore.Initialize();

            serviceControllerHost = new NancyHost(new Uri($"http://localhost:{serviceControllerPort}"));
            HostFactory.Run(x =>
            {
                x.Service<ChaosService>(s =>
                {
                    s.ConstructUsing(name => new ChaosService(documentStore, databaseName));
                    s.WhenStarted(cs =>
                    {
                        cs.Start();
                        serviceControllerHost.Start();
                    });
                    s.WhenStopped(cs =>
                    {
                        cs.Stop();
                        serviceControllerHost.Stop();
                    });
                    s.WhenShutdown(cs => cs.Dispose());
                });
                x.RunAsLocalSystem();
                x.StartManually();

                x.SetDescription("Chaos.Raven Server Process");
                x.SetDisplayName("Chaos.Raven Server");
                x.SetServiceName("Chaos.Raven");
            });
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (documentStore != null && !documentStore.WasDisposed)
                documentStore.Dispose();

            serviceControllerHost.Dispose();
        }
    }
}
