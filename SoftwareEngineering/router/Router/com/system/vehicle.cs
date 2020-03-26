using System;
using System.Collections.Generic;
using static Router.com.system.LocationsCollection;

namespace router.com.system
{
    public class vehicle
    {
        private int capacity;
        private string name;
        private HashSet<kid> kids_list = new HashSet<kid>();
        private bool full;

        public vehicle(int vehicle_capacity, string vehicle_name, HashSet<kid> list)
        {
            capacity = vehicle_capacity;
            name = vehicle_name;
            if (list != null)
                kids_list = list;
        }

        public vehicle(int vehicle_capacity, string vehicle_name)
        {
            capacity = vehicle_capacity;
            name = vehicle_name;
            full = false;
        }

        public vehicle(string name_and_capacity)
        {
            int idx = name_and_capacity.IndexOf('/');
            this.name = name_and_capacity.Substring(0, idx);
            this.capacity = int.Parse(name_and_capacity.Substring(idx + 1));
        }

        public int getCapacity()
        {
            return capacity;
        }

        public string getName()
        {
            return name;
        }

        public HashSet<kid> getKids()
        {
            return kids_list;
        }

        public bool Equals(object obj)
        {
            return ((vehicle)obj).capacity == capacity && ((vehicle)obj).name == name;
        }


        public void fill_kids(List<location> list)
        {
            foreach (location each in list)
            {
                kids_list.Add(each.get_location_info());
            }
            full = true;
        }

        public bool has_filled_kids()
        {
            return full;
        }


        public override int GetHashCode()
        {
            return name.GetHashCode() + capacity.GetHashCode();
        }

        public void load_kid(kid k)
        {
            kids_list.Add(k);
        }

        public override string ToString()
        {
            return this.name + "/" + this.capacity;
        }

        public void addKid(kid kid)
        {
            kids_list.Add(kid);
        }

        public void addKids(HashSet<kid> kids)
        {
            kids_list = kids;
        }
    }
}
