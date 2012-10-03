namespace Dev.IL.Engineering.SSMSHistorizer
{
    using System;
    using System.IO;

    using Dev.IL.Engineering.SSMSHistorizer.Properties;

    using EnvDTE;
    using EnvDTE80;

    using NLog;
    using LogManager = Dev.IL.Engineering.SSMSHistorizer.Utils.LogManager;

    public sealed class Historizer
    {
        #region Private Member

        private readonly DTE2 _Application;

        private readonly Logger _Logger;

        #endregion

        #region Private Properties

        private CommandEvents QueryExecuteEvent { get; set; }

        private DTE2 Application 
        { 
            get
            {
                return this._Application;
            }
        }

        private Logger Logger
        {
            get
            {
                return this._Logger;
            }
        }

        #endregion

        #region Constructor

        public Historizer(DTE2 application)
        {
            this._Application = application;
            this._Logger = LogManager.Instance.GetCurrentClassLogger();
        }

        #endregion

        #region Public Functions

        public void Initialize()
        {
            // bind query executed event (GUID={52692960-56BC-4989-B5D3-94C47A513E8D}, ID=1)
            this.QueryExecuteEvent = this.Application.DTE.Events.CommandEvents["{52692960-56BC-4989-B5D3-94C47A513E8D}", 1];
            this.QueryExecuteEvent.BeforeExecute += this.OnBeforeQueryExecuted;
            this.Logger.Debug("SSMSHistorizer initialized");
        }

        public void UnInitialize()
        {
            this.QueryExecuteEvent.BeforeExecute -= this.OnBeforeQueryExecuted;
            this.Logger.Debug("SSMSHistorizer uninitialized");
        }

        #endregion

        #region Private Functions

        private void OnBeforeQueryExecuted(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            try
            {
                var textDocument = (TextDocument)this._Application.DTE.ActiveDocument.Object("TextDocument");

                var editPoint = textDocument.StartPoint.CreateEditPoint();
                var query = editPoint.GetText(textDocument.EndPoint);

                var filename = this.GenerateUniqueFileName();
                var absoluteFilename = Path.Combine(ConfigurationSettings.Default.StoragePath, filename);

                File.WriteAllText(absoluteFilename, query);
            }
            catch (Exception e)
            {
                this.Logger.Error(e);
            }
        }

        private string GenerateUniqueFileName()
        {
            return string.Format("query-{0:yyyy-MM-dd_HH-mm-ss-fff}.sql", DateTime.Now);
        }

        #endregion
    }
}