using System;
using System.ComponentModel;
using System.Reflection;

namespace xCollection
{
    internal class Utility
    {
        public static object getValue(object target, string strPropertyName)
        {
            Type type = target.GetType();
            PropertyInfo property = type.GetProperty(strPropertyName);
            if (property == null)
                throw new Exception("Object: " + target.ToString() + " does not have the property: " + strPropertyName);
            return property.GetValue(target, null);
        }

        public static void setValue(object target, string strPropertyName, object value)
        {
            Type type = target.GetType();
            PropertyInfo property = type.GetProperty(strPropertyName);
            if (property == null)
                throw new Exception("Object: " + target.ToString() + " does not have the property: " + strPropertyName);
            property.SetValue(target, value, null);
        }

        public static double interpolate(double d1, double d2, double dPercentage)
        {
            double num = d2 - d1;
            double num2 = num * dPercentage;
            return d1 + num2;
        }

        public static int interpolate(int i1, int i2, double dPercentage)
        {
            return (int)interpolate((double)i1, (double)i2, dPercentage);
        }

        public static float interpolate(float f1, float f2, double dPercentage)
        {
            return (float)interpolate((double)f1, (double)f2, dPercentage);
        }

        public static double convertLinearToEaseInEaseOut(double dElapsed)
        {
            double num = ((dElapsed > 0.5) ? 0.5 : dElapsed);
            double num2 = ((dElapsed > 0.5) ? (dElapsed - 0.5) : 0.0);
            return 2.0 * num * num + 2.0 * num2 * (1.0 - num2);
        }

        public static double convertLinearToAcceleration(double dElapsed)
        {
            return dElapsed * dElapsed;
        }

        public static double convertLinearToDeceleration(double dElapsed)
        {
            return dElapsed * (2.0 - dElapsed);
        }

        public static void raiseEvent<T>(EventHandler<T> theEvent, object sender, T args) where T : EventArgs
        {
            if (theEvent == null)
                return;
            Delegate[] invocationList = theEvent.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                EventHandler<T> eventHandler = (EventHandler<T>)invocationList[i];
                try
                {
                    ISynchronizeInvoke synchronizeInvoke = eventHandler.Target as ISynchronizeInvoke;
                    if (synchronizeInvoke == null || !synchronizeInvoke.InvokeRequired)
                    {
                        eventHandler(sender, args);
                        continue;
                    }
                    synchronizeInvoke.BeginInvoke(eventHandler, new object[2] { sender, args });
                }
                catch (Exception)
                {
                }
            }
        }
    }
}