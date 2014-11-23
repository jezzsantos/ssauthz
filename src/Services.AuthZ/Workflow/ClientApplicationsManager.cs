using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Reflection;
using Common.Services;
using Common.Services.Workflow;
using Common.Storage.DataEntities;
using Services.AuthZ.Properties;
using Services.DataContracts;
using Services.MessageContracts;
using ServiceStack.Web;

namespace Services.AuthZ.Workflow
{
    /// <summary>
    ///     A manager of client applications.
    /// </summary>
    internal class ClientApplicationsManager : WorkflowManager<IClientApplication>, IClientApplicationsManager
    {
        GetClientApplicationResponse IClientApplicationsManager.GetClientApplication(IRequest request,
            GetClientApplication body)
        {
            return new GetClientApplicationResponse
            {
                ClientApplication = (ClientApplication) GetClientApplication(body.Id),
            };
        }

        CreateClientApplicationResponse IClientApplicationsManager.CreateClientApplication(IRequest request,
            CreateClientApplication body)
        {
            return new CreateClientApplicationResponse
            {
                ClientApplication = (ClientApplication) CreateClientApplication(body.Name),
            };
        }

        DeleteClientApplicationResponse IClientApplicationsManager.DeleteClientApplication(IRequest request,
            DeleteClientApplication body)
        {
            DeleteClientApplication(body.Id);
            return new DeleteClientApplicationResponse();
        }

        public IClientApplication GetClientApplication(string id)
        {
            Guard.NotNullOrEmpty(() => id, id);

            // Get the app
            IClientApplication app = Storage.Get(id);
            if (app == null)
            {
                throw LogicErrorThrower.ResourceNotFound();
            }

            return app;
        }

        public IClientApplication GetClientApplicationByClientIdentifier(string clientIdentifier)
        {
            Guard.NotNullOrEmpty(() => clientIdentifier, clientIdentifier);

            // Get the app
            IEnumerable<IClientApplication> apps = Storage.Find(Storage.BuildQuery(
                Reflector<ClientApplication>.GetPropertyName(x => x.ClientIdentifier),
                QueryOperator.EQ, clientIdentifier));
            if (!apps.Any())
            {
                throw LogicErrorThrower.ResourceNotFound();
            }

            return apps.FirstOrDefault();
        }

        public IClientApplication RegisterClientApplication(IClientApplication clientApplication)
        {
            Guard.NotNull(() => clientApplication, clientApplication);

            // Get the app
            IEnumerable<IClientApplication> apps = Storage.Find(Storage.BuildQuery(
                Reflector<ClientApplication>.GetPropertyName(x => x.ClientIdentifier), QueryOperator.EQ,
                clientApplication.ClientIdentifier));
            if (!apps.Any())
            {
                // Create the application
                string newAppId = Storage.Add(clientApplication);
                clientApplication.Id = newAppId;

                //TODO: Audit the creation of the client application

                return clientApplication;
            }

            return null;
        }

        internal IClientApplication CreateClientApplication(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);

            // Get the app
            IEnumerable<IClientApplication> apps = Storage.Find(Storage.BuildQuery(
                Reflector<ClientApplication>.GetPropertyName(x => x.Name), QueryOperator.EQ, name));
            if (apps.Any())
            {
                throw LogicErrorThrower.ResourceConflict(
                    Resources.ClientApplicationsManager_ApplicationByNameExists,
                    name);
            }

            // Construct a new identifier and a secret
            var app = new ClientApplication
            {
                Name = name,
                ClientIdentifier = Guid.NewGuid().ToString("D"),
                ClientSecret = Guid.NewGuid().ToString("D"),
            };

            return RegisterClientApplication(app);
        }

        internal void DeleteClientApplication(string id)
        {
            Guard.NotNullOrEmpty(() => id, id);

            // Get the app
            IClientApplication app = Storage.Get(id);
            if (app == null)
            {
                throw LogicErrorThrower.ResourceNotFound();
            }

            Storage.Delete(id);

            //TODO: Audit the deletion of the client application
        }
    }
}