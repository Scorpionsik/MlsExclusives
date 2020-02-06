using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace MlsExclusive.Utilites
{
    /// <summary>
    /// Класс для записи/считывания переменных в реестре Windows
    /// </summary>
    public static class RegeditEditor
    {
        /// <summary>
        /// Записывает параметр по указанному пути
        /// </summary>
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
