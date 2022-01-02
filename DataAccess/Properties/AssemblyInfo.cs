using System.Runtime.CompilerServices;

#if DEBUG

[assembly: InternalsVisibleTo("Test.DataAccess")]
[assembly: InternalsVisibleTo("Test.BusinessLogic")]

#endif