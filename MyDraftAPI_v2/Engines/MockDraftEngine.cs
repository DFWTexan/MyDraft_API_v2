using Windows.Storage;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using MyDraftAPI_v2.FantasyDataModel;
using global::MyDraftAPI_v2.FantasyDataModel.Draft;

namespace MyDraftAPI_v2.Engines
{
    class MockDraftEngine
    {
        private MockDraftEngine() { }
        private static MockDraftEngine instance;
        public static MockDraftEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MockDraftEngine();
                }
                return instance;
            }
        }

        #region // Delegates //
        public delegate void MockDraftBeginEventHandler();
        public delegate void MockDraftEndEventHandler();
        #endregion // Delegates //

        #region // EventHandlers //
        public event MockDraftBeginEventHandler BeginMockDraft;
        public event MockDraftEndEventHandler EndMockDraft;
        #endregion // EventHandlers //

        #region // RaiseEvents //
        private void RaiseBeginMockDraft()
        {
            BeginMockDraft?.Invoke();
        }
        private void RaiseEndMockDraft()
        {
            EndMockDraft?.Invoke();
        }
        #endregion // RaiseEvents //

        // PUBLISHED EVENTS //
        public void MockDraftStarted()
        {
            RaiseBeginMockDraft();
        }
        public void MockDraftEnded()
        {
            RaiseEndMockDraft();
        }
    }
}


