using Skybrud.Umbraco.Redirects.Components;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Redirects.Composers
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class MigrationComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<MigrationComponent>();
        }
    }
}
