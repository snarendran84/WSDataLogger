using DataLoggerWS.models;
using DataLoggerWS.util;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace DataLoggerWS.core
{
    public class DirectoryMonitor : FileSystemWatcher
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private int interval = 30000; //defaulted to 30s
        string processedFolder = ConfigurationManager.AppSettings["ArchiveFolderPath"];
        string ErrorLogFile = ConfigurationManager.AppSettings["CriticalErrorPath"];
        private IFileWrapper fileWrapper = new FileWrapper();
        public DirectoryMonitor()
        {
            Init();
        }
        public DirectoryMonitor(string inputDirectory) 
            : base(inputDirectory) 
        {
            Init();
        }
        public DirectoryMonitor(string inputDirectory, string inFilter) 
            : base(inputDirectory, inFilter)
        {
            Init();
        }
        private void Init()
        {
           NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

           Changed += OnChanged;
           Created += OnCreated;
           Deleted += OnDeleted;
           Renamed += OnRenamed;
           Error += OnError;

           Filter = "*.txt";
           IncludeSubdirectories = true;
           EnableRaisingEvents = true;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            logger.Error(e.GetException(), "Unexpected error received while monitoring the folder");
        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            logger.Error("File name changes are not supported yet");
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            logger.Error("File deletes are not supported yet");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                var fullPath = e.FullPath;
                processFile(fullPath);
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Error while processing the newly created file {e.FullPath}");
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            logger.Error("File changes are not supported yet");
        }

        private void processFile(string filePath)
        {
            try
            {
                DataAccess da = new DataAccess();

                var fileId = da.CreateFile(filePath);

                if (!(fileId == 0))
                {

                    var dataToLog = new List<FileDataLog>();

                    string[] fileLines = fileWrapper.ReadAllLines(filePath);

                    foreach (var line in fileLines)
                    {
                        dataToLog.Add(new FileDataLog { FileId = fileId, LogData = line.Length > 500 ? line.Substring(0,500) : line });
                    }

                    da.InsertFileDataLog(dataToLog.ToArray());
                }
                else
                {
                    logger.Error($"File Already Exists {filePath}", "Error");
                }
                
                var archiveFileName = System.IO.Path.Combine(processedFolder, System.IO.Path.GetFileName(filePath));
                
                if (fileWrapper.Exists(filePath)) 
                {
                    if (!fileWrapper.Exists(archiveFileName))
                        fileWrapper.Move(filePath, archiveFileName);
                    else
                        fileWrapper.Delete(filePath);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Error while processing the file {filePath}");
            }
        }
    }
}

namespace DataLoggerWS.util
{
    using System.IO;

    public interface IFileWrapper
    {
        bool Exists(string path);
        void Move(string sourcefilename, string destinationfilename);
        void Delete(string path);
        string[] ReadAllLines(string path);
    }

    public class FileWrapper : IFileWrapper
    {
        public void Delete(string path)
        {
           File.Delete(path);
        }

        public bool Exists(string path)
        {
           return File.Exists(path);
        }

        public void Move(string sourcefilename, string destinationfilename)
        {
            File.Move(sourcefilename, destinationfilename);   
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}