using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace xCollection
{
    public class Transition
    {
        public class Args : EventArgs
        {
        }

        internal class TransitionedPropertyInfo
        {
            public object startValue;

            public object endValue;

            public object target;

            public PropertyInfo propertyInfo;

            public IManagedType managedType;

            public TransitionedPropertyInfo copy()
            {
                TransitionedPropertyInfo transitionedPropertyInfo = new TransitionedPropertyInfo();
                transitionedPropertyInfo.startValue = startValue;
                transitionedPropertyInfo.endValue = endValue;
                transitionedPropertyInfo.target = target;
                transitionedPropertyInfo.propertyInfo = propertyInfo;
                transitionedPropertyInfo.managedType = managedType;
                return transitionedPropertyInfo;
            }
        }

        private class PropertyUpdateArgs : EventArgs
        {
            public object target;

            public PropertyInfo propertyInfo;

            public object value;

            public PropertyUpdateArgs(object t, PropertyInfo pi, object v)
            {
                target = t;
                propertyInfo = pi;
                value = v;
            }
        }

        private static IDictionary<Type, IManagedType> m_mapManagedTypes;

        private ITransitionType m_TransitionMethod = null;

        private IList<TransitionedPropertyInfo> m_listTransitionedProperties = new List<TransitionedPropertyInfo>();

        private Stopwatch m_Stopwatch = new Stopwatch();

        private object m_Lock = new object();

        internal IList<TransitionedPropertyInfo> TransitionedProperties
        {
            get
            {
                return m_listTransitionedProperties;
            }
        }

        public event EventHandler<Args> TransitionCompletedEvent;

        static Transition()
        {
            m_mapManagedTypes = new Dictionary<Type, IManagedType>();
            registerType(new ManagedType_Int());
            registerType(new ManagedType_Float());
            registerType(new ManagedType_Double());
            registerType(new ManagedType_Color());
            registerType(new ManagedType_String());
        }

        public static void run(object target, string strPropertyName, object destinationValue, ITransitionType transitionMethod)
        {
            Transition transition = new Transition(transitionMethod);
            transition.add(target, strPropertyName, destinationValue);
            transition.run();
        }

        public static void run(object target, string strPropertyName, object initialValue, object destinationValue, ITransitionType transitionMethod)
        {
            Utility.setValue(target, strPropertyName, initialValue);
            run(target, strPropertyName, destinationValue, transitionMethod);
        }

        public static void runChain(params Transition[] transitions)
        {
            new TransitionChain(transitions);
        }

        public Transition(ITransitionType transitionMethod)
        {
            m_TransitionMethod = transitionMethod;
        }

        public void add(object target, string strPropertyName, object destinationValue)
        {
            Type type = target.GetType();
            PropertyInfo property = type.GetProperty(strPropertyName);
            if (property == null)
                throw new Exception("Object: " + target.ToString() + " does not have the property: " + strPropertyName);
            Type propertyType = property.PropertyType;
            if (!m_mapManagedTypes.ContainsKey(propertyType))
                throw new Exception("Transition does not handle properties of type: " + propertyType.ToString());
            if (!property.CanRead || !property.CanWrite)
                throw new Exception("Property is not both gettable and settable: " + strPropertyName);
            IManagedType managedType = m_mapManagedTypes[propertyType];
            TransitionedPropertyInfo transitionedPropertyInfo = new TransitionedPropertyInfo();
            transitionedPropertyInfo.endValue = destinationValue;
            transitionedPropertyInfo.target = target;
            transitionedPropertyInfo.propertyInfo = property;
            transitionedPropertyInfo.managedType = managedType;
            lock (m_Lock)
            {
                m_listTransitionedProperties.Add(transitionedPropertyInfo);
            }
        }

        public void run()
        {
            foreach (TransitionedPropertyInfo listTransitionedProperty in m_listTransitionedProperties)
            {
                object value = listTransitionedProperty.propertyInfo.GetValue(listTransitionedProperty.target, null);
                listTransitionedProperty.startValue = listTransitionedProperty.managedType.copy(value);
            }
            m_Stopwatch.Reset();
            m_Stopwatch.Start();
            TransitionManager.getInstance().register(this);
        }

        internal void removeProperty(TransitionedPropertyInfo info)
        {
            lock (m_Lock)
            {
                m_listTransitionedProperties.Remove(info);
            }
        }

        internal void onTimer()
        {
            int iTime = (int)m_Stopwatch.ElapsedMilliseconds;
            double dPercentage;
            bool bCompleted;
            m_TransitionMethod.onTimer(iTime, out dPercentage, out bCompleted);
            IList<TransitionedPropertyInfo> list = new List<TransitionedPropertyInfo>();
            lock (m_Lock)
            {
                foreach (TransitionedPropertyInfo listTransitionedProperty in m_listTransitionedProperties)
                {
                    list.Add(listTransitionedProperty.copy());
                }
            }
            foreach (TransitionedPropertyInfo item in list)
            {
                object intermediateValue = item.managedType.getIntermediateValue(item.startValue, item.endValue, dPercentage);
                PropertyUpdateArgs args = new PropertyUpdateArgs(item.target, item.propertyInfo, intermediateValue);
                setProperty(this, args);
            }
            if (bCompleted)
            {
                m_Stopwatch.Stop();
                Utility.raiseEvent(this.TransitionCompletedEvent, this, new Args());
            }
        }

        private void setProperty(object sender, PropertyUpdateArgs args)
        {
            try
            {
                if (!isDisposed(args.target))
                {
                    ISynchronizeInvoke synchronizeInvoke = args.target as ISynchronizeInvoke;
                    if (synchronizeInvoke != null && synchronizeInvoke.InvokeRequired)
                    {
                        IAsyncResult asyncResult = synchronizeInvoke.BeginInvoke(new EventHandler<PropertyUpdateArgs>(setProperty), new object[2] { sender, args });
                        asyncResult.AsyncWaitHandle.WaitOne(50);
                    }
                    else
                        args.propertyInfo.SetValue(args.target, args.value, null);
                }
            }
            catch (Exception)
            {
            }
        }

        private bool isDisposed(object target)
        {
            Control control = target as Control;
            if (control == null)
                return false;
            if (control.IsDisposed || control.Disposing)
                return true;
            return false;
        }

        private static void registerType(IManagedType transitionType)
        {
            Type managedType = transitionType.getManagedType();
            m_mapManagedTypes[managedType] = transitionType;
        }
    }
}