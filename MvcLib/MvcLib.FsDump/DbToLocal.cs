using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using MvcLib.Common;
using MvcLib.DbFileSystem;

namespace MvcLib.FsDump
{
    public class DbToLocal
    {
        static void RecursiveDelete(DirectoryInfo fsInfo, bool self = false, params string[] extensionsToIgnore)
        {
            foreach (var info in fsInfo.EnumerateFileSystemInfos())
            {
                var directoryInfo = info as DirectoryInfo;
                if (directoryInfo != null)
                {
                    RecursiveDelete(directoryInfo, true, extensionsToIgnore);
                }
                else
                {
                    if (extensionsToIgnore.Any(s => info.Extension.Equals(s)))
                        continue;

                    try
                    {
                        info.Delete();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
            }

            if (!self || fsInfo.GetFiles().Any()) return;
            try
            {
                fsInfo.Delete(true);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private static readonly DirectoryInfo DirInfo;

        static DbToLocal()
        {
            var path = Config.ValueOrDefault("DumpToLocalFolder", "~/dbfiles");

            var root = Path.GetFullPath(HostingEnvironment.MapPath(path));
            DirInfo = new DirectoryInfo(root);
            if (!DirInfo.Exists)
                DirInfo.Create();
            else
                RecursiveDelete(DirInfo, false, ".Config");
        }

        public static void Execute()
        {
            Trace.TraceInformation("[DbToLocal]: Starting...");

            using (var ctx = new DbFileContext())
            {
                var dbFiles = ctx.DbFiles
                    .Where(x => !x.IsHidden && !x.IsDirectory && x.Extension != ".dll")
                    .ToList();

                foreach (var dbFile in dbFiles)
                {
                    WriteToDisk(dbFile, false);
                }
            }
        }

        static string GetLocalPath(DbFile dbFile)
        {
            string localpath = DirInfo.FullName + dbFile.VirtualPath.Replace("/", "\\");
            return localpath;
        }

        public static void WriteToDisk(DbFile dbFile, bool force)
        {
            Trace.TraceInformation("[DbToLocal]:Copiando arquivo: '{0}'", dbFile.VirtualPath);

            var localpath = GetLocalPath(dbFile);

            if (File.Exists(localpath))
            {
                var fi = new FileInfo(localpath);

                if (fi.LastWriteTimeUtc > dbFile.LastWriteUtc && !force)
                    return;

                Trace.TraceWarning("[DbToLocal]:Arquivo será excluído: {0}/{1}", fi.FullName, fi.LastAccessTimeUtc);
                try
                {
                    File.Delete(localpath);
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation(ex.Message);
                }
            }

            var dir = Path.GetDirectoryName(localpath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                if (dbFile.IsBinary && dbFile.Bytes.Length > 0)
                    File.WriteAllBytes(localpath, dbFile.Bytes);
                else File.WriteAllText(localpath, dbFile.Texto);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.Message);
            }

        }

        public static void RemoveFromDisk(DbFile dbFile)
        {
            var localpath = GetLocalPath(dbFile);

            if (File.Exists(localpath))
            {
                Trace.TraceWarning("[DbToLocal]:Arquivo será excluído: {0}", localpath);
                try
                {
                    File.Delete(localpath);
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation(ex.Message);
                }
            }
            else if (Directory.Exists(localpath))
            {
                RecursiveDelete(new DirectoryInfo(localpath), true, ".Config");
            }
        }
    }
}