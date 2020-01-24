using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MlsExclusive.Utilites
{
    public static class RegeditEditor
    {
        public static void Write(RegistryKey key, string path, string value_name, object value, bool createPath = false)
        {
            RegistryKey work = key;
            foreach(string path_folder in new Regex(@"[\\/]").Split(path))
            {
                if (work.OpenSubKey(path_folder, true) == null && createPath) work.CreateSubKey(path_folder);
                work = work.OpenSubKey(path_folder, true);
            }
            work.SetValue(value_name, value);
        }

        public static object Read(RegistryKey key, string path, string value_name)
        {
            RegistryKey work = key;
            foreach (string path_folder in new Regex(@"[\\/]").Split(path))
            {
                work = work.OpenSubKey(path_folder, false);
            }
            return work.GetValue(value_name);
        }
    }
}
