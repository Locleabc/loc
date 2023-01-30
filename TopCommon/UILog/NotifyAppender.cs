using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using log4net;
using log4net.Core;
using System.Collections.ObjectModel;
using System.Windows;

namespace TopCom.LOG
{

    /// <summary>
    /// The appender we are going to bind to for our logging.
    /// </summary>
    public class NotifyAppender : AppenderSkeleton, INotifyPropertyChanged
    {
        #region Members and events
        public static int MaxUILogRow { get; set; } = 200;
        private readonly object UILogObject = new object();

        private static ObservableCollection<string> _notification = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Get or set the notification message.
        /// </summary>
        public ObservableCollection<string> Notification
        {
            get
            {
                return _notification;
            }

            set
            {
                if (_notification == value) return;

                _notification = value;
                OnPropertyChanged("Notification");
            }
        }

        /// <summary>
        /// Raise the change notification.
        /// </summary>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Get a reference to the log instance.
        /// </summary>
        public NotifyAppender Appender
        {
            get
            {
                return UILog.Appender;
            }

        }

        /// <summary>
        /// Append the log information to the notification.
        /// </summary>
        /// <param name="loggingEvent">The log event.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            Layout.Format(writer, loggingEvent);
            try  // In case, App is shutdown before Log did
            {
                if (Application.Current == null) return;

                Application.Current.Dispatcher.BeginInvoke((Action)delegate // <--- HERE
                {
                    lock (UILogObject)
                    {
                        if (Notification.Count >= MaxUILogRow)
                        {
                            Notification.RemoveAt(0);
                        }
                        Notification.Add(writer.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
