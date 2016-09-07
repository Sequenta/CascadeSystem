using System;
using Domain;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Core.Tests.Infrastructure
{
    public class WorkplaceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new FilteringSpecimenBuilder(new WorkplaceBuilder(),
                 new WorkplaceSpecification()));
        }

        private class WorkplaceBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var user = new User(context.Create("email"), context.Create("password"), context.Create("lastName"), context.Create("firstName"), context.Create("patronymic"));
                var workplace = new Workplace(user);
                return workplace;
            }
        }

        private class WorkplaceSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request)
            {
                var requestType = request as Type;
                if (requestType == null)
                    return false;
                return requestType == typeof(Workplace);
            }
        }
    }
}