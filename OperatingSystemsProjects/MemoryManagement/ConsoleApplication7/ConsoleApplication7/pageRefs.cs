using System;

namespace ConsoleApplication7
{
    internal class pageRef : System.IEquatable<pageRef>
    {
        private int page;
        private int pID;
        private int count = 0;
        public pageRef(int pID, int page)
        {
            this.pID = pID;
            this.page = page;
        }

         public bool Equals (pageRef r)
        {
            if (r == null) return false;
            return (this.pID == r.pID && this.page == r.page);
        }

        override public string ToString()
        {
            
            return pID + " " + page;
        }

        public void incCount()
        {
            count++;
        }

        public int getCount()
        {
            return count;
        }

        internal void resetCount()
        {
            count = 0;
        }
    }
}