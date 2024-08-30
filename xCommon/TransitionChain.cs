using System.Collections.Generic;

namespace xCollection
{
    internal class TransitionChain
    {
        private LinkedList<Transition> m_listTransitions = new LinkedList<Transition>();

        public TransitionChain(params Transition[] transitions)
        {
            foreach (Transition value in transitions)
            {
                m_listTransitions.AddLast(value);
            }
            runNextTransition();
        }

        private void runNextTransition()
        {
            if (m_listTransitions.Count != 0)
            {
                Transition value = m_listTransitions.First.Value;
                value.TransitionCompletedEvent += onTransitionCompleted;
                value.run();
            }
        }

        private void onTransitionCompleted(object sender, Transition.Args e)
        {
            Transition transition = (Transition)sender;
            transition.TransitionCompletedEvent -= onTransitionCompleted;
            m_listTransitions.RemoveFirst();
            runNextTransition();
        }
    }
}