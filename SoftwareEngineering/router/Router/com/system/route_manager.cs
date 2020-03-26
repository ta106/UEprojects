using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Router.com.gui;
using Router.com.system;
using Router.com.system.tools;

namespace router.com.system
{
    public class route_manager
    {
        private HashSet<kid> kids;
        private HashSet<vehicle> vehicles;
        private List<vehicle> routed_vehicles;

        private route_visualizer visualizer;

        public route_manager(HashSet<kid> kids, HashSet<vehicle> vehicles)
        {
            this.kids = kids;
            this.vehicles = vehicles;
        }

        public void computeRoutes()
        {
            //List<kid> list_of_kids = new List<kid>();
            //list_of_kids.AddRange(kids);

            //LocationsCollection kids_list = new LocationsCollection(list_of_kids);
            //if (!kids_list.get_distance_data_from_api())
            //{
            //    kids_list.get_geocode_data_from_api();
            //    kids_list.get_distance_based_on_geocode();
            //}

            //ClustersCollection clustercollection = new ClustersCollection(kids_list);
            //clustercollection.grouping();
            //List<vehicle> v = new List<vehicle>();
            //v.AddRange(vehicles);
            //List<kid> unassigned_kids = clustercollection.suit_to_vehicles(v);
            //HashSet<kid> u_kids = new HashSet<kid>(unassigned_kids);
            //v.Add(new vehicle(unassigned_kids.Count, "Unassigned Kids", u_kids));
            //HashSet<vehicle> final_vehicles = new HashSet<vehicle>(v);
            strategy algorithim = new SequentialCluster();
            algorithim.init(kids.ToList(), vehicles.ToList());
            routed_vehicles = algorithim.getVehicles(kids.ToList(), vehicles.ToList());

        }

        public List<vehicle> getRoutes()
        {
            return routed_vehicles;
        }

        public void printFiles(string file_dir)
        {
            foreach(var each in routed_vehicles)
            {
                Directory.CreateDirectory(file_dir);
                String output = each.getName() + " : \n";
                foreach(kid each2 in each.getKids())
                {
                    if(each2 != null)
                    output += (each2.getName() + "; " + each2.getAddress() + "\n");
                }
                File.WriteAllText(file_dir + each.getName() + ".txt", output);
            }
        }

        public void displayRoute()
        {
            visualizer = new route_visualizer(new HashSet<vehicle> (routed_vehicles));
            visualizer.Show();
        }
    }
}