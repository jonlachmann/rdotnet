using RDotNet.Internals;
using System;
using System.Runtime.Serialization;

namespace RDotNet
{
    /// <summary>
    /// Thrown when an engine comes to an error.
    /// </summary>
    [Serializable]
    public class ParseException : Exception
    // (http://msdn.microsoft.com/en-us/library/vstudio/system.applicationexception%28v=vs.110%29.aspx)
    // "If you are designing an application that needs to create its own exceptions,
    // you are advised to derive custom exceptions from the Exception class"
    {
        private const string StatusFieldName = "status";

        private const string ErrorStatementFieldName = "errorStatement";
        private readonly string _errorStatement;
        private readonly ParseStatus _status;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        private ParseException()
            : this(ParseStatus.Null, "", "")
        // This does not internally occur. See Parse.h in R_HOME/include/R_ext/Parse.h
        { }

        /// <summary>
        /// Creates a new instance with the specified error.
        /// </summary>
        /// <param name="status">The error status</param>
        /// <param name="errorStatement">The statement that failed to be parsed</param>
        /// <param name="errorMsg">The error message given by the native R engine</param>
        public ParseException(ParseStatus status, string errorStatement, string errorMsg)
            : base(MakeErrorMsg(status, errorStatement, errorMsg))
        {
            _status = status;
            _errorStatement = errorStatement;
        }

        private static string MakeErrorMsg(ParseStatus status, string errorStatement, string errorMsg)
        {
            return string.Format("Status {2} for {0} : {1}", errorStatement, errorMsg, status);
        }

        /// <summary>
        /// Creates a new ParseException
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialised object data about the exception being thrown.</param>
        /// <param name="context"></param>
        [Obsolete("Obsolete")]
        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _status = (ParseStatus)info.GetValue(StatusFieldName, typeof(ParseStatus));
            _errorStatement = info.GetString(ErrorStatementFieldName);
        }

        /// <summary>
        /// The error.
        /// </summary>
        public ParseStatus Status => _status;

        /// <summary>
        /// The statement caused the error.
        /// </summary>
        public string ErrorStatement => _errorStatement;

        /// <summary>
        /// Sets the serialization info about the exception thrown
        /// </summary>
        /// <param name="info">Serialised object data.</param>
        /// <param name="context">Contextual information about the source or destination</param>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(StatusFieldName, _status);
            info.AddValue(ErrorStatementFieldName, _errorStatement);
        }
    }
}
