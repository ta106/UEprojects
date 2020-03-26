using Microsoft.VisualStudio.TestTools.UnitTesting;
using router.com.system;
using Router.com.system;
using System.Collections.Generic;

namespace RouterUnitTests
{
    [TestClass]
    public class ClustersCollection_test
    {
        [TestMethod]
        public void Grouping_test()
        {
            List<kid> addresses = new List<kid>() {new kid("kid1", "1272 Burdette Ave, Evansville, IN"),
                               new kid("kid2", "1800 Lincoln Ave, Evansville, IN"),
                               new kid("kid3", "401 N Burkhardt Rd, Evansville, IN"),
                               new kid("kid4", "6501 E Lloyd Expy, Evansville, IN"),
                               new kid("kid5", "800 N Green River Rd, Evansville, IN")};
            LocationsCollection a = new LocationsCollection(addresses);
            Assert.AreEqual(a.get_geocode_data_from_api(), true);
            Assert.AreEqual(a.get_distance_based_on_geocode(), true);

            ClustersCollection c = new ClustersCollection(a);
            c.grouping();

            Assert.AreEqual(a.find_location(addresses[0]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[1]).get_cluster(), 1);
            Assert.AreEqual(a.find_location(addresses[2]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[3]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[4]).get_cluster(), 0);
        }

        [TestMethod]
        public void Fill_vehicles_test1()
        {
            List<kid> addresses = new List<kid>() {new kid("kid1", "1272 Burdette Ave, Evansville, IN"),
                               new kid("kid2", "1800 Lincoln Ave, Evansville, IN"),
                               new kid("kid3", "401 N Burkhardt Rd, Evansville, IN"),
                               new kid("kid4", "6501 E Lloyd Expy, Evansville, IN"),
                               new kid("kid5", "800 N Green River Rd, Evansville, IN")};
            LocationsCollection a = new LocationsCollection(addresses);
            Assert.AreEqual(a.get_geocode_data_from_api(), true);
            Assert.AreEqual(a.get_distance_based_on_geocode(), true);

            ClustersCollection c = new ClustersCollection(a);
            c.grouping();

            Assert.AreEqual(c.num_of_clusters(), 2);
            Assert.AreEqual(a.find_location(addresses[0]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[1]).get_cluster(), 1);
            Assert.AreEqual(a.find_location(addresses[2]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[3]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[4]).get_cluster(), 0);


            List<vehicle> vehicles = new List<vehicle>();
            List<kid> unassigned_kids;
            vehicles.Add(new vehicle(1, "v1"));
            vehicles.Add(new vehicle(1, "v2"));
            vehicles.Add(new vehicle(1, "v3"));
            vehicles.Add(new vehicle(1, "v4"));
            unassigned_kids = c.suit_to_vehicles(vehicles);
            Assert.AreEqual(unassigned_kids[0], addresses[1]);

            List<vehicle> test = new List<vehicle>();
            test.Add(new vehicle(1, "v1", new HashSet<kid>() { addresses[2] }));
            test.Add(new vehicle(1, "v2", new HashSet<kid>() { addresses[3] }));
            test.Add(new vehicle(1, "v3", new HashSet<kid>() { addresses[4] }));
            test.Add(new vehicle(1, "v4", new HashSet<kid>() { addresses[0] }));
            Assert.IsTrue(compare_vehicle_lists(test, vehicles));
        }

        [TestMethod]
        public void Fill_vehicles_test2()
        {
            List<kid> addresses = new List<kid>() {new kid("kid1", "1272 Burdette Ave, Evansville, IN"),
                               new kid("kid2", "1800 Lincoln Ave, Evansville, IN"),
                               new kid("kid3", "401 N Burkhardt Rd, Evansville, IN"),
                               new kid("kid4", "6501 E Lloyd Expy, Evansville, IN"),
                               new kid("kid5", "800 N Green River Rd, Evansville, IN"),
                               new kid("kid6", "4605 Bellemeade Avenue, Evansville, IN"),
                               new kid("kid7", "400 South Rotherwood Avenue, Evansville, IN"),
                               new kid("kid8", "1101 North Green River Road, Evansville, IN"),
                               new kid("kid9", "Dexter, Evansville, IN"),
                               new kid("kid10", "200 North Green River Road, Evansville, IN")};
            LocationsCollection a = new LocationsCollection(addresses);
            Assert.AreEqual(a.get_geocode_data_from_api(), true);
            Assert.AreEqual(a.get_distance_based_on_geocode(), true);

            ClustersCollection c = new ClustersCollection(a);
            c.grouping();

            Assert.AreEqual(c.num_of_clusters(), 4);
            Assert.AreEqual(a.find_location(addresses[0]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[1]).get_cluster(), 1);
            Assert.AreEqual(a.find_location(addresses[2]).get_cluster(), 2);
            Assert.AreEqual(a.find_location(addresses[3]).get_cluster(), 2);
            Assert.AreEqual(a.find_location(addresses[4]).get_cluster(), 3);
            Assert.AreEqual(a.find_location(addresses[5]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[6]).get_cluster(), 1);
            Assert.AreEqual(a.find_location(addresses[7]).get_cluster(), 3);
            Assert.AreEqual(a.find_location(addresses[8]).get_cluster(), 0);
            Assert.AreEqual(a.find_location(addresses[9]).get_cluster(), 3);

            List<vehicle> vehicles = new List<vehicle>();
            List<kid> unassigned_kids;
            vehicles.Add(new vehicle(3, "v1"));
            vehicles.Add(new vehicle(2, "v2"));
            vehicles.Add(new vehicle(2, "v3"));
            vehicles.Add(new vehicle(3, "v4"));
            unassigned_kids = c.suit_to_vehicles(vehicles);
            Assert.AreEqual(unassigned_kids, null);

            List<vehicle> test = new List<vehicle>();
            test.Add(new vehicle(3, "v1", new HashSet<kid>() { addresses[4], addresses[7], addresses[9] }));
            test.Add(new vehicle(2, "v2", new HashSet<kid>() { addresses[2], addresses[3] }));
            test.Add(new vehicle(2, "v3", new HashSet<kid>() { addresses[1], addresses[6] }));
            test.Add(new vehicle(3, "v4", new HashSet<kid>() { addresses[0], addresses[5], addresses[8] }));
            Assert.IsTrue(compare_vehicle_lists(test, vehicles));
        }

        [TestMethod]
        public void Fill_vehicles_test3()
        {
            List<kid> addresses = new List<kid>() {new kid("kid1", "1272 Burdette Ave, Evansville, IN"),
                               new kid("kid2", "1800 Lincoln Ave, Evansville, IN"),
                               new kid("kid3", "401 N Burkhardt Rd, Evansville, IN"),
                               new kid("kid4", "6501 E Lloyd Expy, Evansville, IN"),
                               new kid("kid5", "800 N Green River Rd, Evansville, IN"),
                               new kid("kid6", "4605 Bellemeade Avenue, Evansville, IN"),
                               new kid("kid7", "400 South Rotherwood Avenue, Evansville, IN"),
                               new kid("kid8", "1101 North Green River Road, Evansville, IN"),
                               new kid("kid9", "Dexter, Evansville, IN"),
                               new kid("kid10", "200 North Green River Road, Evansville, IN")};
            LocationsCollection a = new LocationsCollection(addresses);
            Assert.AreEqual(a.get_geocode_data_from_api(), true);
            Assert.AreEqual(a.get_distance_based_on_geocode(), true);

            ClustersCollection c = new ClustersCollection(a);
            c.grouping();

            Assert.AreEqual(c.num_of_clusters(), 4);

            List<vehicle> vehicles = new List<vehicle>();
            List<kid> unassigned_kids;
            vehicles.Add(new vehicle(6, "v1"));
            vehicles.Add(new vehicle(6, "v2"));
            unassigned_kids = c.suit_to_vehicles(vehicles);
            Assert.AreEqual(unassigned_kids, null);

            List<vehicle> test = new List<vehicle>();
            test.Add(new vehicle(6, "v1", new HashSet<kid>() { addresses[2], addresses[3], addresses[4], addresses[7], addresses[9] }));
            test.Add(new vehicle(6, "v2", new HashSet<kid>() { addresses[0], addresses[5], addresses[8], addresses[1], addresses[6] }));
            Assert.IsTrue(compare_vehicle_lists(test, vehicles));
        }

        [TestMethod]
        public void Fill_vehicles_test4()
        {
            List<kid> addresses = new List<kid>() {new kid("kid1", "1272 Burdette Ave, Evansville, IN"),
                               new kid("kid2", "1800 Lincoln Ave, Evansville, IN"),
                               new kid("kid3", "401 N Burkhardt Rd, Evansville, IN"),
                               new kid("kid4", "6501 E Lloyd Expy, Evansville, IN"),
                               new kid("kid5", "800 N Green River Rd, Evansville, IN"),
                               new kid("kid6", "4605 Bellemeade Avenue, Evansville, IN"),
                               new kid("kid7", "400 South Rotherwood Avenue, Evansville, IN"),
                               new kid("kid8", "1101 North Green River Road, Evansville, IN"),
                               new kid("kid9", "Dexter, Evansville, IN"),
                               new kid("kid10", "200 North Green River Road, Evansville, IN")};
            LocationsCollection a = new LocationsCollection(addresses);
            Assert.AreEqual(a.get_geocode_data_from_api(), true);
            Assert.AreEqual(a.get_distance_based_on_geocode(), true);

            ClustersCollection c = new ClustersCollection(a);
            c.grouping();

            Assert.AreEqual(c.num_of_clusters(), 4);

            List<vehicle> vehicles = new List<vehicle>();
            List<kid> unassigned_kids;
            vehicles.Add(new vehicle(8, "v1"));
            vehicles.Add(new vehicle(1, "v2"));
            unassigned_kids = c.suit_to_vehicles(vehicles);
            Assert.AreEqual(unassigned_kids[0], addresses[6]);

            List<vehicle> test = new List<vehicle>();
            test.Add(new vehicle(8, "v1", new HashSet<kid>() { addresses[2], addresses[3], addresses[4], addresses[7], addresses[9], addresses[0], addresses[5], addresses[8] }));
            test.Add(new vehicle(1, "v2", new HashSet<kid>() { addresses[1] }));
            Assert.IsTrue(compare_vehicle_lists(test, vehicles));
        }

        private bool compare_vehicle_lists(List<vehicle> v1, List<vehicle> v2)
        {
            for (int i = 0; i < v1.Count; i++)
            {
                if (!compare_kid_lists(v1[i].getKids(), v2[i].getKids()))
                {
                    return false;
                }
            }
            return true;
        }

        private bool compare_kid_lists(HashSet<kid> list1, HashSet<kid> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            foreach (kid k1 in list1)
            {
                if (!list2.Contains(k1))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
