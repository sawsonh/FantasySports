using System;

namespace FS.Core.Services
{
    public interface ILogService
    {
        void Debug(string format);
        void Debug(string format, params object[] args);
        void Error(string format);
        void Error(string format, params object[] args);
        void Error(string format, Exception exception);
        void Fatal(string format);
        void Fatal(string format, params object[] args);
        void Fatal(string format, Exception exception);
        void Info(string format, params object[] args);
        void Info(string format);
        void CatchException(string methodName, object[] parameters, Exception ex);
    }
}
