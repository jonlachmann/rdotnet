using RDotNet.Internals;
using RDotNet.Internals.Unix;
using RDotNet.NativeLibrary;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace RDotNet.Devices
{
    internal class CharacterDeviceAdapter : IDisposable
    {
        /// <summary>
        /// When R calls the character device (unamanged R calling managed code),
        /// it sometimes calls the method with 'this == null' when writing/reading
        /// from console (this seems to happen on Mono and may be a bug).
        ///
        /// The (somewhat incorrect) workaround is to keep the last device in a static
        /// field and use it when 'this == null' (the check is done in 'this.Device').
        /// This workarounds: http://rdotnet.codeplex.com/workitem/154
        /// </summary>
        private static ICharacterDevice _lastDevice;

        private readonly ICharacterDevice _device;
        private REngine _engine;

        private ptr_R_Suicide _suicideDelegate;
        private ptr_R_ShowMessage _showMessageDelegate;
        private ptr_R_ReadConsole _readConsoleDelegate;
        private ptr_R_WriteConsole _writeConsoleDelegate;
        private ptr_R_WriteConsoleEx _writeConsoleExDelegate;
        private ptr_R_ResetConsole _resetConsoleDelegate;
        private ptr_R_FlushConsole _flushConsoleDelegate;
        private ptr_R_ClearerrConsole _clearerrConsoleDelegate;
        private ptr_R_Busy _busyDelegate;
        private ptr_R_CleanUp _cleanUpDelegate;
        private ptr_R_ShowFiles _showFilesDelegate;
        private ptr_R_ChooseFile _chooseFileDelegate;
        private ptr_R_EditFile _editFileDelegate;
        private ptr_R_loadhistory _loadHistoryDelegate;
        private ptr_R_savehistory _saveHistoryDelegate;
        private ptr_R_addhistory _addHistoryDelegate;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="device">The implementation.</param>
        public CharacterDeviceAdapter(ICharacterDevice device)
        {
            ArgumentNullException.ThrowIfNull(device);
            _lastDevice = device;
            _device = device;
        }

        /// <summary>
        /// Gets the implementation of <see cref="ICharacterDevice"/> interface.
        /// </summary>
        public ICharacterDevice Device
        {
            get
            {
                if (this == null) return _lastDevice;
                else return _device;
            }
        }

        private REngine Engine => _engine;

        protected TDelegate GetFunction<TDelegate>() where TDelegate : class
        {
            return Engine.GetFunction<TDelegate>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.KeepAlive(this);
        }

        #endregion IDisposable Members

        internal void Install(REngine engine, StartupParameter parameter)
        {
            _engine = engine;
            switch (NativeUtility.GetPlatform())
            {
                case PlatformID.Win32NT:
                    SetupWindowsDevice(parameter);
                    break;

                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    SetupUnixDevice();
                    break;
            }
        }

        private void SetupWindowsDevice(StartupParameter parameter)
        {
            if (parameter.RHome == null)
            {
                parameter.Start.rhome = ToNativeUnixPath(NativeUtility.GetRHomeEnvironmentVariable());
            }
            if (parameter.Home == null)
            {
                var home = Marshal.PtrToStringAnsi(Engine.GetFunction<GetValue>("getRUser")());
                parameter.Start.home = ToNativeUnixPath(home);
            }
            if (IsMapped(nameof(ReadConsole)))
                parameter.Start.ReadConsole = ReadConsole;
            if (IsMapped(nameof(WriteConsole)))
                parameter.Start.WriteConsole = WriteConsole;
            if (IsMapped(nameof(WriteConsoleEx)))
                parameter.Start.WriteConsoleEx = WriteConsoleEx;
            if (IsMapped(nameof(Callback)))
                parameter.Start.CallBack = Callback;
            if (IsMapped(nameof(ShowMessage)))
                parameter.Start.ShowMessage = ShowMessage;
            if (IsMapped(nameof(Ask)))
                parameter.Start.YesNoCancel = Ask;
            if (IsMapped(nameof(Busy)))
                parameter.Start.Busy = Busy;
        }

        private static IntPtr ToNativeUnixPath(string path)
        {
            return Marshal.StringToHGlobalAnsi(ConvertSeparatorToUnixStylePath(path));
        }

        private bool IsMapped(string method)
        {
            var devType = _device.GetType();
            return devType.GetMethod(method)?.GetCustomAttribute<NotMappedAttribute>() == null;
        }

        private void SetupUnixDevice()
        {
            if (IsMapped(nameof(Suicide)))
            {
                _suicideDelegate = Suicide;
                var newSuicide = Marshal.GetFunctionPointerForDelegate(_suicideDelegate);
                _engine.WriteIntPtr("ptr_R_Suicide", newSuicide);
            }

            if (IsMapped(nameof(ShowMessage)))
            {
                _showMessageDelegate = ShowMessage;
                var newShowMessage = Marshal.GetFunctionPointerForDelegate(_showMessageDelegate);
                _engine.WriteIntPtr("ptr_R_ShowMessage", newShowMessage);
            }

            if (IsMapped(nameof(ReadConsole)))
            {
                _readConsoleDelegate = ReadConsole;
                var newReadConsole = Marshal.GetFunctionPointerForDelegate(_readConsoleDelegate);
                _engine.WriteIntPtr("ptr_R_ReadConsole", newReadConsole);
            }

            // Candidate fix for https://github.com/rdotnet/rdotnet/issues/131
            // Not sure when behavior changed in R character handling, but it seems that at some point
            // having R_outputfile set to not NULL e.g.:
            // in src/unix/system.c
            // R_Outputfile = stdout;
            // R_Consolefile = stderr;
            // took precedence over setting ptr_R_WriteConsole with a callback.
            // We need to reset these two values to a nullptr:
            _engine.WriteIntPtr("R_Outputfile", IntPtr.Zero);
            _engine.WriteIntPtr("R_Consolefile", IntPtr.Zero);


            if (IsMapped(nameof(WriteConsole)))
            {
                _writeConsoleDelegate = WriteConsole;
                var newWriteConsole = Marshal.GetFunctionPointerForDelegate(_writeConsoleDelegate);
                _engine.WriteIntPtr("ptr_R_WriteConsole", newWriteConsole);
            }

            if (IsMapped(nameof(WriteConsoleEx)))
            {
                _writeConsoleExDelegate = WriteConsoleEx;
                var newWriteConsoleEx = Marshal.GetFunctionPointerForDelegate(_writeConsoleExDelegate);
                _engine.WriteIntPtr("ptr_R_WriteConsoleEx", newWriteConsoleEx);
            }

            if (IsMapped(nameof(ResetConsole)))
            {
                _resetConsoleDelegate = ResetConsole;
                var newResetConsole = Marshal.GetFunctionPointerForDelegate(_resetConsoleDelegate);
                _engine.WriteIntPtr("ptr_R_ResetConsole", newResetConsole);
            }

            if (IsMapped(nameof(FlushConsole)))
            {
                _flushConsoleDelegate = FlushConsole;
                var newFlushConsole = Marshal.GetFunctionPointerForDelegate(_flushConsoleDelegate);
                _engine.WriteIntPtr("ptr_R_FlushConsole", newFlushConsole);
            }

            if (IsMapped(nameof(ClearErrorConsole)))
            {
                _clearerrConsoleDelegate = ClearErrorConsole;
                var newClearerrConsole = Marshal.GetFunctionPointerForDelegate(_clearerrConsoleDelegate);
                _engine.WriteIntPtr("ptr_R_ClearerrConsole", newClearerrConsole);
            }

            if (IsMapped(nameof(Busy)))
            {
                _busyDelegate = Busy;
                var newBusy = Marshal.GetFunctionPointerForDelegate(_busyDelegate);
                _engine.WriteIntPtr("ptr_R_Busy", newBusy);
            }

            if (IsMapped(nameof(CleanUp)))
            {
                _cleanUpDelegate = CleanUp;
                var newCleanUp = Marshal.GetFunctionPointerForDelegate(_cleanUpDelegate);
                _engine.WriteIntPtr("ptr_R_CleanUp", newCleanUp);
            }

            if (IsMapped(nameof(ShowFiles)))
            {
                _showFilesDelegate = ShowFiles;
                var newShowFiles = Marshal.GetFunctionPointerForDelegate(_showFilesDelegate);
                _engine.WriteIntPtr("ptr_R_ShowFiles", newShowFiles);
            }

            if (IsMapped(nameof(ChooseFile)))
            {
                _chooseFileDelegate = ChooseFile;
                var newChooseFile = Marshal.GetFunctionPointerForDelegate(_chooseFileDelegate);
                _engine.WriteIntPtr("ptr_R_ChooseFile", newChooseFile);
            }

            if (IsMapped(nameof(EditFile)))
            {
                _editFileDelegate = EditFile;
                var newEditFile = Marshal.GetFunctionPointerForDelegate(_editFileDelegate);
                _engine.WriteIntPtr("ptr_R_EditFile", newEditFile);
            }

            if (IsMapped(nameof(LoadHistory)))
            {
                _loadHistoryDelegate = LoadHistory;
                var newLoadHistory = Marshal.GetFunctionPointerForDelegate(_loadHistoryDelegate);
                _engine.WriteIntPtr("ptr_R_loadhistory", newLoadHistory);
            }

            if (IsMapped(nameof(SaveHistory)))
            {
                _saveHistoryDelegate = SaveHistory;
                var newSaveHistory = Marshal.GetFunctionPointerForDelegate(_saveHistoryDelegate);
                _engine.WriteIntPtr("ptr_R_savehistory", newSaveHistory);
            }

            if (IsMapped(nameof(AddHistory)))
            {
                _addHistoryDelegate = AddHistory;
                var newAddHistory = Marshal.GetFunctionPointerForDelegate(_addHistoryDelegate);
                _engine.WriteIntPtr("ptr_R_addhistory", newAddHistory);
            }
        }

        private static string ConvertSeparatorToUnixStylePath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        private bool ReadConsole(string prompt, StringBuilder buffer, int count, bool history)
        {
            buffer.Clear();
            var input = Device.ReadConsole(prompt, count, history);
            buffer.Append(input).Append('\n'); // input must end with '\n\0' ('\0' is appended during marshalling).
            return input != null;
        }

        private void WriteConsole(string buffer, int length)
        {
            WriteConsoleEx(buffer, length, ConsoleOutputType.None);
        }

        private void WriteConsoleEx(string buffer, int length, ConsoleOutputType outputType)
        {
            Device.WriteConsole(buffer, length, outputType);
        }

        private void ShowMessage(string message)
        {
            Device.ShowMessage(message);
        }

        private void Busy(BusyType which)
        {
            Device.Busy(which);
        }

        private void Callback()
        {
            Device.Callback();
        }

        private YesNoCancel Ask(string question)
        {
            return Device.Ask(question);
        }

        private void Suicide(string message)
        {
            Device.Suicide(message);
        }

        private void ResetConsole()
        {
            Device.ResetConsole();
        }

        private void FlushConsole()
        {
            Device.FlushConsole();
        }

        private void ClearErrorConsole()
        {
            Device.ClearErrorConsole();
        }

        private void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        {
            Device.CleanUp(saveAction, status, runLast);
        }

        private bool ShowFiles(int count, string[] files, string[] headers, string title, bool delete, string pager)
        {
            return Device.ShowFiles(files, headers, title, delete, pager);
        }

        private int ChooseFile(bool create, StringBuilder buffer, int length)
        {
            var path = Device.ChooseFile(create);
            return path == null ? 0 : Encoding.ASCII.GetByteCount(path);
        }

        private void EditFile(string file)
        {
            Device.EditFile(file);
        }

        private IntPtr CallDeviceFunction(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment, Func<Language, SymbolicExpression, Pairlist, REnvironment, SymbolicExpression> func)
        {
            var c = new Language(Engine, call);
            var op = new SymbolicExpression(Engine, operation);
            var arglist = new Pairlist(Engine, args);
            var env = new REnvironment(Engine, environment);
            var result = func(c, op, arglist, env);
            return result.DangerousGetHandle();
        }

        private IntPtr LoadHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
        {
            return CallDeviceFunction(call, operation, args, environment, Device.LoadHistory);
        }

        private IntPtr SaveHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
        {
            return CallDeviceFunction(call, operation, args, environment, Device.SaveHistory);
        }

        private IntPtr AddHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
        {
            return CallDeviceFunction(call, operation, args, environment, Device.AddHistory);
        }

        #region Nested type: getValue

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetValue();

        #endregion Nested type: getValue
    }
}
