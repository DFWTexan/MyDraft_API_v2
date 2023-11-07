using ViewModel;

namespace System.Collections.Generic
{
    internal class IDictionaryDebugView<T1, T2>
    {
        private Dictionary<string, DraftPick> resData;

        public IDictionaryDebugView(Dictionary<string, DraftPick> resData)
        {
            this.resData = resData;
        }

        public object Items { get; internal set; }
    }
}