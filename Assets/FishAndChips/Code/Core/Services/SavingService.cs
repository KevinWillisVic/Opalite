using System;
using System.IO;
using UnityEngine;

namespace FishAndChips
{
    /// <summary>
    /// Service handling reading and writing from files.
    /// </summary>
    public class SavingService : Singleton<SavingService>
    {
        #region -- Private Methods --
        /// <summary>
        /// Save json to location.
        /// </summary>
        /// <param name="fullPath">Location to save json.</param>
        /// <param name="json">Json to save.</param>
        private void Save(string fullPath, string json)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(json);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        /// <summary>
        /// Read and return json from file at path.
        /// </summary>
        /// <param name="fullPath">Path json is stored at.</param>
        /// <returns>Json read from file.</returns>
        private string Load(string fullPath)
        {
            if (File.Exists(fullPath) == false)
            {
                return string.Empty;
            }

            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Delete file at path location if it exists.
        /// </summary>
        /// <param name="fullPath">Folder location of file.</param>
        private void Delete(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
		#endregion

		#region -- Public Methods --
        /// <summary>
        /// Save json to file at supplied path.
        /// </summary>
        /// <param name="filePath">Folder location.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="json">Json representation.</param>
		public void SaveJson(string filePath, string fileName, string json)
        {
            string path = filePath;
            string fullPath = Path.Combine(path, fileName);
            Save(fullPath, json);
        }

        /// <summary>
        /// Save json to file at persistent data path.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="json">Json representation.</param>
        public void SaveJson(string fileName, string json)
        {
            string path = Application.persistentDataPath;
            string fullPath = Path.Combine(path, fileName);
            Save(fullPath, json);
        }

        /// <summary>
        /// Read json at supplied path.
        /// </summary>
        /// <param name="filePath">Folder location.</param>
        /// <param name="fileName">File name</param>
        /// <returns>Json representation.</returns>
        public string LoadJson(string filePath, string fileName)
        {
			string path = filePath;
			string fullPath = Path.Combine(path, fileName);
            return Load(fullPath);
        }

        /// <summary>
        /// Read json at persistent data path.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Json representation.</returns>
        public string LoadJson(string fileName)
        {
            string path = Application.persistentDataPath;
			string fullPath = Path.Combine(path, fileName);
            return Load(fullPath);
        }

        /// <summary>
        /// Delete file at path location.
        /// </summary>
        /// <param name="filePath">Path of file.</param>
        /// <param name="fileName">Name of file.</param>
        public void DeleteFile(string filePath, string fileName)
        {
			string path = filePath;
			string fullPath = Path.Combine(path, fileName);
            Delete(fullPath);
		}

        /// <summary>
        /// Delete file at persistent data path.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        public void DeleteFile(string fileName)
        {
            string path = Application.persistentDataPath;
			string fullPath = Path.Combine(path, fileName);
            Delete(fullPath);
		}
        #endregion
    }
}
