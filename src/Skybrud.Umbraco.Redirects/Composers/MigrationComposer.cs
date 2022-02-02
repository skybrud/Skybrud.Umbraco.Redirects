using Skybrud.Umbraco.Redirects.Components;
using Umbraco.Core;
using Umbraco.Core.Composing;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Composers {

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class MigrationComposer : IUserComposer {

        public void Compose(Composition composition) {
            composition.Components().Append<MigrationComponent>();
        }

    }

}