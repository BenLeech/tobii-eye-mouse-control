using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tobii.Interaction;
using WindowsInput;

namespace TobiiFormApp
{
    static class Program
    {
        private static Point prevPos;
        private static bool hasPrevPos;
        private static float alpha = 0.3f;

        private enum filters {Smooth, Averaged, Unfiltered};

        private static bool enableGazeMouseControl = false;
        private static int currentFilter;

        private static Form1 form;
        private static IKeyboardMouseEvents m_GlobalHook;

        private static void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                InputSimulator inputSimulator = new InputSimulator(); //!!!!!
                inputSimulator.Mouse.LeftButtonDown();
            }
        }

        private static void GlobalHookKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                InputSimulator inputSimulator = new InputSimulator(); //!!!!!
                inputSimulator.Mouse.LeftButtonUp();
            }
        }

        //Moves the mouse cursor and applies filter based on the currently selected setting
        private static void moveCursor(int x, int y)
        {
            Cursor.Position = SmoothFilter(new Point(x,y));
        }

        private static void subscribeGlobalKeyHook()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyDown += GlobalHookKeyDown;
            m_GlobalHook.KeyUp += GlobalHookKeyUp;
        }

        private static void unsubscribeGlobalKeyHook()
        {
            m_GlobalHook.KeyDown -= GlobalHookKeyDown;
            m_GlobalHook.KeyUp -= GlobalHookKeyUp;
            m_GlobalHook.Dispose();
        }

        //Applies a filter to the point based on currently selected setting
        private static Point SmoothFilter(Point point)
        {
            //checks which filter is selected
            checkFilterSettings();

            Point filteredPoint = point;

            if (!hasPrevPos)
            {
                prevPos = point;
                hasPrevPos = true;
            }

            if(currentFilter == (int)filters.Smooth)
            {
                filteredPoint = new Point((int)((point.X * alpha) + (prevPos.X * (1.0f - alpha))),
                                                (int)((point.Y * alpha) + (prevPos.Y * (1.0f - alpha))));
            }else if(currentFilter == (int)filters.Averaged)  //takes the average of the current point and the previous point
            {
                filteredPoint = new Point((point.X + prevPos.X) / 2, (point.Y + prevPos.Y) / 2);
            }

            prevPos = filteredPoint; //set the previous point to current point

            return filteredPoint;
        }

        private static void toggleGazeMouse(object sender, EventArgs e)
        {
            enableGazeMouseControl = !enableGazeMouseControl;
        }

        private static void checkFilterSettings()
        {
            if (form.radioButton1.Checked) //Smooth alpha filter
            {
                currentFilter = (int)filters.Smooth;
            }
            else if (form.radioButton2.Checked) //Averaged filter
            {
                currentFilter = (int)filters.Averaged;
            }
            else //unfiltered
            {
                currentFilter = (int)filters.Unfiltered;
            }
        }

        [STAThread]
        static void Main()
        {
            var host = new Host();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new Form1();

            subscribeGlobalKeyHook();

            //handle the 'toggle gaze control' button event
            form.button1.Click += new System.EventHandler(toggleGazeMouse);

            //create the data stream
            var gazePointDataStream = host.Streams.CreateGazePointDataStream(Tobii.Interaction.Framework.GazePointDataMode.LightlyFiltered);
            gazePointDataStream.GazePoint((x, y, _) =>
            {
                if (enableGazeMouseControl)
                {
                    moveCursor((int)x, (int)y);
                    //update the form labels with gaze coordinate
                    form.label3.Invoke((MethodInvoker)(() => form.label3.Text = x.ToString()));
                    form.label4.Invoke((MethodInvoker)(() => form.label4.Text = y.ToString()));
                }
                
            });

            Application.Run(form);
        }
    }
}
