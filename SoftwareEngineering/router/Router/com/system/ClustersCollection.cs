using router.com.system;
using Router.com.system.tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Router.com.system.LocationsCollection;

namespace Router.com.system
{
    public class cluster : strategy
    {
        private List<location> group;
        private int size;
        private int group_num;
        private double ave_distance_from_dream_center = 0;

        public cluster(location a)
        {
            group = new List<location>();
            group.Add(a);

            size = 1;
        }

        public cluster(location[] list, int n)
        {
            group = new List<location>();
            group.AddRange(list);

            size = group.Count;
            assign_cluster_identifier(n);
        }

        public void add(location a)
        {
            if (!group.Contains(a))
            {
                group.Add(a);
                a.assign_cluster(group_num);
                size++;
            }
        }

        public void add_range(List<location> list)
        {
            group.AddRange(list);
            size += list.Count;
            assign_cluster_identifier(group_num);
        }

        public void remove(location a)
        {
            if (group.Contains(a))
            {
                a.assign_cluster(-1);
                group.Remove(a);
                size--;
            }
        }

        public void remove_all()
        {
            group.Clear();
            size = 0;
        }

        public location find(location a)
        {
            foreach (location each in group)
            {
                if (each.Equals(a))
                    return each;
            }
            return null;
        }

        public List<location> get_cluster()
        {
            return group;
        }

        public int get_size()
        {
            return size;
        }

        public int get_cluster_identifier()
        {
            return group_num;
        }

        public void assign_cluster_identifier(int n)
        {
            group_num = n;
            foreach (location each in group)
            {
                each.assign_cluster(n);
            }
        }

        public bool contains(location a)
        {
            return group.Contains(a);
        }

        public void compute_ave_distance_from_dream_center()
        {
            foreach (location each in group)
            {
                ave_distance_from_dream_center += each.get_distance_from_dream_center();
            }
            ave_distance_from_dream_center /= size;
        }

        public double get_ave_distance_from_dream_center()
        {
            return ave_distance_from_dream_center;
        }

        public double distance_to(cluster B)
        {
            double total = 0;
            foreach (location a in group)
            {
                double ave_d = 0;
                foreach (location b in B.get_cluster())
                {
                    ave_d += a.distance_to(b);
                }
                ave_d /= B.get_size();
                total += ave_d;
            }
            total /= size;
            return total;
        }

        public List<vehicle> getVehicles(List<kid> k, List<vehicle> v)
        {
            throw new NotImplementedException();
        }

        public void init(List<kid> k, List<vehicle> v)
        {
            throw new NotImplementedException();
        }
    }










    public class ClustersCollection
    {
        private List<location> points;
        private List<cluster> groups;
        double ave_distance_in_groups;

        public ClustersCollection(LocationsCollection locations)
        {
            points = locations.get_location_list();
        }

        public int num_of_clusters()
        {
            if (groups == null)
            {
                return -1;
            }
            else
            {
                return groups.Count;
            }
        }

        public void grouping()
        {
            int c = 0;
            groups = new List<cluster>();
            foreach (location point in points)
            {
                if (point.get_cluster() == -1 && point.closest_place().get_cluster() == -1)
                {
                    if (point.closest_place().closest_place().Equals(point))
                    {
                        groups.Add(new cluster(new location[] { point, point.closest_place() }, c));
                        c++;
                    }
                }
            }

            if (c < 2)
            {
                location new_group = null;
                double furthest = 0;
                foreach (location point in points)
                {
                    if (point.get_cluster() == -1)
                    {
                        cluster p = new cluster(point);
                        double d = 0;
                        foreach (cluster A in groups)
                        {
                            d = p.distance_to(A);
                            if (d > furthest)
                            {
                                furthest = d;
                                new_group = point;
                            }
                        }
                    }
                }
                if (new_group != null)
                {
                    groups.Add(new cluster(new location[] { new_group }, c));
                }
                else
                {
                    groups.Clear();
                    c = 0;
                    foreach (location point in points)
                    {
                        groups.Add(new cluster(new location[] { point }, c));
                        c++;
                    }
                }

            }

            clustering();
        }

        private void clustering()
        {
            foreach (location point in points)
            {
                if (point.get_cluster() == -1)
                {
                    cluster p = new cluster(point);
                    cluster target_group = closest_group_to(p);
                    target_group.add(point);
                }
            }
        }

        public List<kid> suit_to_vehicles(List<vehicle> vehicles)
        {
            vehicles = vehicles.OrderBy(x => x.getCapacity()).ToList();
            compute_ave_distance_to_center_for_clusters();
            compute_ave_distance_between_clusters();

            cluster most_distant_cluster = find_most_distant_cluster();
            while (!has_filled_all_vehicles(vehicles) && most_distant_cluster != null)
            {
                vehicle proper_car = find_appropriate_vehicle_to(most_distant_cluster, vehicles);
                fill_vehicle_and_reset_clusters(proper_car, vehicles, most_distant_cluster);
                most_distant_cluster = find_most_distant_cluster();
            }

            List<kid> unassigned_kids = collect_kids_left();
            return unassigned_kids;
        }

        private List<kid> collect_kids_left()
        {
            if (groups.Count == 0)
                return null;

            List<kid> unassigned_kids = new List<kid>();
            foreach (cluster c in groups)
            {
                foreach (location l in c.get_cluster())
                {
                    unassigned_kids.Add(l.get_location_info());
                }
            }
            return unassigned_kids;
        }

        private void fill_vehicle_and_reset_clusters(vehicle proper_car, List<vehicle> vehicles, cluster most_distant_cluster)
        {
            if (proper_car.getCapacity() < most_distant_cluster.get_size())
            {
                fill_vehicle_with_max_capacity(proper_car, vehicles, most_distant_cluster);
            }
            else
            {
                fill_vehicle_and_may_pull_extra_kids(proper_car, vehicles, most_distant_cluster);
            }
        }

        private void fill_vehicle_with_max_capacity(vehicle proper_car, List<vehicle> vehicles, cluster most_distant_cluster)
        {
            //int extra_kids = most_distant_cluster.get_size() - proper_car.getCapacity();
            cluster closest_cluster = closest_group_to(most_distant_cluster);
            if (closest_cluster != null)
            {
                fill_vehicle_with_appropriate_kids(proper_car, most_distant_cluster, closest_cluster);
                //proper_car.fill_kids(most_distant_cluster.get_cluster());
                //most_distant_cluster = find_most_distant_cluster();
            }
            else
            {
                split();
                most_distant_cluster = find_most_distant_cluster();
                //closest_cluster = closest_group_to(most_distant_cluster);
                proper_car = find_appropriate_vehicle_to(most_distant_cluster, vehicles);
                fill_vehicle_and_reset_clusters(proper_car, vehicles, most_distant_cluster);
            }
        }

        private void fill_vehicle_with_appropriate_kids(vehicle proper_car, cluster most_distant_cluster, cluster closest_cluster)
        {
            List<location> left_kids = new List<location>();
            //List<location> selected_kids = new List<location>();
            int extra_kids = most_distant_cluster.get_size() - proper_car.getCapacity();
            while (extra_kids > 0)
            {
                location closest = find_closest_location(most_distant_cluster, closest_cluster);
                left_kids.Add(closest);
                most_distant_cluster.remove(closest);
                extra_kids--;
            }
            proper_car.fill_kids(most_distant_cluster.get_cluster());

            most_distant_cluster.remove_all();
            most_distant_cluster.add_range(left_kids);
        }

        private void fill_vehicle_and_may_pull_extra_kids(vehicle proper_car, List<vehicle> vehicles, cluster most_distant_cluster)
        {
            int total_extra_seats = current_total_capacity_of(vehicles) - current_size_of_clusters();
            cluster closest_cluster = closest_group_to(most_distant_cluster);
            int extra_seats = proper_car.getCapacity() - most_distant_cluster.get_size();
            if (closest_cluster == null || extra_seats == 0)
            {
                fill_vehicle_with_all_kids_in_cluster(proper_car, most_distant_cluster);
            }
            else
            {
                if (extra_seats < total_extra_seats && extra_seats < 0.2 * proper_car.getCapacity()
                     && most_distant_cluster.distance_to(closest_cluster) > ave_distance_in_groups)
                {
                    fill_vehicle_with_all_kids_in_cluster(proper_car, most_distant_cluster);
                }
                else
                {
                    fill_vehicle_and_pick_extra_kids(proper_car, most_distant_cluster, total_extra_seats);
                }
            }
        }

        private int current_size_of_clusters()
        {
            int total = 0;
            foreach (cluster each in groups)
            {
                total += each.get_size();
            }
            return total;
        }

        private bool has_filled_all_vehicles(List<vehicle> vehicles)
        {
            foreach (vehicle each in vehicles)
            {
                if (!each.has_filled_kids())
                    return false;
            }
            return true;
        }

        private int current_total_capacity_of(List<vehicle> vehicles)
        {
            int total = 0;
            foreach (vehicle each in vehicles)
            {
                if (!each.has_filled_kids())
                    total += each.getCapacity();
            }
            return total;
        }

        private void fill_vehicle_with_all_kids_in_cluster(vehicle proper_car, cluster most_distant_cluster)
        {
            proper_car.fill_kids(most_distant_cluster.get_cluster());
            groups.Remove(most_distant_cluster);
        }

        private void fill_vehicle_and_pick_extra_kids(vehicle proper_car, cluster most_distant_cluster, int total_extra_seats)
        {
            int extra_seats = proper_car.getCapacity() - most_distant_cluster.get_size();
            cluster closest_cluster = closest_group_to(most_distant_cluster);
            while (closest_cluster != null && extra_seats >= closest_cluster.get_size())
            {
                extra_seats -= closest_cluster.get_size();
                merge_groups(most_distant_cluster, closest_cluster);
                closest_cluster = closest_group_to(most_distant_cluster);
            }

            fill_vehicle_with_all_kids_in_cluster(proper_car, most_distant_cluster);
            if (extra_seats > 0 && closest_cluster != null && !(extra_seats < total_extra_seats
                && extra_seats < 0.2 * proper_car.getCapacity()
                && most_distant_cluster.distance_to(closest_cluster) > ave_distance_in_groups))
            {
                pull_appropriate_kids(proper_car, most_distant_cluster, closest_cluster, extra_seats);
            }

        }

        private void pull_appropriate_kids(vehicle proper_car, cluster most_distant_cluster, cluster closest_cluster, int extra_seats)
        {
            List<location> selected_kids = new List<location>();
            //List<location> selected_kids = new List<location>();
            while (extra_seats > 0)
            {
                location closest = find_closest_location(closest_cluster, most_distant_cluster);
                selected_kids.Add(closest);
                closest_cluster.remove(closest);
                extra_seats--;
            }
            proper_car.fill_kids(selected_kids);
        }

        private location find_closest_location(cluster target_cluster, cluster adjacent_cluster)
        {
            double d = 9999;
            location target = null;
            foreach (location each in target_cluster.get_cluster())
            {
                cluster c = new cluster(each);
                if (c.distance_to(adjacent_cluster) < d)
                {
                    d = c.distance_to(adjacent_cluster);
                    target = each;
                }
            }
            return target;
        }

        private vehicle find_largest_unfilled_car(List<vehicle> vehicles)
        {
            int largest = 0;
            vehicle target_car = null;
            foreach (vehicle v in vehicles)
            {
                if (!v.has_filled_kids() && v.getCapacity() > largest)
                {
                    largest = v.getCapacity();
                    target_car = v;
                }
            }
            return target_car;
        }

        private vehicle find_appropriate_vehicle_to(cluster most_distant_cluster, List<vehicle> vehicles)
        {
            foreach (vehicle v in vehicles)
            {
                if (!v.has_filled_kids() && v.getCapacity() >= most_distant_cluster.get_size())
                    return v;
            }
            return find_largest_unfilled_car(vehicles);
        }

        private cluster closest_group_to(cluster o)
        {
            double closest = 9999;
            cluster closest_group = null;
            foreach (cluster group in groups)
            {
                if (!group.Equals(o))
                {
                    double d = o.distance_to(group); //distance_between_groups(o, group);
                    if (d < closest)
                    {
                        closest = d;
                        closest_group = group;
                    }
                }
            }
            return closest_group;
        }

        private void merge_groups(cluster A, cluster B)
        {
            groups.Remove(B);
            foreach (cluster each in groups)
            {
                if (each.Equals(A))
                {
                    //foreach(location a in A)
                    //{
                    //a.hebing = true;
                    //}
                    each.add_range(B.get_cluster());
                }
            }
        }

        private void compute_ave_distance_to_center_for_clusters()
        {
            foreach (cluster each in groups)
            {
                each.compute_ave_distance_from_dream_center();
            }
        }

        private void compute_ave_distance_between_clusters()
        {
            double total = 0;
            foreach (cluster a in groups)
            {
                foreach (cluster b in groups)
                {
                    if (!a.Equals(b))
                    {
                        total += a.distance_to(b); //distance_between_groups(a, b);
                    }
                }
                total /= num_of_clusters();
            }
            total /= num_of_clusters();
            ave_distance_in_groups = total;
        }

        private cluster find_most_distant_cluster()
        {
            double most_distant = 0;
            cluster target = null;
            foreach (cluster each in groups)
            {
                if (each.get_ave_distance_from_dream_center() > most_distant)
                {
                    most_distant = each.get_ave_distance_from_dream_center();
                    target = each;
                }
            }
            return target;
        }

        private void split()
        {
            List<location> current_locations_in_groups = new List<location>();
            foreach (location each in groups[0].get_cluster())
            {
                each.assign_cluster(-1);
                current_locations_in_groups.Add(each);
            }
            points = current_locations_in_groups;
            grouping();
            compute_ave_distance_to_center_for_clusters();
        }
    }
}
