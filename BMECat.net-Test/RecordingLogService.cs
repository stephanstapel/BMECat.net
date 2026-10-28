using BMECat.net;

namespace BMECat.net_Test
{
    /// <summary>
    /// Keeps all logged messages, e.g. to check that a warning was issued
    /// </summary>
    internal class RecordingLogService : ILogService
    {
        public List<string> Warnings { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();


        public void Debug(string msg, params object[] args)
        {
        }


        public void Error(string msg, params object[] args)
        {
            Errors.Add(msg);
        }


        public void Info(string msg, params object[] args)
        {
        }


        public void Warn(string msg, params object[] args)
        {
            Warnings.Add(msg);
        }
    }
}
