using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{

    /// <summary>
    /// BudgetFiles class is used to manage the files used in the Budget project. Only has static members.
    /// </summary>
    public class BudgetFiles
    {
        private static String DefaultSavePath = @"Budget\";
        private static String DefaultAppData = @"%USERPROFILE%\AppData\Local\";

        // ====================================================================
        // verify that the name of the file exists, or set the default file, and 
        // is it readable?
        // throws System.IO.FileNotFoundException if file does not exist
        // ====================================================================
        /// <summary>
        /// Takes a FilePath and DefaultFileName and tries to read from the file.
        /// If the FilePath is null then it will use the default one in AppData + DefaultFileName. Else it will use the FilePath (full path).
        /// Returns that File if it exists, otherwise throws a FileNotFoundException.
        /// </summary>
        /// <param name="FilePath">Represents the full path to the file.</param>
        /// <param name="DefaultFileName">Represents the default name of the file (used if FilePath is null).</param>
        /// <returns>Returns a valid file path</returns>
        /// <exception cref="FileNotFoundException">The provided FilePath doesn't exist or the DefaultFileName is not in the default AppData path.</exception>
        /// <example>
        /// <code>
        /// // providing the DefaultFileName and using the AppData path
        /// string filePath = BudgetFiles.VerifyReadFromFileName(null, "test.budget");
        /// // providing the FullPath
        /// string filePath = BudgetFiles.VerifyReadFromFileName("C:\Users\studentID\Downloads\BudgetSolution\BudgetSolution\test.budget", null);
        /// </code>
        /// </example>
        public static String VerifyReadFromFileName(String FilePath, String DefaultFileName)
        {

            // ---------------------------------------------------------------
            // if file path is not defined, use the default one in AppData
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does FilePath exist?
            // ---------------------------------------------------------------
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("ReadFromFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ----------------------------------------------------------------
            // valid path
            // ----------------------------------------------------------------
            return FilePath;
        }

        // ====================================================================
        // verify that the name of the file exists, or set the default file, and 
        // is it writable
        // ====================================================================
        /// <summary>
        /// Takes a FilePath and DefaultFileName and verifies that we can write to the file.
        /// If the FilePath is null, will use the directory at the path of the AppData. If it doesn't exist then it will create a directory.
        /// If the Budget directory doesn't exist in the app directory then it will create it. Finally it uses that path + DefaultFileName.
        /// If the FilePath is not null then it will use that as full path. It then verifies that the directory exists, the file exists and that we can write to it.
        /// Returns that filePath if it exists and we can write to it, otherwise throws an Exception.
        /// </summary>
        /// <param name="FilePath">Represents the full path to the file.</param>
        /// <param name="DefaultFileName">Represents the default name of the file (used if FilePath is null).</param>
        /// <returns>Returns a valid file path to which we can write to.</returns>
        /// <exception cref="Exception">Thrown if there isn't a budget directory in the provided FilePath, the FilePath doesn't exist, the budget file doesn't exist, or the budget file is read-only.</exception>
        /// <example>
        /// <code>
        /// // providing the DefaultFileName and using the AppData path
        /// string filePath = BudgetFiles.VerifyReadFromFileName(null, "test.budget");
        /// // providing the FullPath
        /// string filePath = BudgetFiles.VerifyReadFromFileName(@"C:\Users\studentID\Downloads\BudgetSolution\BudgetSolution\test.budget", null);
        /// </code>
        /// </example>
        public static String VerifyWriteToFileName(String FilePath, String DefaultFileName)
        {
            // ---------------------------------------------------------------
            // if the directory for the path was not specified, then use standard application data
            // directory
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                // create the default appdata directory if it does not already exist
                String tmp = Environment.ExpandEnvironmentVariables(DefaultAppData);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                // create the default Budget directory in the appdirectory if it does not already exist
                tmp = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does directory where you want to save the file exist?
            // ... this is possible if the user is specifying the file path
            // ---------------------------------------------------------------
            String folder = Path.GetDirectoryName(FilePath);
            String delme = Path.GetFullPath(FilePath);
            if (!Directory.Exists(folder))
            {
                throw new Exception("SaveToFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ---------------------------------------------------------------
            // can we write to it?
            // ---------------------------------------------------------------
            if (File.Exists(FilePath))
            {
                FileAttributes fileAttr = File.GetAttributes(FilePath);
                if ((fileAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new Exception("SaveToFileException:  FilePath(" + FilePath + ") is read only");
                }
            }

            // ---------------------------------------------------------------
            // valid file path
            // ---------------------------------------------------------------
            return FilePath;
        }
    }
}
