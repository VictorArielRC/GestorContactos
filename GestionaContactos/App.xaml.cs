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

            // Diagnóstico: registra recursos embebidos y verifica nombres comunes XAML/BAML
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
                WriteExceptionDiagnostics(ex, "Creando o mostrando MainWindow");
                MessageBox.Show($"Error al iniciar MainWindow. Diagnóstico escrito en:\n{_logPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void App_DispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                WriteExceptionDiagnostics(e.Exception, "Excepción de UI no controlada");
                MessageBox.Show($"Excepción no controlada. Diagnóstico escrito en:\n{_logPath}", "Excepción no controlada", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        }

        private void LogResourceDiagnostics()
        {
            try
            {
                var sb = new StringBuilder();
                var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

                // Cabecera básica
                sb.AppendLine($"Marca temporal: {DateTime.UtcNow:O}");
                sb.AppendLine($"Ensamblado: {asm.FullName}");

                // Lista nombres de recursos del manifiesto
                var names = asm.GetManifestResourceNames();
                sb.AppendLine("NombresRecursosManifiesto:");
                foreach (var n in names)
                    sb.AppendLine("  " + n);

                // Candidatos a comprobar: variantes comunes XAML/BAML
                string[] xamls = new[] { "MainWindow.xaml", "App.xaml", "Themes/Executive.xaml", "views/mainwindow.xaml", "Views/MainWindow.xaml" };

                bool anyFound = false;

                foreach (var x in xamls)
                {
                    sb.AppendLine($"Comprobando '{x}':");
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
                            sb.AppendLine($"  Candidato '{candidate}' => Excepción al probar: {ex.Message}");
                        }

                        if (found) anyFound = true;
                        sb.AppendLine($"  Candidato '{candidate}' => {(found ? "ENCONTRADO" : "FALTANTE")}");
                    }
                }

                // Comprobaciones WPF adicionales usando pack URIs y APIs de recursos
                try
                {
                    string asmName = asm.GetName().Name;
                    string packTheme = $"pack://application:,,,/{asmName};component/Themes/Executive.xaml";
                    sb.AppendLine($"Probando pack URI tema: {packTheme}");
                    try
                    {
                        var rd = new ResourceDictionary();
                        rd.Source = new Uri(packTheme, UriKind.Absolute);
                        sb.AppendLine("  Tema cargado correctamente via pack URI");
                        anyFound = true;
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"  Falló carga pack URI: {ex.Message}");
                    }

                    // Intentar obtener MainWindow mediante Application.GetResourceStream
                    string mainWindowPack = $"/{asmName};component/Views/MainWindow.xaml";
                    sb.AppendLine($"Probando stream de recurso de aplicación: {mainWindowPack}");
                    try
                    {
                        var sri = Application.GetResourceStream(new Uri(mainWindowPack, UriKind.Relative));
                        if (sri?.Stream != null)
                        {
                            sb.AppendLine("  Stream de MainWindow encontrado");
                            anyFound = true;
                        }
                        else
                        {
                            sb.AppendLine("  Stream de MainWindow ausente");
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"  Prueba de recurso MainWindow falló: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"Comprobaciones WPF fallaron: {ex.Message}");
                }

                File.WriteAllText(_logPath, sb.ToString());
                Debug.WriteLine(sb.ToString());

                // Mostrar popup solo cuando no se encuentra ningún recurso
                if (!anyFound)
                {
                    MessageBox.Show($"Diagnóstico de recursos escrito en:\n{_logPath}", "Diagnóstico - Recursos faltantes", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Diagnóstico de recursos falló: " + ex);
                try { File.WriteAllText(_logPath, "Diagnóstico de recursos falló: " + ex); } catch { }
            }

            // Helper local: generar nombres candidatos
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
                sb.AppendLine($"Marca temporal: {DateTime.UtcNow:O}");
                sb.AppendLine($"Contexto: {context}");
                sb.AppendLine(ex.ToString());

                // Adjuntar diagnóstico previo si existe
                if (File.Exists(_logPath))
                {
                    sb.AppendLine();
                    sb.AppendLine("--- Diagnósticos previos ---");
                    sb.AppendLine(File.ReadAllText(_logPath));
                }

                File.WriteAllText(_logPath, sb.ToString());
                Debug.WriteLine(sb.ToString());
            }
            catch { }
        }
    }
}

