using RDotNet.Internals;
using RDotNet.Internals.Windows;
using System;
using System.Runtime.InteropServices;

namespace RDotNet
{
    /// <summary>
    /// Represents parameters on R's startup.
    /// </summary>
    /// <remarks>
    /// Wraps RStart struct.
    /// </remarks>
    public class StartupParameter
    {
        private static readonly ulong EnvironmentDependentMaxSize = Environment.Is64BitProcess ? ulong.MaxValue : uint.MaxValue;

        // Windows style RStart includes Unix-style RStart.
        internal RStart Start;

        /// <summary>
        /// Create a new Startup parameter, using some default parameters
        /// </summary>
        public StartupParameter()
        {
            Start = new RStart();
            SetDefaultParameter();
        }

        /// <summary>
        /// Gets or sets the value indicating that R runs as quiet mode.
        /// </summary>
        public bool Quiet
        {
            get => Start.Common.R_Quiet;
            set => Start.Common.R_Quiet = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R runs as slave mode.
        /// </summary>
        public bool Slave
        {
            get => Start.Common.R_Slave;
            set => Start.Common.R_Slave = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R runs as interactive mode.
        /// </summary>
        public bool Interactive
        {
            get => Start.Common.R_Interactive;
            set => Start.Common.R_Interactive = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R runs as verbose mode.
        /// </summary>
        public bool Verbose
        {
            get => Start.Common.R_Verbose;
            set => Start.Common.R_Verbose = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R loads the site file.
        /// </summary>
        public bool LoadSiteFile
        {
            get => Start.Common.LoadSiteFile;
            set => Start.Common.LoadSiteFile = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R loads the init file.
        /// </summary>
        public bool LoadInitFile
        {
            get => Start.Common.LoadInitFile;
            set => Start.Common.LoadInitFile = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R debugs the init file.
        /// </summary>
        public bool DebugInitFile
        {
            get => Start.Common.DebugInitFile;
            set => Start.Common.DebugInitFile = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R restores the history.
        /// </summary>
        public StartupRestoreAction RestoreAction
        {
            get => Start.Common.RestoreAction;
            set => Start.Common.RestoreAction = value;
        }

        /// <summary>
        /// Gets or sets the value indicating that R saves the history.
        /// </summary>
        public StartupSaveAction SaveAction
        {
            get => Start.Common.SaveAction;
            set => Start.Common.SaveAction = value;
        }

        /// <summary>
        /// Gets or sets the minimum memory size.
        /// </summary>
        public ulong MinMemorySize
        {
            get => Start.Common.vsize.ToUInt64();
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EnvironmentDependentMaxSize);
                Start.Common.vsize = new UIntPtr(value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum number of cons cells.
        /// </summary>
        public ulong MinCellSize
        {
            get => Start.Common.nsize.ToUInt64();
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EnvironmentDependentMaxSize);
                Start.Common.nsize = new UIntPtr(value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum memory size.
        /// </summary>
        public ulong MaxMemorySize
        {
            get => Start.Common.max_vsize.ToUInt64();
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EnvironmentDependentMaxSize);
                Start.Common.max_vsize = new UIntPtr(value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of cons cells.
        /// </summary>
        public ulong MaxCellSize
        {
            get => Start.Common.max_nsize.ToUInt64();
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EnvironmentDependentMaxSize);
                Start.Common.max_nsize = new UIntPtr(value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of protected pointers in stack.
        /// </summary>
        public ulong StackSize
        {
            get => Start.Common.ppsize.ToUInt64();
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EnvironmentDependentMaxSize);
                Start.Common.ppsize = new UIntPtr(value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that R does NOT load the environment file.
        /// </summary>
        public bool NoRenviron
        {
            get => Start.Common.NoRenviron;
            set => Start.Common.NoRenviron = value;
        }

        /// <summary>
        /// Gets or sets the base directory in which R is installed.
        /// </summary>
        /// <remarks>
        /// Only Windows.
        /// </remarks>
        public string RHome
        {
            get
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    throw new NotSupportedException();
                }
                return Marshal.PtrToStringAnsi(Start.rhome);
            }
            set
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    throw new NotSupportedException();
                }
                Start.rhome = Marshal.StringToHGlobalAnsi(value);
            }
        }

        /// <summary>
        /// Gets or sets the default user workspace.
        /// </summary>
        /// <remarks>
        /// Only Windows.
        /// </remarks>
        public string Home
        {
            get
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    throw new NotSupportedException();
                }
                return Marshal.PtrToStringAnsi(Start.home);
            }
            set
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    throw new NotSupportedException();
                }
                Start.home = Marshal.StringToHGlobalAnsi(value);
            }
        }

        /// <summary>
        /// Gets or sets the UI mode.
        /// </summary>
        /// <remarks>
        /// Only Windows.
        /// </remarks>
        public UiMode CharacterMode
        {
            get
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    throw new NotSupportedException();
                }
                return Start.CharacterMode;
            }
            set
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    throw new NotSupportedException();
                }
                Start.CharacterMode = value;
            }
        }

        private void SetDefaultParameter()
        {
            Quiet = true;
            //Slave = false;
            Interactive = true;
            //Verbose = false;
            RestoreAction = StartupRestoreAction.NoRestore;
            SaveAction = StartupSaveAction.NoSave;
            LoadSiteFile = true;
            LoadInitFile = true;
            //DebugInitFile = false;
            MinMemorySize = 6291456;
            MinCellSize = 350000;
            MaxMemorySize = EnvironmentDependentMaxSize;
            MaxCellSize = EnvironmentDependentMaxSize;
            StackSize = 50000;
            //NoRenviron = false;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                CharacterMode = UiMode.LinkDll;
            }
        }
    }
}
