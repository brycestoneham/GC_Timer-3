using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

using Bentley.MicroStation.WinForms;
using Bentley.GenerativeComponents.UISupport;
using Bentley.GenerativeComponents;
using Bentley.GenerativeComponents.Features;
using Bentley.GenerativeComponents.Features.Specific;
using Bentley.GenerativeComponents.GCScript;
using Bentley.GenerativeComponents.MicroStation;
using Bentley.GenerativeComponents.API;
using Bentley.GenerativeComponents.GeneralPurpose.Wpf;
using Bentley.GenerativeComponents.View.ControlPanel;


namespace Cerver.Timer
{
    /// <summary>
    /// Interaction logic for timer controls
    /// </summary>
    public partial class TimerControl : UserControl , IControlPanelViewModelListener
    {
        

        private int frameCt = 0;
        private double timeInt = 25;
        private DispatcherTimer timer;
        private Feature[] objs;
        private Timer thisNode;

        
        public TimerControl()
        {
            
            timer = new DispatcherTimer();
            timer.Tick += gcTimer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(timeInt);
            this.DataContextChanged += NodeControl_DataContextChanged;
  
            InitializeComponent();

        }
        
        void IControlPanelViewModelListener.OnViewModelSaysToStopEditing()
        {
            //not implimented

        }
       
        // this is here because the controls are not valid untill the node has sucessfully generated. this is the event that gets triggered
        void NodeControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (thisNode == null)
            {
                thisNode = (Timer)this.DataContext;
                thisNode.Changed += thisNode_Changed;
                thisNode.NodeClosing += thisNode_NodeClosing;
                updateGC();
                
            }
            
        }

        void thisNode_NodeClosing(EventArgs e)
        {
            timer.Stop();
        }

        void thisNode_Changed(Timer m, EventArgs e)
        {
            timeInt = thisNode.interval;
            timer.Interval = TimeSpan.FromMilliseconds(timeInt);
            objs = thisNode.GCobjects;        
        }

        private void btPlay_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)btPlay.IsChecked)
            {
                thisNode_Changed(thisNode, EventArgs.Empty);
                if ((bool)this.btReset.IsChecked)
                {
                    btReset.IsChecked = false;
                }
                timer.Start();
            }
            else
            {
                timer.Stop();
            }

            if (thisNode != null)
            {
                thisNode.PlayPressed = (bool)btPlay.IsChecked;
                thisNode.ResetPressed = (bool)btReset.IsChecked;
                thisNode.Reset = (bool)btReset.IsChecked;

            }

        }


        private void btReset_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if ((bool)btReset.IsChecked)
            {
                timer.Stop();
                btPlay.IsChecked = false;
                frameCt = 0;
            }

            if (thisNode != null)
            {

                GCTools.UserModel.Lock(GCTools.UserModel);
                try
                {
                    thisNode.frameNum = frameCt.ToString();
                    thisNode.ResetPressed = (bool)btReset.IsChecked;
                    thisNode.Reset = (bool)btReset.IsChecked;
                    thisNode.UpdateNodeTree();
               

                }
                finally
                {
                    GCTools.UserModel.Unlock(GCTools.UserModel);
                }
                GCTools.SyncUpMicroStation();
                
            }

        }


        private void gcTimer_Tick(object sender, EventArgs e)
        {
            frameCt++;
            updateGC();
        }

        private void updateGC()
        {

            GCTools.UserModel.Lock(GCTools.UserModel);
            try
            {
                if (objs != null) APIHelper.UpdateNodeTree(objs);
                thisNode.frameNum = frameCt.ToString();
                thisNode.UpdateNodeTree();
                
            }
            finally
            {
                GCTools.UserModel.Unlock(GCTools.UserModel);
            }
            GCTools.SyncUpMicroStation();


        }

        /////public///////////////////////////////
        public void StopTimer()
        {
            if ((bool)btPlay.IsChecked)
            {
                if ((bool)this.btReset.IsChecked) btReset.IsChecked = false ;
                timer.Stop();
            }
        }


        
    }
}
