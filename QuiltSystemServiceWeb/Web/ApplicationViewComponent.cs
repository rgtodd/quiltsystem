//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Web
{
    public abstract class ApplicationViewComponent<TModelFactory> : ApplicationViewComponentBase where TModelFactory : ApplicationModelFactory, new()
    {
        private TModelFactory m_modelFactory;

        public ApplicationViewComponent(
            IApplicationLocale applicationLocale)
            : base(
                  applicationLocale)
        { }

        public TModelFactory ModelFactory
        {
            get
            {
                if (m_modelFactory == null)
                {
                    var modelFactory = ApplicationModelFactory.Create<TModelFactory>(this);

                    m_modelFactory = modelFactory;
                }

                return m_modelFactory;
            }
        }
    }
}