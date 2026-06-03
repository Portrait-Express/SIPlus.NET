using SIPlus.NET;
using System.Runtime.InteropServices;

namespace SIPlus.Test {
    public class GlobalSetup {
        private static string GetLibraryName() {
            if (OperatingSystem.IsWindows()) {
                return "siplus.dll";
            } else if (OperatingSystem.IsLinux()) {
                return "libsiplus.so";
            } else if (OperatingSystem.IsMacOS()) {
                return "libsiplus.dylib";
            } else {
                throw new InvalidOperationException("Unsupported platform.");
            }
        }

        public GlobalSetup() {
            //Does not look in runtimes if not in a nuget package, so this is necessary for testing
            NativeLibrary.SetDllImportResolver(typeof(Parser).Assembly, (name, assembly, searchPath) => {
                if (name == "siplus") {
                    var path = Path.Combine(
                        Environment.CurrentDirectory,
                        "runtimes",
                        RuntimeInformation.RuntimeIdentifier,
                        "native",
                        GetLibraryName()
                    );

                    if (NativeLibrary.TryLoad(path, out var handle))
                        return handle;
                }

                return IntPtr.Zero;
            });
        }
    }
}
