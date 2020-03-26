using System;
using System.Collections.Generic;

namespace ConsoleApplication7
{
    internal class memoryManager
    {
        private List<pageRef> pRefs = new List<pageRef>();
        private int frames;
        private int fifoPFaults, OPTPFaults, LRUPFults;
        public memoryManager(List<string> refs , int frames)
        {
            this.frames = frames;

            foreach(string each in refs){
                string [] splitter = each.Split(' ');
                
                int PID = Convert.ToInt32(splitter[0]);
                int Page = Convert.ToInt32(splitter[1]);
                pageRef r = new pageRef(PID, Page);
                pRefs.Add(r);

            }
        }

        public void OPTSim()
        {
            List<pageRef> l = new List<pageRef>();
            int i;
            for (i = 0; i < pRefs.Count; i++)
            {
                if (!l.Contains(pRefs[i]))
                {
                    OPTPFaults++;
                    if (l.Count < frames)
                    {
                        l.Add(pRefs[i]);
                    }
                    else

                    if (l.Count == frames)
                    {
                        int index = i + 1;
                        List<pageRef> survivors = new List<pageRef>();
                        int j = 0;
                        while (j < frames-1 && index < pRefs.Count)
                        {
                            if (!survivors.Contains(pRefs[index]) && l.Contains(pRefs[index]))
                            {
                                survivors.Add(pRefs[index]);
                                j++;
                            }
                            index = index + 1;
                        }
                        foreach (pageRef p in l)
                        {
                            if (survivors.Count == frames - 1) break;
                            if ( !survivors.Contains(p))
                            {
                                survivors.Add(p);
                            }
                        }
                        survivors.Add(pRefs[i]);
                        l = survivors;

                        
                    }
                }

            }
            Console.WriteLine(OPTPFaults);
        }

        public void LRUSim()
        {
            List<pageRef> l = new List<pageRef>();
            int i;
            for(i=0; i< pRefs.Count; i++)
            {
                if (!l.Contains(pRefs[i]))
                {
                    LRUPFults++;
                    if (l.Count < frames)
                    {
                        l.Add(pRefs[i]);
                    }else

                    if (l.Count == frames)
                    {

                        int index = i - 1;
                        List<pageRef> victums = new List<pageRef>();
                        int j = 0;
                        while (j < frames && index > -1)
                        {
                            if (!victums.Contains(pRefs[index]) && l.Contains(pRefs[index]))
                            {
                                victums.Add(pRefs[index]);
                                j++;
                            }
                            index = index - 1;
                        }
                        l.Remove(victums[victums.Count - 1]);
                        l.Add(pRefs[i]);
                    }
                }
            }
            Console.WriteLine(LRUPFults);

        }

        public void FIFOSim()
        {
            List<pageRef> l = new List<pageRef>();
            
            foreach(pageRef each in pRefs)
            {
                if (!l.Contains(each))
                {
                    fifoPFaults++;
                    if (l.Count < frames )
                    {
                        l.Add(each);
                    }else

                    if(l.Count == frames)
                    {
                        pageRef v = l[0];
                       foreach(pageRef each1 in l)
                        {
                            if(each1.getCount() > v.getCount())
                            {
                                v = each1;
                            }
                        }
                        v.resetCount();
                        l.Remove(v);
                        l.Add(each);
                    }

                }else
                {
                    each.resetCount();
                }
                foreach (pageRef incs in l)
                {
                    incs.incCount();
                }
            }

            Console.WriteLine(fifoPFaults);

        }


    }
}