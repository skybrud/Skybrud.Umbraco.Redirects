using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Redirects.EventHandlers
{
    public class MigrationComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<MigrationComponent>();
        }
    }
}
