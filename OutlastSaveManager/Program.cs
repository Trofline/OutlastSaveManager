using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;

namespace OutlastSaveManager
{
    internal static class Program
    {
        public static bool firstRunBool = true;
        public static string localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveManager");
        //public static string startupPath = Directory.GetParent(Application.StartupPath).FullName;
        public static string startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        private static string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);



        public static string argumentStartedWith;


        [STAThread]
        static void Main(string[] args)
        {
            // habe das in Funktionalem Stil geschrieben, die Originalfassung hat mir mein funktionalliebendes Programmierherz gebrochen... :)
            if (!prop.Default.externalModPackage)
            {
                if (Process.GetProcesses()
                          .Any(p => p.ProcessName.StartsWith("BetterOutlastLauncher", StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show(
                        "If you run the SaveManager with InternalPackages,\n" +
                        "please dont use the BetterOutlastLauncher\n" +
                        "to avoid the tools conflicting with each other!",
                        "Warning"
                    );
                }
            }

            if (prop.Default.shortcuts)
            {
                CreateShortcutInParent("Vanilla", "vanilla");
                CreateShortcutInParent("Macca%", "macca");
                CreateShortcutInParent("SuperJump", "superjump");
                CreateShortcutInParent("AllDoorsUnlocked", "alldoorsunlocked");
                CreateShortcutInParent("SaveManager");

                //if (!File.Exists(Path.Combine(startupPath, "RunWithOtherProgram.bat")))
                //{
                //    File.Move(Path.Combine(startupPath, "SaveManager", "RunWithOtherProgram.bat"), Path.Combine(startupPath, "RunWithOtherProgram.bat"), true);
                //}
            }
            else
            {
                CreateShortcutInCurrent("Vanilla", "vanilla");
                CreateShortcutInCurrent("Macca%", "macca");
                CreateShortcutInCurrent("SuperJump", "superjump");
                CreateShortcutInCurrent("AllDoorsUnlocked", "alldoorsunlocked");
                CreateShortcutInCurrent("SaveManager");
                try
                {
                    File.Delete(Path.Combine(startupPath, "AllDoorsUnlocked.ink"));
                    File.Delete(Path.Combine(startupPath, "SuperJump.ink"));
                    File.Delete(Path.Combine(startupPath, "Macca%.ink"));
                    File.Delete(Path.Combine(startupPath, "Vanilla.ink"));
                    File.Delete(Path.Combine(startupPath, "SaveManager.ink"));
                    File.Delete(Path.Combine(startupPath, "RunWithOtherProgram.bat"));
                }
                catch (Exception)
                {

                    throw;
                }

                //if (!File.Exists(Path.Combine(startupPath, "RunWithOtherProgram.bat")))
                //{
                //    File.Move(Path.Combine(startupPath, "SaveManager", "RunWithOtherProgram.bat"), Path.Combine(startupPath, "RunWithOtherProgram.bat"), true);
                //}
            }
            try
            {

                if (args == null || args.Length == 0)
                {

                    goto here;
                }

                if (args.Length > 0)
                {
                    argumentStartedWith = args[0];

                }
           
                    ApplicationConfiguration.Initialize();
                    var curDir = Application.StartupPath;
                    string configPathOLEngine = Path.Combine(documentsFolder, "My Games","Outlast","OLGame", "Config", "OLEngine.ini");
                    string[] lines = File.ReadAllLines(configPathOLEngine);

               
                var procOL = Process.GetProcessesByName("OLGame");

                if (procOL.Length != null)
                {
                    foreach (var item in procOL)
                    {
                        item.Kill();
                    }
                }



              



                if (args == null || args.Length == 0)
                {

                    goto here;
                }




                if (prop.Default.smoothmouse)
                {
                    smoothmouse(true);
                }
                else
                {
                    smoothmouse(false);
                }


                if (prop.Default.fps)
                {
                    int counter = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith("MaxSmoothedFrameRate="))
                        {
                            // hier wählst du dynamisch den Wert
                            lines[i] = "MaxSmoothedFrameRate=61.81";
                            counter = i;
                        }
                    }
                    if (counter > 0)
                    {
                        File.AppendAllText(configPathOLEngine, lines[counter]);
                    }
                   // File.WriteAllLines(configPathOLEngine, lines);
                    
                }
                else
                {
                    int counter = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith("MaxSmoothedFrameRate="))
                        {
                            // hier wählst du dynamisch den Wert
                            lines[i] = "MaxSmoothedFrameRate=62";
                            counter = i;
                        }
                    }
                    if (counter > 0)
                    {
                        File.AppendAllText(configPathOLEngine, lines[counter]);
                    }
                    //File.WriteAllLines(configPathOLEngine, lines);
                }

                if (prop.Default.borderless)
                {

                    File.Copy(Path.Combine(startupPath, "SaveManager", "SaveManager-Mods", "BorderlessWindow", "32", "dinput8.dll"), Path.Combine(startupPath, "Binaries", "Win32", "dinput8.dll"), true);

                    File.Copy(Path.Combine(startupPath, "SaveManager", "SaveManager-Mods", "BorderlessWindow", "64", "dinput8.dll"), Path.Combine(startupPath, "Binaries", "Win64", "dinput8.dll"), true);

                }
                else
                {
                    try
                    {
                        File.Delete(Path.Combine(startupPath, "Binaries", "Win32", "dinput8.dll"));

                    }
                    catch
                    {


                    }
                    try
                    {
                        File.Delete(Path.Combine(startupPath, "Binaries", "Win64", "dinput8.dll"));

                    }
                    catch
                    {
                    }
                }

                if (!Directory.Exists(Path.Combine(localAppDataPath, "Bit")))
                {
                    Directory.CreateDirectory(Path.Combine(localAppDataPath, "Bit"));
                    File.Create(Path.Combine(localAppDataPath, "Bit", "64")).Dispose();
                }

                if (prop.Default.bit)
                {
                    try { File.Create(Path.Combine(localAppDataPath, "Bit", "32")).Dispose(); } catch (Exception) { throw; }

                    try
                    {
                        File.Delete(Path.Combine(localAppDataPath, "Bit", "64"));

                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
                else
                {
                    try { File.Create(Path.Combine(localAppDataPath, "Bit", "64")).Dispose(); } catch (Exception) { throw; }

                    try
                    {
                        File.Delete(Path.Combine(localAppDataPath, "Bit", "32"));

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }


                if (args == null || args.Length == 0)
                {

                    goto here;
                }


                else if (argumentStartedWith == "vanilla")
                {
                    vanilla();
                }
                else if (argumentStartedWith == "superjump")
                {
                    superjump();
                }
                else if (argumentStartedWith == "macca")
                {
                    macca();
                }
                else if (argumentStartedWith == "alldoorsunlocked")
                {
                    alldoorsunlocked();
                }

                bShowCheckpointName();
                iniCheck();
                internalPackages(!prop.Default.externalModPackage);
                LaunchOLGame();

            here:

                if (Path.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveManager/FirstRun")))
                {
                    firstRunBool = false;
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveManager/FirstRun"));
                    firstRunBool = true;
                }

                if (firstRunBool == true)
                {
                    copyBackToStart();
                    Application.Run(new firstRun());
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var splash = new boot())
                    {
                        splash.Show();
                        Application.DoEvents();
                        checkFilesMovedAlready();   // Start → LocalAppData
                        System.Threading.Thread.Sleep(1000);
                    }
                    Application.Run(new Manager());
                }




            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e);
                throw;
            }
        }

        private static void internalPackages(bool value)
        {
            string configPath = Path.Combine(startupPath, "OLGame", "Config", "DefaultGame.ini");

            if (!File.Exists(configPath))
            {
                MessageBox.Show("Config-File not found: " + configPath);
                return;
            }

            string[] lines = File.ReadAllLines(configPath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("DefaultGame="))
                {
                    lines[i] = value ? "DefaultGame=SaveManager.SMGame" : "DefaultGame=OLGame.OLGame";
                }
                else if (lines[i].StartsWith("DefaultServerGame="))
                {
                    lines[i] = value ? "DefaultServerGame=SaveManager.SMGame" : "DefaultServerGame=OLGame.OLGame";
                }
                else if (lines[i].StartsWith("PlayerControllerClassName="))
                {
                    lines[i] = "PlayerControllerClassName=OLGame.OLPlayerController"; // bleibt gleich
                }
                else if (lines[i].StartsWith("DefaultGameType="))
                {
                    lines[i] = value ? "DefaultGameType=\"SaveManager.SMGame\"" : "DefaultGameType=\"OLGame.OLGame\"";
                }
            }

            File.WriteAllLines(configPath, lines);

            try
            {
                File.Copy(Path.Combine(startupPath, "SaveManager", "SaveManager-Mods", "InternalModPackages", "SaveManager.u"), Path.Combine(startupPath, "OLGame", "CookedPCConsole", "SaveManager.u"), true);

            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e);
                throw;
            }
        }


        private static void iniCheck()
        {
            string configPath = Path.Combine(startupPath, "OLGame", "Config", "DefaultInput.ini");
            string searchLineStart = "exec commands.cfg";
            string newLine = ".Bindings=(Name=\"RightShift\",Command=\"exec commands.cfg\")";

            string[] lines = File.ReadAllLines(configPath);
            bool found = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(searchLineStart))
                {
                    lines[i] = newLine;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // ans Ende hinzufügen
                var newList = lines.ToList();
                newList.Add(newLine);
                lines = newList.ToArray();
            }

            File.WriteAllLines(configPath, lines);

            string cfgPath = Path.Combine(startupPath, "Binaries", "commands.cfg");
            if (!File.Exists(cfgPath))
            {
                File.Create(cfgPath).Dispose(); // Dispose nicht vergessen, sonst bleibt Datei gelockt
            }
        }


        private static void removeShortcuts()
        {
            string exeFolder = Directory.GetCurrentDirectory();
            string exePath = Path.Combine(exeFolder, "OutlastSaveManager.exe");

            // Eine Ebene höher
            string parentFolder = Directory.GetParent(exeFolder).FullName;

            string[] filesToDelete = new string[]
{
    "Vanilla.Ink.Inc",
    "SuperJump.Ink.Inc",
    "Macca%.Ink.Inc",
    "AllDoorsUnlocked.Ink.Inc",
    "SaveManager.Ink.Inc"
};

            foreach (string fileName in filesToDelete)
            {
                string fullPath = Path.Combine(parentFolder, fileName);
                try
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        
                    }
                }
                catch
                {
                    
                }
            }
        }

        private static void smoothmouse(bool value)
        {
            string configPath = Path.Combine(startupPath, "OLGame", "Config", "DefaultInput.ini"); // Pfad zur INI-Datei
            string searchLineStart = "bEnableMouseSmoothing="; // Anfang der Zeile, die ersetzt werden soll
            string newLine = "bEnableMouseSmoothing=" + (value ? "true" : "false");


            // Datei einlesen
            string[] lines = File.ReadAllLines(configPath);

            // Zeile ersetzen
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(searchLineStart))
                {
                    lines[i] = newLine;
                    break; // nur die erste gefundene Zeile ersetzen
                }
            }

            // Datei wieder speichern
            File.WriteAllLines(configPath, lines);
        }

        private static void bShowCheckpointName()
        {
            string configPath = Path.Combine(startupPath,"OLGame","Config","DefaultUI.ini"); // Pfad zur INI-Datei
            string searchLineStart = "bShowCheckpointNames=false"; // Anfang der Zeile, die ersetzt werden soll
            string newLine = "bShowCheckpointNames=true"; // Neue Zeile

            // Datei einlesen
            string[] lines = File.ReadAllLines(configPath);

            // Zeile ersetzen
            for (int i = 0; i < lines.Length;i++)
            {
                if (lines[i].StartsWith(searchLineStart))
                {
                    lines[i] = newLine;
                    break; // nur die erste gefundene Zeile ersetzen
                }
            }

            // Datei wieder speichern
            File.WriteAllLines(configPath, lines);
        }

        public static void CreateShortcutInParent(string shortcutName, string arguments = "")
        {
            // Pfad zur EXE
            string exeFolder = Directory.GetCurrentDirectory();
            string exePath = Path.Combine(exeFolder,"OutlastSaveManager.exe");

            // Eine Ebene höher
            string parentFolder = Directory.GetParent(exeFolder).FullName;

            // Pfad zur .lnk-Datei
            string shortcutPath = Path.Combine(parentFolder, shortcutName + ".lnk");

            // COM-Objekt WScript.Shell
            Type t = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(t);

            // Verknüpfung erstellen
            var shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = exePath;
            shortcut.WorkingDirectory = exeFolder;
            shortcut.Arguments = arguments; // Parameter
            shortcut.Description = $"Boots {shortcutName} with arguments {arguments}";
            shortcut.IconLocation = exePath + ",0";
            shortcut.Save();
        }
        public static void CreateShortcutInCurrent(string shortcutName, string arguments = "")
        {
            // Pfad zur EXE
            string exeFolder = Directory.GetCurrentDirectory();
            string exePath = Path.Combine(exeFolder, "OutlastSaveManager.exe");

            // Eine Ebene höher
            string parentFolder = Directory.GetParent(exeFolder).FullName;
            parentFolder = Path.Combine(parentFolder ,"SaveManager");

            // Pfad zur .lnk-Datei
            string shortcutPath = Path.Combine(parentFolder, shortcutName + ".lnk");

            // COM-Objekt WScript.Shell
            Type t = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(t);

            // Verknüpfung erstellen
            var shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = exePath;
            shortcut.WorkingDirectory = exeFolder;
            shortcut.Arguments = arguments; // Parameter
            shortcut.Description = $"Boots {shortcutName} with arguments {arguments}";
            shortcut.IconLocation = exePath + ",0";
            shortcut.Save();
        }
        private static void macca()
        {
            removeAllMods();
            string modName = "Macca";
            string modPath = Path.Combine(startupPath,"SaveManager", "SaveManager-Mods", modName);
            string olgameConfigPath = Path.Combine(startupPath, "OLGame", "Config");

            File.Copy(Path.Combine(startupPath, "SaveManager", "SaveManager-Mods", "Macca","Config", "DefaultGame.ini"), Path.Combine(olgameConfigPath, "DefaultGame.ini"), true);
            File.Copy(Path.Combine(startupPath, "SaveManager", "SaveManager-Mods", "Macca","Config", "DefaultUI.ini"), Path.Combine(olgameConfigPath, "DefaultUI.ini"), true);

        }

        private static void superjump()
        {
            removeAllMods();
            string modName = "SuperJump";
            string modPath = Path.Combine(startupPath,"SaveManager", "SaveManager-Mods", modName);
            string olgameConfigPath = Path.Combine(startupPath, "OLGame", "Config");

            File.Copy(Path.Combine(startupPath,"SaveManager","SaveManager-Mods","SuperJump","DefaultGame.ini"),Path.Combine(olgameConfigPath,"DefaultGame.ini"), true);
            File.Copy(Path.Combine(startupPath, "SaveManager", "SaveManager-Mods", "SuperJump", "DefaultUI.ini"), Path.Combine(olgameConfigPath, "DefaultUI.ini"),true);

        }

        private static void vanilla()
        {
            removeAllMods();
        }

        private static void alldoorsunlocked()
           {
            string modName = "AllDoorsUnlocked";
            string modPath = Path.Combine(startupPath,"SaveManager", "SaveManager-Mods", modName);
            string olgameConfigPath = Path.Combine(startupPath, "OLGame", "Config");
            string olgameCustomPath = Path.Combine(startupPath, "OLGame", "CookedPCConsole", "Custom");

            // Alte Mod entfernen
            removeAllMods();

            // Backup der Config-Dateien
            string backupPath = Path.Combine(localAppDataPath,"SaveManager", "Backup", "Config");
            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(backupPath);

            foreach (string file in Directory.GetFiles(olgameConfigPath, "*.ini"))
            {
                string dest = Path.Combine(backupPath, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            // Config-Dateien kopieren (Config=true)
            string[] configFiles = { "DefaultEngine.ini", "DefaultUI.ini", "DefaultGame.ini" };
            foreach (string cfg in configFiles)
            {
                string src = Path.Combine(modPath, "Config", cfg);
                string dst = Path.Combine(olgameConfigPath, cfg);
                if (File.Exists(src))
                    File.Copy(src, dst, true);
            }

            // Content mounten (Content=true)
            string modContentPath = Path.Combine(modPath, "Content");
            string targetContentPath = Path.Combine(olgameCustomPath, $"{modName}.OLScript");

            if (Directory.Exists(modContentPath))
            {
                // Wenn Datei schon existiert, löschen
                if (File.Exists(targetContentPath))
                    File.Delete(targetContentPath);

                // Copy mod content file
                string contentFile = Path.Combine(modContentPath, "AllDoorsUnlocked.OLScript");
                if (File.Exists(contentFile))
                    File.Copy(contentFile, targetContentPath, true);
            }

            // Optional: Spiel starten oder andere Aktionen hier einfügen
            

        }

        private static string GetOLGameExePath()
        {
            string bitFolder = Path.Combine(localAppDataPath, "Bit");
            string exePath;

            if (File.Exists(Path.Combine(bitFolder, "32")))
            {
                exePath = Path.Combine(startupPath, "Binaries", "Win32", "OLGame.exe");
            }
            else if (File.Exists(Path.Combine(bitFolder, "64")))
            {
                exePath = Path.Combine(startupPath, "Binaries", "Win64", "OLGame.exe");
            }
            else
            {
                // Fallback: 64-Bit standard
                string fallbackFile = Path.Combine(bitFolder, "64");
                File.Create(fallbackFile).Dispose(); // Datei erzeugen
                exePath = Path.Combine(startupPath, "Binaries", "Win64", "OLGame.exe");
            }

            return exePath;
        }

        private static void LaunchOLGame()
        {
            string bitFolder = Path.Combine(localAppDataPath, "Bit");
            string exePath = GetOLGameExePath();
            string arguments = "-NoSteam -NOHOMEDIR";

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                });
        }

        private static void removeAllMods()
        {
            if (!Directory.Exists(Path.Combine(startupPath,"OLGame","CookedPCConsole","Custom")))
            {
                Directory.CreateDirectory(Path.Combine(startupPath, "OLGame", "CookedPCConsole", "Custom"));
            }
            try
            {
                File.Delete(Path.Combine(startupPath, "OLGame", "CookedPCConsole", "Custom", "AllDoorsUnlocked.OLScript"));
            }catch{ }
            
            File.Copy(Path.Combine(startupPath,"SaveManager", "SaveManager-Mods", "Vanilla","Config", "DefaultEngine.ini"), Path.Combine(startupPath,"OLGame","Config", "DefaultEngine.ini"),true);
            File.Copy(Path.Combine(startupPath,"SaveManager", "SaveManager-Mods", "Vanilla", "Config", "DefaultGame.ini"), Path.Combine(startupPath, "OLGame", "Config", "DefaultGame.ini"),true);
            File.Copy(Path.Combine(startupPath,"SaveManager", "SaveManager-Mods", "Vanilla", "Config", "DefaultUI.ini"), Path.Combine(startupPath, "OLGame", "Config", "DefaultUI.ini"), true);

        }



        /// <summary>;
        /// Prüft und kopiert bestimmte Dateien + Ordner vom Startpfad ins LocalAppData
        /// </summary>
        private static void checkFilesMovedAlready()
                {
                    string startPath = AppDomain.CurrentDomain.BaseDirectory;
                    string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveManager", "WebViewer");

                    if (!Directory.Exists(localPath))
                        Directory.CreateDirectory(localPath);

                    // Nur diese Dateien
                    string[] filesToCopy = {
                        "WebView2Loader.dll",
                        "Microsoft.Web.WebView2.Wpf.xml",
                        "Microsoft.Web.WebView2.WinForms.xml",
                        "Microsoft.Web.WebView2.Core.xml"
                    };

                    foreach (string fileName in filesToCopy)
                    {
                        string sourceFile = Path.Combine(startPath, fileName);
                        string destFile = Path.Combine(localPath, fileName);

                        if (File.Exists(sourceFile))
                        {
                            File.Copy(sourceFile, destFile, true);
                            File.Delete(sourceFile); // ← nach Kopieren löschen
                        }
                    }

                    // Ganze Ordner verschieben
                    string[] foldersToCopy = {
                        "runtimes",
                        "OutlastSaveManager.exe.WebView2"
                    };

                    foreach (string folderName in foldersToCopy)
                    {
                        string sourceFolder = Path.Combine(startPath, folderName);
                        string destFolder = Path.Combine(localPath, folderName);

                        if (Directory.Exists(sourceFolder))
                        {
                            CopyDirectory(sourceFolder, destFolder, true);
                            Directory.Delete(sourceFolder, true); // ← nach Kopieren löschen
                        }
                    }
                }

        /// <summary>
        /// Kopiert bestimmte Dateien + Ordner zurück vom LocalAppData ins Startverzeichnis
        /// </summary>
        private static void copyBackToStart()
        {
            string startPath = AppDomain.CurrentDomain.BaseDirectory;
            string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveManager", "WebViewer");

            if (!Directory.Exists(localPath))
                return;

            // Nur diese Dateien
            string[] filesToCopy = {
                "WebView2Loader.dll",
                "Microsoft.Web.WebView2.Wpf.xml",
                "Microsoft.Web.WebView2.WinForms.xml",
                "Microsoft.Web.WebView2.Core.xml"
            };

            foreach (string fileName in filesToCopy)
            {
                string sourceFile = Path.Combine(localPath, fileName);
                string destFile = Path.Combine(startPath, fileName);

                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, destFile, true);
                    File.Delete(sourceFile); // ← nach Kopieren löschen
                }
            }

            // Ganze Ordner zurück
            string[] foldersToCopy = {
                "runtimes",
                "OutlastSaveManager.exe.WebView2"
            };

            foreach (string folderName in foldersToCopy)
            {
                string sourceFolder = Path.Combine(localPath, folderName);
                string destFolder = Path.Combine(startPath, folderName);

                if (Directory.Exists(sourceFolder))
                {
                    CopyDirectory(sourceFolder, destFolder, true);
                    Directory.Delete(sourceFolder, true); // ← nach Kopieren löschen
                }
            }
        }

        /// <summary>
        /// Hilfsmethode: ganzen Ordner rekursiv kopieren
        /// </summary>
        private static void CopyDirectory(string sourceDir, string destDir, bool overwrite)
        {
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite);
            }

            foreach (string folder in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(folder));
                CopyDirectory(folder, destSubDir, overwrite);
            }
        }
    }
}
