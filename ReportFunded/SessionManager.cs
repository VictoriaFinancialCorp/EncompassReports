using EllieMae.Encompass.Client;

namespace ReportFunded
{
    class SessionManager
    {
        private Session mySession;

        public SessionManager()
        {
            this.mySession = Utility.ConnectToServer();
        }
        public Session getSession()
        {
            return this.mySession;
        }
        public void closeSession()
        {
            mySession.End();
        }
    }
}