[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Boxing.Tests")]

namespace Boxing
{
    public partial class Box {}
    public partial struct Box<T> {}

    namespace Linq
    {
        public partial class BoxQueryExtensions {}
    }
}
