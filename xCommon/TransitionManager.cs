using System.Collections.Generic;
using System.Timers;

namespace xCollection
{
    internal class TransitionManager
    {
        private static TransitionManager m_Instance;

        private IDictionary<Transition, bool> m_Transitions = new Dictionary<Transition, bool>();

        private Timer m_Timer = null;

        private object m_Lock = new object();

        public static TransitionManager getInstance()
        {
            if (m_Instance == null)
                m_Instance = new TransitionManager();
            return m_Instance;
        }

        public void register(Transition transition)
        {
            lock (m_Lock)
            {
                removeDuplicates(transition);
                m_Transitions[transition] = true;
                transition.TransitionCompletedEvent += onTransitionCompleted;
            }
        }

        private void removeDuplicates(Transition transition)
        {
            foreach (KeyValuePair<Transition, bool> transition2 in m_Transitions)
            {
                removeDuplicates(transition, transition2.Key);
            }
        }

        private void removeDuplicates(Transition newTransition, Transition oldTransition)
        {
            IList<Transition.TransitionedPropertyInfo> transitionedProperties = newTransition.TransitionedProperties;
            IList<Transition.TransitionedPropertyInfo> transitionedProperties2 = oldTransition.TransitionedProperties;
            for (int num = transitionedProperties2.Count - 1; num >= 0; num--)
            {
                Transition.TransitionedPropertyInfo transitionedPropertyInfo = transitionedProperties2[num];
                foreach (Transition.TransitionedPropertyInfo item in transitionedProperties)
                {
                    if (transitionedPropertyInfo.target == item.target && transitionedPropertyInfo.propertyInfo == item.propertyInfo)
                        oldTransition.removeProperty(transitionedPropertyInfo);
                }
            }
        }

        private TransitionManager()
        {
            m_Timer = new Timer(15.0);
            m_Timer.Elapsed += onTimerElapsed;
            m_Timer.Enabled = true;
        }

        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Timer == null)
                return;
            m_Timer.Enabled = false;
            IList<Transition> list;
            lock (m_Lock)
            {
                list = new List<Transition>();
                foreach (KeyValuePair<Transition, bool> transition in m_Transitions)
                {
                    list.Add(transition.Key);
                }
            }
            foreach (Transition item in list)
            {
                item.onTimer();
            }
            m_Timer.Enabled = true;
        }

        private void onTransitionCompleted(object sender, Transition.Args e)
        {
            Transition transition = (Transition)sender;
            transition.TransitionCompletedEvent -= onTransitionCompleted;
            lock (m_Lock)
            {
                m_Transitions.Remove(transition);
            }
        }
    }
}