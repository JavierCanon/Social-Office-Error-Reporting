using System;
using System.Reflection;
using ExceptionReporter.Config;
using ExceptionReporter.Core;

namespace ExceptionReporter
{
    public class ExceptionReporter
    {
        private readonly ExceptionReportInfo _reportInfo;
        private IExceptionReportView _reportView;
        private readonly IInternalExceptionView _internalExceptionView;
    	private readonly ViewInjector _viewInjector;

    	/// <summary>
        /// initialise the ExceptionReporter
        /// <remarks>readConfig() should be called (explicitly) if you need to override default config</remarks>
        /// </summary>
        public ExceptionReporter()
        {
            _reportInfo = new ExceptionReportInfo();
    		_viewInjector = new ViewInjector(Assembly.GetExecutingAssembly());
    		_internalExceptionView = _viewInjector.Resolve<IInternalExceptionView>();
        }

		// ReSharper disable UnusedMember.Global
		/// <summary>
		/// public access to configuration
		/// </summary>
        public ExceptionReportInfo Config
        {
            get { return _reportInfo; }
        }
    	// ReSharper restore UnusedMember.Global

        /// <summary>
        /// Show the <see cref="IExceptionReportView"/> dialog
        /// </summary>
        /// <remarks>The <see cref="ExceptionReporter"/> will analyze the <see cref="Exception"/>s and create and show the report dialog.</remarks>
        /// <param name="exceptions">The <see cref="Exception"/>s to show.</param>
        public void Show(params Exception[] exceptions)
        {
            if (exceptions == null) return;		//TODO perhaps show a dialog that says "No exception to show" ?

            try
            {
                _reportInfo.SetExceptions(exceptions);
				_reportView = _viewInjector.Resolve<IExceptionReportView>(_reportInfo);
                _reportView.ShowExceptionReport();
            }
            catch (Exception internalException)
            {
				_internalExceptionView.ShowException("Unable to show Exception Report", internalException);
            }
        }

        public void Show(string customMessage, params Exception[] exceptions)
        {
            _reportInfo.CustomMessage = customMessage;
            Show(exceptions);
        }

        /// <summary>
        /// Read settings from config file
        /// <remarks> This method must be explicitly called - config is not automatically read</remarks>
        /// </summary>
        public void ReadConfig()
        {
            try
            {
                var configReader = new ConfigReader(_reportInfo);
                configReader.ReadConfig();
            }
            catch (Exception ex)
            {	
				_internalExceptionView.ShowException(
					"Unable to read ExceptionReporter configuration - default values will be used", ex);
            }
        }
    }
}