using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Supervertaler.Trados.Core
{
    /// <summary>
    /// Loads the embedded Supervertaler title-bar icon (sv-icon.ico) once and
    /// hands the cached <see cref="Icon"/> out to every dialog. Used by all
    /// plugin Form constructors so windows show the blue Sv logo instead of
    /// the WinForms generic icon.
    /// </summary>
    internal static class IconHelper
    {
        private const string ResourceName = "Supervertaler.Trados.Resources.sv-icon.ico";

        private static Icon _cached;
        private static bool _attempted;

        /// <summary>
        /// Returns the cached app icon, or null if the resource is missing /
        /// fails to decode. Callers can do <c>this.Icon = IconHelper.AppIcon;</c>
        /// without a null check — WinForms tolerates a null Icon assignment
        /// and falls back to the generic icon.
        /// </summary>
        public static Icon AppIcon
        {
            get
            {
                if (_attempted) return _cached;
                _attempted = true;
                try
                {
                    var asm = Assembly.GetExecutingAssembly();
                    using (var stream = asm.GetManifestResourceStream(ResourceName))
                    {
                        if (stream != null)
                            _cached = new Icon(stream);
                    }
                }
                catch
                {
                    _cached = null;
                }
                return _cached;
            }
        }
    }
}
