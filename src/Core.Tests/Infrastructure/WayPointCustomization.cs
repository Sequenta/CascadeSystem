using System;
using Domain;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Core.Tests.Infrastructure
{
    public class WayPointCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new FilteringSpecimenBuilder(new WayPointBuilder(),
                new WayPointSpecification()));
        }

        private class WayPointBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var userFrom = new User(context.Create("email"), context.Create("password"), context.Create("lastName"), context.Create("firstName"), context.Create("patronymic"));
                var userTo = new User(context.Create("email"), context.Create("password"), context.Create("lastName"), context.Create("firstName"), context.Create("patronymic"));
                var workplaceFrom = new Workplace(userFrom);
                var workplaceTo = new Workplace(userTo);
                return new WayPoint(workplaceFrom, workplaceTo, null)
                {
                    Parent = null
                };
            }
        }

        private class WayPointSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request)
            {
                var requestType = request as Type;
                if (requestType == null)
                    return false;
                return requestType == typeof(WayPoint);
            }
        }
    }
}