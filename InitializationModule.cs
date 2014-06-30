using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Nergard.EPi.CreateTypeOnlyOnce
{
    [InitializableModule]
    public class InitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.CreatedContent += Instance_CreatedContent;
            DataFactory.Instance.DeletedContent += Instance_DeletedContent;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.CreatedContent -= Instance_CreatedContent; ;
            DataFactory.Instance.DeletedContent -= Instance_DeletedContent;
        }

        void Instance_CreatedContent(object sender, ContentEventArgs e)
        {
            var createonce = e.Content as ICreateOnlyOnce;
            if (createonce == null)
                return;

            var repository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            if (repository != null)
            {
                var contentType = repository.Load(e.Content.ContentTypeID);

                if (contentType != null)
                {
                    var writableContentType = (ContentType)contentType.CreateWritableClone();
                    writableContentType.IsAvailable = false;
                    repository.Save(writableContentType);
                }
            }
        }

        void Instance_DeletedContent(object sender, DeleteContentEventArgs e)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var contentTyperepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            foreach (ContentReference cr in e.DeletedDescendents)
            {
                var content = contentRepository.Get<IContent>(cr);

                var createonce = content as ICreateOnlyOnce;
                if (createonce != null)
                {
                    if (contentTyperepository != null)
                    {
                        var contentType = contentTyperepository.Load(content.ContentTypeID);

                        if (contentType != null)
                        {
                            var writableContentType = (ContentType)contentType.CreateWritableClone();
                            writableContentType.IsAvailable = true;
                            contentTyperepository.Save(writableContentType);
                        }
                    }
                }
            }
        }
    } 
} 
