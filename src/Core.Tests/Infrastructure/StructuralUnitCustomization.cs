using System;
using Domain;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Core.Tests.Infrastructure
{
    internal class StructuralUnitCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new FilteringSpecimenBuilder(new Builder(fixture),
                new ObjectSpecification()));
        }

        private class Builder : ISpecimenBuilder
        {
            private readonly IFixture _fixture;

            public Builder(IFixture fixture)
            {
                _fixture = fixture;
            }

            public object Create(object request, ISpecimenContext context)
            {
                return new StructuralUnit(context.Create("name"),_fixture.Create<Workplace>());
            }
        }

        private class ObjectSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request)
            {
                var requestType = request as Type;
                if (requestType == null)
                    return false;
                return requestType == typeof (StructuralUnit);
            }
        }
    }
}