namespace RvtTransponder.WebUtils
{
    internal class NewDataReceivedEventArgs
    {
        private readonly string _data;

        internal NewDataReceivedEventArgs(string data)
        {
            this._data = data;
        }

        internal string Data
        {
            get { return this._data; }
        }
    }
}