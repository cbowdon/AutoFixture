using System;
using Ploeh.AutoFixture.Kernel;

namespace Ploeh.AutoFixture
{
    public class FreezeOnMatchCustomization : ICustomization
    {
        private readonly Type targetType;
        private readonly IRequestSpecification matcher;
        private IFixture fixture;

        public FreezeOnMatchCustomization(Type targetType)
            : this(targetType, new ExactTypeSpecification(targetType))
        {
        }

        public FreezeOnMatchCustomization(
            Type targetType,
            IRequestSpecification matcher)
        {
            this.targetType = targetType;
            this.matcher = matcher;
        }

        public Type TargetType
        {
            get { return this.targetType; }
        }

        public IRequestSpecification Matcher
        {
            get { return this.matcher; }
        }

        public void Customize(IFixture fixture)
        {
            if (fixture == null)
            {
                throw new ArgumentNullException("fixture");
            }

            this.fixture = fixture;

            FreezeTypeForMatchingRequests();
        }

        private void FreezeTypeForMatchingRequests()
        {
            this.fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    FreezeTargetType(),
                    this.matcher));
        }

        private ISpecimenBuilder FreezeTargetType()
        {
            var context = new SpecimenContext(this.fixture);
            var specimen = context.Resolve(this.targetType);
            return new FixedBuilder(specimen);
        }
    }
}
