using System.Windows;
using Datos;
using Microsoft.EntityFrameworkCore;
using Servicios;
using GestionaContactos.Views;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Threading;

namespace GestionaContactos
{
    public partial class App : Application
    {
        private string _logPath = Path.Combine(Path.GetTempPath(), "GestorContactos_resources_log.txt");

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Diagnostic: log embedded resources and check common XAML/BAML names
            LogResourceDiagnostics();

            var options = new DbContextOptionsBuilder<GestorContactosDbContext>()
                .UseSqlServer("Server=localhost;Database=GestorContactosDB;Trusted_Connection=True;Encrypt=False;")
                .Options;

            var db = new GestorContactosDbContext(options);
            await db.Database.EnsureCreatedAsync();

            var repo = new RepositorioContactos(db);
            var servicio = new ServiciodeContacto(repo);

            try
            {
                var ventana = new MainWindow(servicio);
                ventana.Show();
            }
            catch (Exception ex)
            {
                WriteExceptionDiagnostics(ex, "Creating or showing MainWindow");
                MessageBox.Show($"Error launching MainWindow. Diagnostics written to:\n{_logPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void App_DispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                WriteExceptionDiagnostics(e.Exception, "Unhandled UI exception");
                MessageBox.Show($"Unhandled exception. Diagnostics written to:\n{_logPath}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        }

        private void LogResourceDiagnostics()
        {
            try
            {
                var sb = new StringBuilder();
                var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

                // Basic header info
                sb.AppendLine($"Timestamp: {DateTime.UtcNow:O}");
                sb.AppendLine($"Assembly: {asm.FullName}");

                // List manifest resource names
                var names = asm.GetManifestResourceNames();
                sb.AppendLine("ManifestResourceNames:");
                foreach (var n in names)
                    sb.AppendLine("  " + n);

                // Candidates to check -- include common XAML/BAML variants
                string[] xamls = new[] { "MainWindow.xaml", "App.xaml", "Themes/Executive.xaml", "views/mainwindow.xaml", "Views/MainWindow.xaml" };

                bool anyFound = false;

                foreach (var x in xamls)
                {
                    sb.AppendLine($"Checking '{x}':");
                    string baml = Path.ChangeExtension(x, ".baml");

                    foreach (var candidate in GetCandidates(asm, x, baml).Distinct())
                    {
                        bool found = false;
                        try
                        {
                            using (var s = asm.GetManifestResourceStream(candidate))
                            {
                                found = s != null;
                            }
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine($"  Candidate '{candidate}' => Exception when probing: {ex.Message}");
                        }

                        if (found) anyFound = true;
                        sb.AppendLine($"  Candidate '{candidate}' => {(found ? "FOUND" : "MISSING")}");
                    }
                }

                // Additional WPF-specific checks using pack URIs and resource APIs
                try
                {
                    string asmName = asm.GetName().Name;
                    string packTheme = $"pack://application:,,,/{asmName};component/Themes/Executive.xaml";
                    sb.AppendLine($"Trying pack URI theme: {packTheme}");
                    try
                    {
                        var rd = new ResourceDictionary();
                        rd.Source = new Uri(packTheme, UriKind.Absolute);
                        sb.AppendLine("  Pack URI theme loaded successfully");
                        anyFound = true;
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"  Pack URI theme failed: {ex.Message}");
                    }

                    // Try to get MainWindow via Application resource stream
                    string mainWindowPack = $"/{asmName};component/Views/MainWindow.xaml";
                    sb.AppendLine($"Trying application resource stream: {mainWindowPack}");
                    try
                    {
                        var sri = Application.GetResourceStream(new Uri(mainWindowPack, UriKind.Relative));
                        if (sri?.Stream != null)
                        {
                            sb.AppendLine("  MainWindow resource stream found");
                            anyFound = true;
                        }
                        else
                        {
                            sb.AppendLine("  MainWindow resource stream missing");
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"  MainWindow resource probe failed: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"WPF-specific checks failed: {ex.Message}");
                }

                File.WriteAllText(_logPath, sb.ToString());
                Debug.WriteLine(sb.ToString());

                // Only show a popup when no resources were found
                if (!anyFound)
                {
                    MessageBox.Show($"Resource diagnostics written to:\n{_logPath}", "Diagnostics - Resources missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Resource diagnostics failed: " + ex);
                try { File.WriteAllText(_logPath, "Resource diagnostics failed: " + ex); } catch { }
            }

            // Local helper: build candidate names
            static IEnumerable<string> GetCandidates(Assembly asm, string xaml, string baml)
            {
                yield return xaml;
                yield return xaml.Replace('/', '.');
                yield return xaml.ToLowerInvariant();
                yield return xaml.ToLowerInvariant().Replace('/', '.');
                yield return Path.GetFileName(xaml);
                yield return baml;
                yield return baml.Replace('/', '.');
                yield return Path.GetFileName(baml);
                yield return "Themes." + Path.GetFileName(baml);
                yield return asm.GetName().Name + "." + baml.Replace('/', '.');
                yield return asm.GetName().Name + "." + xaml.Replace('/', '.');
            }
        }

        private void WriteExceptionDiagnostics(Exception ex, string context)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Timestamp: {DateTime.UtcNow:O}");
                sb.AppendLine($"Context: {context}");
                sb.AppendLine(ex.ToString());

                // Append previous diagnostics if present
                if (File.Exists(_logPath))
                {
                    sb.AppendLine();
                    sb.AppendLine("--- Previous diagnostics ---");
                    sb.AppendLine(File.ReadAllText(_logPath));
                }

                File.WriteAllText(_logPath, sb.ToString());
                Debug.WriteLine(sb.ToString());
            }
            catch { }
        }
    }
}

